using System;
using Simulation.Runtime.View;
using UnityEngine;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.View.Rendering;
using static Simulation.Runtime.Entities.GameWorld;

namespace Simulation.Runtime.Entities
{
    
    [Serializable]
    public struct Crop
    {
        public CropsType Type;
        public GrowStage Stage;
        public int Entity;
        public int CropsPerHarvest;
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
        public static Crop[] Crops;
        public static int CropsCount;

        public static void InitializeFarming()
        {
            Crops = new Crop[10];
            CropsCount = 0;
        }
        
        public static void CreateCrop(Crop crop, Vector3 position)
        {
            ref var cell = ref GetCellReference((int)position.x, (int)position.y);
            if (cell.ContainsContent) return;
            
            
            if (cell.Type == CellType.Soil)
            {
                var cropEntity = CreateEntity(new Entity()
                {
                    Position = position,
                    Orientation = 0f,
                    Flags = EntityFlags.Tickable
                });
            
                crop.Entity = cropEntity;
                crop.GrowingTime = 0f;
                crop.TimeSinceLastWatering = 0f;
                crop.Stage = GrowStage.Seeds;

                if (CropsCount == Crops.Length)
                {
                    Array.Resize(ref Crops, CropsCount << 1);
                }

                Crops[CropsCount++] = crop;
                cell.ContainsContent = true;
                cell.ContentEntity = cropEntity;
                cell.Content = CellContent.Crops;
            
                InstantiateCellContentView(CellContent.Crops, cropEntity, crop.Type.ToString());
            }

        }
        
        public static void TickCrops(Crop[] crops, int cropsCount)
        {
            for (var i = 0; i < cropsCount; ++i)
            {
                if (crops[i].Stage == GrowStage.Faded)
                {
                    continue;
                }
                
                crops[i].GrowingTime += Time.deltaTime;
                crops[i].TimeSinceLastWatering += Time.deltaTime;

                if (crops[i].TimeSinceLastWatering >= crops[i].TimeToDry)
                {
                    crops[i].Stage = GrowStage.Faded;
                    DrawCrop(ref crops[i]);
                    continue;
                }

                var growPercent = crops[i].GrowingTime / crops[i].TimeToGrow;

                if (growPercent is > 0.25f and < 0.5f)
                {
                    crops[i].Stage = GrowStage.Shoots;
                    DrawCrop(ref crops[i]);
                } else if (growPercent is > 0.5f and < 0.75f)
                {
                    crops[i].Stage = GrowStage.HalfGrown;
                    DrawCrop(ref crops[i]);
                }else if (growPercent is > 0.75f and < 1f)
                {
                    crops[i].Stage = GrowStage.AlmostGrown;
                    DrawCrop(ref crops[i]);
                }else if (growPercent >= 1f)
                {
                    crops[i].Stage = GrowStage.FullyGrown;
                    DrawCrop(ref crops[i]);
                }
            }
        }

        //TODO: find cell and remove content from it
        public static void DestroyCrop(int index)
        {
            var entity = Crops[index].Entity;
            DeleteEntity(entity);
            Crops[index] = Crops[--CropsCount];
            Crops[CropsCount] = default;
            Rendering.DestroyCrop(entity);
        }
    }
}