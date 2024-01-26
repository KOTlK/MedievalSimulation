using System;
using UnityEngine;

namespace Simulation.Runtime.Entities
{
    public static class EntityManager
    {
        public static Entity[] Entities = new Entity[100];
        public static int      EntitiesCount;

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

            Entities[id].Id = id;
            Entities[id].IsAlive = true;
            
            return id;
        }

        public static int CreateEntity(Entity config)
        {
            var id = CreateEntity();
            Entities[id].Orientation = config.Orientation;
            Entities[id].Position = config.Position;
            Entities[id].Flags = config.Flags;
            Entities[id].EntityType = config.EntityType;

            Debug.Log($"Entity Created: {Entities[id].EntityType.ToString("F")}, {Entities[id].Id.ToString()}");
            return id;
        }

        public static void DeleteEntity(int id)
        {
            Debug.Log($"Removing Entity: {Entities[id].EntityType.ToString("F")}, {Entities[id].Id.ToString()}");
            Entities[id] = default;
            Entities[id].IsAlive = false;
            Entities[id].Id = -1;
            if (_freeEntitiesCount == _freeEntities.Length)
            {
                Array.Resize(ref _freeEntities, _freeEntitiesCount << 1);
            }

            _freeEntities[_freeEntitiesCount++] = id;
            EntitiesCount--;
        }
    }
}