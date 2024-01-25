using System;
using UnityEngine;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.View.Rendering;
using static Simulation.Runtime.Entities.Mining;
using Random = UnityEngine.Random;

namespace Simulation.Runtime.Entities
{
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

    public static class GameWorld
    {
        public static Vector2Int Size;
        public static Cell[] Cells;
        public static WorldBounds Bounds;
        public static Vector2 CellSize = new (1f, 1f);
        public static int WorldSeed = 87645;
        public static float ResourceDepositChance = 0.02f;

        public static void CreateWorld(Vector2Int size, WorldGeneration genFunc )
        {
            Size = size;
            Cells = new Cell[size.x * size.y];

            for (var y = 0; y < size.y; ++y)
            {
                for (var x = 0; x < size.x; ++x)
                {
                    var entity = CreateEntity();
                    var position = new Vector3(x * CellSize.x, y * CellSize.y, 0);
                    var content = genFunc(x, y);

                    if (content == CellContent.Rock)
                    {
                        //generate random resource deposit or no

                        var rand = Random.Range(0f, 1f);

                        if (rand <= ResourceDepositChance)
                        {
                            CreateRandomResourceDeposit(position);
                        }
                    }
                    var cell = new Cell()
                    {
                        Content = genFunc(x, y),
                        Entity = entity,
                        Position = position
                    };

                    SetCell(x, y, cell);
                    EntityManager.Entities[entity].Position = position;
                    EntityManager.Entities[entity].Flags = EntityFlags.WorldCell;
                }
            }

            Bounds = new WorldBounds()
            {
                Max = new Vector2(size.x - 1 + CellSize.x / 2f, size.y - 1 + CellSize.y / 2f),
                Min = new Vector2(0 - CellSize.x / 2f, 0 - CellSize.y / 2f)
            };
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

        public static Cell GetCell(int x, int y) => Cells[x + y * Size.x];
        public static void SetCell(int x, int y, Cell cell) => Cells[x + y * Size.x] = cell;
    }
}