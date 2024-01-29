using System;
using System.Collections.Generic;
using UnityEngine;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.View.Rendering;
using static Simulation.Runtime.Entities.GameWorld;

namespace Simulation.Runtime.Entities
{
    
    [Serializable]
    public struct Plant
    {
        public CropsType Type;
        public GrowStage Stage;
        public int Entity;
        public int ResourcesPerHarvest;
        public float TimeToGrow;
        public float GrowingTime;
        public float TimeSinceLastWatering;
        public float TimeToDry;
    }

    public enum GrowStage
    {
        Seeds,
        Shoots,
        HalfGrown,
        AlmostGrown,
        FullyGrown,
        Faded
    }
        
    public enum CropsType
    {
        Wheat,
    }
    
    public static class Farming
    {
        public static Plant[] Plants = new Plant[10];
        public static int PlantsCount = 0;

        private static Dictionary<int, int> _indexByEntity = new();
        
        public static void CreatePlant(Plant plant, Vector3Int position)
        {
            if (IsCellFree(position.x, position.y) == false) return;
            
            if (GetCellType(position.x, position.y) == CellType.Soil)
            {
                var plantEntity = CreateEntity(new Entity()
                {
                    Position = position,
                    Orientation = 0f,
                    Flags = EntityFlags.Tickable,
                    EntityType = EntityType.Resource
                });
            
                plant.Entity = plantEntity;
                plant.GrowingTime = 0f;
                plant.TimeSinceLastWatering = 0f;
                plant.Stage = GrowStage.Seeds;

                if (PlantsCount == Plants.Length)
                {
                    Array.Resize(ref Plants, PlantsCount << 1);
                }

                var index = PlantsCount++;
                Plants[index] = plant;

                FillCellContent(position.x, position.y, CellContent.Plants, plantEntity);

                _indexByEntity[plantEntity] = index;
                InstantiateCellContentView(CellContent.Plants, plantEntity, plant.Type.ToString());
            }

        }
        
        public static void TickPlants(Plant[] plants, int plantsCount)
        {
            for (var i = 0; i < plantsCount; ++i)
            {
                if (plants[i].Stage == GrowStage.Faded)
                {
                    continue;
                }
                
                plants[i].GrowingTime += Time.deltaTime;
                plants[i].TimeSinceLastWatering += Time.deltaTime;

                if (plants[i].TimeSinceLastWatering >= plants[i].TimeToDry)
                {
                    plants[i].Stage = GrowStage.Faded;
                    DrawPlant(ref plants[i]);
                    continue;
                }

                var growPercent = plants[i].GrowingTime / plants[i].TimeToGrow;

                if (growPercent is > 0.25f and < 0.5f)
                {
                    plants[i].Stage = GrowStage.Shoots;
                    DrawPlant(ref plants[i]);
                } else if (growPercent is > 0.5f and < 0.75f)
                {
                    plants[i].Stage = GrowStage.HalfGrown;
                    DrawPlant(ref plants[i]);
                }else if (growPercent is > 0.75f and < 1f)
                {
                    plants[i].Stage = GrowStage.AlmostGrown;
                    DrawPlant(ref plants[i]);
                }else if (growPercent >= 1f)
                {
                    plants[i].Stage = GrowStage.FullyGrown;
                    DrawPlant(ref plants[i]);
                }
            }
        }

        public static void DestroyPlant(int index)
        {
            var entity = Plants[index].Entity;
            var position = EntityManager.Entities[entity].Position;

            Plants[index] = Plants[--PlantsCount];
            Plants[PlantsCount] = default;
            RemoveCellContent((int)position.x, (int)position.y);
            DeleteEntity(entity);
            ReleasePlant(entity);
            _indexByEntity[Plants[index].Entity] = index;
        }

        public static ref Plant GetPlantByEntity(int entity)
        {
            return ref Plants[_indexByEntity[entity]];
        }

        public static int GetPlantIdByEntity(int entity) => _indexByEntity[entity];
    }
}