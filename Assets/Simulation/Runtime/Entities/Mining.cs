using System;
using UnityEngine;
using Random = UnityEngine.Random;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.Entities.GameWorld;
using static Simulation.Runtime.View.Rendering;

namespace Simulation.Runtime.Entities
{
    public struct ResourceDeposit
    {
        public DepositType Type;
        public int Entity;
        public int ResourcesLeft;
    }
    
    public enum DepositType
    {
        Iron,
        Coal,
    }

    [Serializable]
    public struct DepositChances
    {
        public DepositType Type;
        public float Chance;
    }
    
    public static class Mining
    {
        public static int MinResourcesCount;
        public static int MaxResourcesCount;
        
        //Higher chance goes lower
        public static readonly DepositChances[] DepositChance = {
            new()
            {
                Type = DepositType.Iron,
                Chance = 0.5f
            },
            new()
            {
                Type = DepositType.Coal,
                Chance = 1f
            }
        };
        
        public static DepositType GetRandomDepositType()
        {
            var deposit = DepositType.Coal;
            var random = Random.Range(0f, 1f);
            
            for (var i = 0; i < DepositChance.Length; ++i)
            {
                ref var chances = ref DepositChance[i];
                if (random <= chances.Chance)
                {
                    deposit = chances.Type;
                    break;
                }
            }

            return deposit;
        }

        public static void CreateResourceDeposit(ResourceDeposit deposit, Vector3 position)
        {
            var entity = CreateEntity();

            EntityManager.Entities[entity].Position = position;

            deposit.Entity = entity;

            if (ResourcesCount == GameWorld.Resources.Length)
            {
                Array.Resize(ref GameWorld.Resources, ResourcesCount << 1);
            }
            
            GameWorld.Resources[ResourcesCount++] = deposit;
            InstantiateCellContentView(CellContent.ResourceDeposit, entity, deposit.Type.ToString());
        }

        public static void CreateRandomResourceDeposit(Vector3 position)
        {
            CreateResourceDeposit(new ResourceDeposit()
            {
                Type = GetRandomDepositType(),
                ResourcesLeft = Random.Range(MinResourcesCount, MaxResourcesCount)
            }, position);
        }

        public static void DestroyResourceDeposit(int index)
        {
            var entity = GameWorld.Resources[index].Entity;
            DeleteEntity(entity);
            GameWorld.Resources[index] = GameWorld.Resources[--ResourcesCount];
            GameWorld.Resources[ResourcesCount] = default;
            DestroyView(entity);
        }
    }
}