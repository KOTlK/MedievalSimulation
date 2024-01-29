using System;
using System.Collections.Generic;
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
        public static ResourceDeposit[] Resources = new ResourceDeposit[10];
        public static int ResourcesCount = 0;
        public static int MinResourcesCount = 200;
        public static int MaxResourcesCount = 10000;
        
        private static Dictionary<int, int> _indexByEntity = new();
        
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

        public static int CreateResourceDeposit(ResourceDeposit deposit, Vector3Int position)
        {
            var entity = CreateEntity(new Entity
            {
                Position = position,
                Flags = EntityFlags.Static,
                EntityType = EntityType.Resource
            });

            deposit.Entity = entity;

            if (ResourcesCount == Resources.Length)
            {
                Array.Resize(ref Resources, ResourcesCount << 1);
            }

            var index = ResourcesCount++;
            Resources[index] = deposit;
            InstantiateCellContentView(CellContent.ResourceDeposit, entity, deposit.Type.ToString());

            FillCellContent(position.x, position.y, CellContent.ResourceDeposit, entity);

            _indexByEntity[entity] = index;
            
            return entity;
        }

        public static int CreateRandomResourceDeposit(Vector3Int position)
        {
            return CreateResourceDeposit(new ResourceDeposit()
            {
                Type = GetRandomDepositType(),
                ResourcesLeft = Random.Range(MinResourcesCount, MaxResourcesCount)
            }, position);
        }

        public static void DestroyResourceDeposit(int index)
        {
            var entity = Resources[index].Entity;
            var position = EntityManager.Entities[entity].Position;

            Resources[index] = Resources[--ResourcesCount];
            Resources[ResourcesCount] = default;
            RemoveCellContent((int)position.x, (int)position.y);
            DeleteEntity(entity);
            ReleaseView(entity);
            _indexByEntity[Resources[index].Entity] = index;
        }

        public static ref ResourceDeposit GetResourceByEntity(int entity)
        {
            return ref Resources[_indexByEntity[entity]];
        }
        
        public static int GetResourceIdByEntity(int entity) => _indexByEntity[entity];
    }
}