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