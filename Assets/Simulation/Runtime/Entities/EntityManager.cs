using System;
using Simulation.Runtime.View;
using UnityEngine;

namespace Simulation.Runtime.Entities
{
    public static class EntityManager
    {
        public static Entity[] Entities = new Entity[100];
        public static Farmer[] Farmers = new Farmer[100];
        public static int      EntitiesCount;
        public static int      FarmersCount;

        private static int[]   _freeEntities = new int[100];
        private static int     _freeEntitiesCount;
        
        public static bool IsAlive(int entity)
        {
            return Entities[entity].IsAlive;
        }
        
        public static int CreateEntity()
        {
            var id = -1;

            if (_freeEntitiesCount > 0)
            {
                id = _freeEntities[--_freeEntitiesCount];
            }
            else
            {
                if (EntitiesCount == Entities.Length)
                {
                    Array.Resize(ref Entities, EntitiesCount << 1);
                }
                
                id = EntitiesCount++;
            }

            Entities[id] = default;
            Entities[id].IsAlive = true;
            
            return id;
        }

        public static int CreateEntity(Entity config)
        {
            var id = CreateEntity();
            Entities[id].Orientation = config.Orientation;
            Entities[id].Position = config.Position;
            Entities[id].Flags = config.Flags;

            return id;
        }

        public static void DeleteEntity(int id)
        {
            Entities[id].IsAlive = false;
            EntitiesCount--;
            if (_freeEntitiesCount == _freeEntities.Length)
            {
                Array.Resize(ref _freeEntities, _freeEntitiesCount << 1);
            }
            _freeEntities[_freeEntitiesCount++] = id;
            Rendering.DestroyView(id);
        }

        public static int CreateFarmer(Farmer farmer, Vector3 position)
        {
            var entity = CreateEntity();
            farmer.Entity = entity;
            Entities[entity].Position = position;
            Entities[entity].Orientation = -1f;
            Entities[entity].Flags = EntityFlags.Tickable | EntityFlags.Farmer;
            farmer.Index = FarmersCount++;

            if (farmer.Index == Farmers.Length)
            {
                Array.Resize(ref Farmers, farmer.Index << 1);
            }
            
            Farmers[farmer.Index] = farmer;

            Rendering.CreateFarmer(entity, farmer.Sex, position);

            return farmer.Index;
        }

        public static void DeleteFarmer(int id)
        {
            if (id >= FarmersCount)
            {
                Debug.LogError("Index can't be more than or equals FarmersCount");
            }

            DeleteEntity(Farmers[id].Entity);
            Farmers[id] = Farmers[--FarmersCount];
            Farmers[id].Index = id;
            Farmers[FarmersCount] = default;
        }
    }
}