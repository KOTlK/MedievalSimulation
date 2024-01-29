using System;
using System.Collections.Generic;
using UnityEngine;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.View.Rendering;
using static Simulation.Runtime.Entities.Mining;
using static Simulation.Runtime.Vars;
using Random = UnityEngine.Random;

namespace Simulation.Runtime.Entities
{
    public struct Cell
    {
        public Vector3 Position;
        public CellType Type;
        public CellContent Content;
        public bool ContainsContent;
        public int ContentEntity;
        public int Entity;
    }
    
    [Flags]
    public enum CellType : ulong
    {
        Soil            = 1 << 0,
        Rock            = 1 << 1,
        SmallWater      = 1 << 2,
        DeepWater       = 1 << 3,
    }

    public enum CellContent
    {
        Plants,
        ResourceDeposit
    }
    
    public struct WorldBounds
    {
        public Vector2 Max;
        public Vector2 Min;
    }

    public delegate CellType WorldGeneration(int x, int y);

    public static class GameWorld
    {
        public static Cell[] Cells;
        public static WorldBounds Bounds;

        private static Dictionary<int, int> _indexByEntity = new();

        public static void CreateWorld(Vector2Int size, WorldGeneration genFunc )
        {
            WorldSize = size;
            Cells = new Cell[size.x * size.y];

            for (var y = 0; y < size.y; ++y)
            {
                for (var x = 0; x < size.x; ++x)
                {
                    var position = new Vector3(x * LocalCellSize.x, y * LocalCellSize.y, 0);
                    var cellType = genFunc(x, y);
                    var entity = CreateEntity(new Entity
                    {
                        Position = position,
                        EntityType = EntityType.GroundCell,
                        Flags = EntityFlags.Static
                    });

                    var cell = new Cell()
                    {
                        Type = genFunc(x, y),
                        Entity = entity,
                        Position = position,
                        ContainsContent = false
                    };
                    
                    if (cellType == CellType.Rock)
                    {
                        //generate random resource deposit or no

                        var rand = Random.Range(0f, 1f);

                        if (rand <= ResourceDepositChance)
                        {
                            cell.ContainsContent = true;
                            cell.Content = CellContent.ResourceDeposit;
                            cell.ContentEntity = CreateRandomResourceDeposit(new Vector3Int(x, y, 0));
                        }
                    }

                    SetCell(x, y, cell);
                }
            }

            Bounds = new WorldBounds()
            {
                Max = new Vector2(size.x - 1 + LocalCellSize.x / 2f, size.y - 1 + LocalCellSize.y / 2f),
                Min = new Vector2(0 - LocalCellSize.x / 2f, 0 - LocalCellSize.y / 2f)
            };
        }

        public static CellType PerlinNoise(int x, int y)
        {
            var rand = Mathf.PerlinNoise(x * 0.1f + WorldSeed, y * 0.1f + WorldSeed);

            if (rand >= 0.6f)
            {
                return CellType.Rock;
            }
            else
            {
                return CellType.Soil;
            }
        }

        public static void FillCellContent(int x, int y, CellContent content, int contentEntity)
        {
            Cells[x + y * WorldSize.x].Content = content;
            Cells[x + y * WorldSize.x].ContainsContent = true;
            Cells[x + y * WorldSize.x].ContentEntity = contentEntity;
        }

        public static void RemoveCellContent(int x, int y)
        {
            Cells[x + y * WorldSize.x].ContainsContent = false;
            Cells[x + y * WorldSize.x].ContentEntity = -1;
        }

        public static Cell GetCell(int x, int y) => Cells[x + y * WorldSize.x];
        public static ref Cell GetCellReference(int x, int y) => ref Cells[x + y * WorldSize.x];
        public static void SetCell(int x, int y, Cell cell)
        {
            var index = x + y * WorldSize.x;

            _indexByEntity[cell.Entity] = index;
            Cells[x + y * WorldSize.x] = cell;
        }

        public static ref Cell GetCellByEntity(int entity) => ref Cells[_indexByEntity[entity]];
        public static int GetCellIdByEntity(int entity) => _indexByEntity[entity];

        public static bool IsCellFree(int x, int y) => Cells[x + y * WorldSize.x].ContainsContent == false;
        public static CellType GetCellType(int x, int y) => Cells[x + y * WorldSize.x].Type;
    }
}