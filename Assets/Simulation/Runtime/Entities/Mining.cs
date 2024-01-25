using System;
using UnityEngine;
using Random = UnityEngine.Random;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.View.Rendering;
using static Simulation.Runtime.Entities.GameWorld;

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
        public static ResourceDeposit[] Resources;
        public static int ResourcesCount;
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

        public static void InitializeMining()
        {
            Resources = new ResourceDeposit[10];
            ResourcesCount = 0;
        }
        
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

        public static int CreateResourceDeposit(ResourceDeposit deposit, Vector3 position)
        {
            var entity = CreateEntity();
            ref var cell = ref GetCellReference((int)position.x, (int)position.y);

            EntityManager.Entities[entity].Position = position;

            deposit.Entity = entity;

            if (ResourcesCount == Resources.Length)
            {
                Array.Resize(ref Resources, ResourcesCount << 1);
            }
            
            Resources[ResourcesCount++] = deposit;
            InstantiateCellContentView(CellContent.ResourceDeposit, entity, deposit.Type.ToString());

            cell.ContainsContent = true;
            cell.ContentEntity = entity;
            cell.Content = CellContent.ResourceDeposit;
            
            return entity;
        }

        public static int CreateRandomResourceDeposit(Vector3 position)
        {
            return CreateResourceDeposit(new ResourceDeposit()
            {
                Type = GetRandomDepositType(),
                ResourcesLeft = Random.Range(MinResourcesCount, MaxResourcesCount)
            }, position);
        }

        //TODO: find cell and remove resource from it
        public static void DestroyResourceDeposit(int index)
        {
            var entity = Resources[index].Entity;
            DeleteEntity(entity);
            Resources[index] = Resources[--ResourcesCount];
            Resources[ResourcesCount] = default;
            DestroyView(entity);
        }
    }
}