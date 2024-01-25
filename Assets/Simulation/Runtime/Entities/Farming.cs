using System;
using Simulation.Runtime.View;
using UnityEngine;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.View.Rendering;

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
        public static void CreateCrop(ref World world, Crop crop, Vector3 position)
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

            world.Crops[world.CropsCount++] = crop;
            DrawCropFirst(ref crop);
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

        public static void DestroyCrop(ref World world, int entity)
        {
            world.Crops[entity] = default;
            DeleteEntity(entity);
            Rendering.DestroyCrop(entity);
        }
    }
}