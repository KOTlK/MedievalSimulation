using System;
using UnityEngine;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.View.Rendering;

namespace Simulation.Runtime.Entities
{
    public struct World
    {
        public Vector2Int Size;
        public Cell[] Cells;
        public Crop[] Crops;
        public int CropsCount;
        public WorldBounds Bounds;

        public Cell this[int x, int y]
        {
            get => Cells[x + y * Size.x];
            set => Cells[x + y * Size.x] = value;
        }
    }

    public struct Cell
    {
        public Vector3 Position;
        public CellContent Content;
        public int Entity;
    }

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

    public struct ResourceDeposit
    {
        public DepositType Type;
        public int Entity;
        public int ResourcesLeft;
    }

    public struct Food
    {
        public FoodType Type;
        public float Nutrition;
    }

    public enum FoodType
    {
        Meat,
        Vegetable,
        Fruit,
        Grain
    }
    
    [Flags]
    public enum CellContent : ulong
    {
        Soil            = 1 << 0,
        Rock            = 1 << 1,
        ResourceDeposit = 1 << 2,
        SmallWater      = 1 << 3,
        DeepWater       = 1 << 4,
        Crops           = 1 << 5,
    }

    public enum DepositType
    {
        Iron,
        Coal,
    }

    public enum CropsType
    {
        Wheat,
    }

    public struct WorldBounds
    {
        public Vector2 Max;
        public Vector2 Min;
    }

    public delegate CellContent WorldGeneration(int x, int y);

    public static class WorldUtils
    {
        public static Vector2 CellSize = new (1f, 1f);
        public static World World;
        public static int WorldSeed = 87645;

        public static void CreateWorld(Vector2Int size, WorldGeneration genFunc )
        {
            var world = new World();

            world.Size = size;
            world.Cells = new Cell[size.x * size.y];
            world.Crops = new Crop[10];
            world.CropsCount = 0;

            for (var y = 0; y < size.y; ++y)
            {
                for (var x = 0; x < size.x; ++x)
                {
                    var entity = CreateEntity();
                    var position = new Vector3(x * CellSize.x, y * CellSize.y, 0);

                    world[x, y] = new Cell()
                    {
                        Content = genFunc(x, y),
                        Entity = entity,
                        Position = position
                    };
                    EntityManager.Entities[entity].Position = position;
                    EntityManager.Entities[entity].Flags = EntityFlags.WorldCell;
                }
            }

            world.Bounds = new WorldBounds()
            {
                Max = new Vector2(size.x - 1 + CellSize.x / 2f, size.y - 1 + CellSize.y / 2f),
                Min = new Vector2(0 - CellSize.x / 2f, 0 - CellSize.y / 2f)
            };

            World = world;
        }

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

        public static CellContent PerlinNoise(int x, int y)
        {
            var rand = Mathf.PerlinNoise(x * 0.1f + WorldSeed, y * 0.1f + WorldSeed);

            if (rand >= 0.6f)
            {
                return CellContent.Rock;
            }
            else
            {
                return CellContent.Soil;
            }
        }
    }
}