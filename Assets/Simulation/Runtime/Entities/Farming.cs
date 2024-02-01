using System;
using System.Collections.Generic;
using UnityEngine;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.Entities.WorldUtils;

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
        private static Dictionary<int, int> _indexByEntity = new();
        
        public static void CreatePlant(ref LocalGrid grid, Plant plant, Vector3Int position)
        {
            if (IsCellFree(ref grid, position.x, position.y) == false) return;
            
            if (GetCellType(ref grid, position.x, position.y) == WorldCellType.Soil)
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

                if (grid.PlantsCount == grid.Plants.Length)
                {
                    Array.Resize(ref grid.Plants, grid.PlantsCount << 1);
                }

                var index = grid.PlantsCount++;
                grid.Plants[index] = plant;

                FillCellContent(ref grid, position, CellContent.Plants, plantEntity);

                _indexByEntity[plantEntity] = index;
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
                    continue;
                }

                var growPercent = plants[i].GrowingTime / plants[i].TimeToGrow;

                if (growPercent is > 0.25f and < 0.5f)
                {
                    plants[i].Stage = GrowStage.Shoots;
                } else if (growPercent is > 0.5f and < 0.75f)
                {
                    plants[i].Stage = GrowStage.HalfGrown;
                }else if (growPercent is > 0.75f and < 1f)
                {
                    plants[i].Stage = GrowStage.AlmostGrown;
                }else if (growPercent >= 1f)
                {
                    plants[i].Stage = GrowStage.FullyGrown;
                }
            }
        }

        public static void DestroyPlant(ref LocalGrid grid, int index)
        {
            var entity = grid.Plants[index].Entity;
            var position = EntityManager.Entities[entity].Position;

            grid.Plants[index] = grid.Plants[--grid.PlantsCount];
            grid.Plants[grid.PlantsCount] = default;
            RemoveCellContent(ref grid, new Vector3Int((int)position.x, (int)position.y));
            DeleteEntity(entity);
            _indexByEntity[grid.Plants[index].Entity] = index;
        }

        public static int GetPlantIdByEntity(int entity) => _indexByEntity[entity];
    }
}