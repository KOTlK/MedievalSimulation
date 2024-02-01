using System;
using UnityEngine;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.View.Rendering;

namespace Simulation.Runtime.Entities
{
    public struct Farmer
    {
        public int Entity;
        /// <summary>
        /// True - male, false - female
        /// </summary>
        public bool Sex;
    }

    public enum UnitType
    {
        Farmer
    }
    
    public static class Units
    {
        public static int CreateFarmer(ref LocalGrid grid, Farmer farmer, Vector3 position)
        {
            var entity = CreateEntity(new Entity
            {
                Position = position,
                Orientation = -1f,
                Flags = EntityFlags.Tickable,
                EntityType = EntityType.Unit
            });
            farmer.Entity = entity;
            
            if (grid.FarmersCount == grid.Farmers.Length)
            {
                Array.Resize(ref grid.Farmers, grid.FarmersCount << 1);
            }
            
            grid.Farmers[grid.FarmersCount++] = farmer;

            InstantiateFarmer(entity, farmer.Sex, position);

            return entity;
        }

        public static void DeleteFarmer(ref LocalGrid grid, int id)
        {
            if (id >= grid.FarmersCount)
            {
                Debug.LogError("Index can't be more than or equals FarmersCount");
            }

            DeleteEntity(grid.Farmers[id].Entity);
            grid.Farmers[id] = grid.Farmers[--grid.FarmersCount];
            grid.Farmers[grid.FarmersCount] = default;
        }
    }
}