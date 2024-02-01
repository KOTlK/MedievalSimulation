using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.Entities.WorldUtils;
using static Simulation.Runtime.Vars;

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

        public static int CreateResourceDeposit(ref LocalGrid grid, ResourceDeposit deposit, Vector3Int position)
        {
            var entity = CreateEntity(new Entity
            {
                Position = position,
                Flags = EntityFlags.Static,
                EntityType = EntityType.Resource
            });

            deposit.Entity = entity;

            if (grid.ResourcesCount == grid.Resources.Length)
            {
                Array.Resize(ref grid.Resources, grid.ResourcesCount << 1);
            }

            var index = grid.ResourcesCount++;
            grid.Resources[index] = deposit;

            FillCellContent(ref grid, position, CellContent.ResourceDeposit, entity);

            _indexByEntity[entity] = index;
            
            return entity;
        }

        public static int CreateRandomResourceDeposit(ref LocalGrid localGrid, Vector3Int position)
        {
            return CreateResourceDeposit(
                ref localGrid,
                new ResourceDeposit
                {
                    Type = GetRandomDepositType(),
                    ResourcesLeft = Random.Range(MinResourcesCount, MaxResourcesCount)
                }, position
            );
        }

        public static void DestroyResourceDeposit(ref LocalGrid grid, int index)
        {
            var entity = grid.Resources[index].Entity;
            var position = EntityManager.Entities[entity].Position;

            grid.Resources[index] = grid.Resources[--grid.ResourcesCount];
            grid.Resources[grid.ResourcesCount] = default;
            RemoveCellContent(ref grid, new Vector3Int((int)position.x, (int)position.y));
            DeleteEntity(entity);
            _indexByEntity[grid.Resources[index].Entity] = index;
        }
        
        public static int GetResourceIdByEntity(int entity) => _indexByEntity[entity];
    }
}