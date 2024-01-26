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
    
    public static class Units
    {
        public static Farmer[] Farmers = new Farmer[10];
        public static int FarmersCount = 0;
        
        public static int CreateFarmer(Farmer farmer, Vector3 position)
        {
            var entity = CreateEntity(new Entity
            {
                Position = position,
                Orientation = -1f,
                Flags = EntityFlags.Tickable,
                EntityType = EntityType.Unit
            });
            farmer.Entity = entity;
            
            if (FarmersCount == Farmers.Length)
            {
                Array.Resize(ref Farmers, FarmersCount << 1);
            }
            
            Farmers[FarmersCount++] = farmer;

            InstantiateFarmer(entity, farmer.Sex, position);

            return entity;
        }

        public static void DeleteFarmer(int id)
        {
            if (id >= FarmersCount)
            {
                Debug.LogError("Index can't be more than or equals FarmersCount");
            }

            DeleteEntity(Farmers[id].Entity);
            Farmers[id] = Farmers[--FarmersCount];
            Farmers[FarmersCount] = default;
        }
    }
}