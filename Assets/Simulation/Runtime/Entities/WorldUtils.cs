using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Simulation.Runtime.Game;
using UnityEngine;
using static Simulation.Runtime.Entities.EntityManager;
using static Simulation.Runtime.Entities.Mining;
using static Simulation.Runtime.Vars;
using Random = UnityEngine.Random;

namespace Simulation.Runtime.Entities
{
    public struct Cell
    {
        public Vector3 Position;
        public WorldCellType Type;
        public CellContent Content;
        public bool ContainsContent;
        public int ContentEntity;
    }
    
    public struct LocalGrid
    {
        public GridBounds Bounds;
        public Vector2Int WorldPosition;
        public WorldCellType CellType;
        public Entity[] EntitiesGrid;
        public Plant[] Plants;
        public ResourceDeposit[] Resources;
        public Farmer[] Farmers;
        public int FarmersCount;
        public int ResourcesCount;
        public int PlantsCount;
        public Cell[] Cells;
        public int Id;
        //there will be more descriptions for cells generation, but not now
    }
    
    public struct WorldGrid
    {
        public LocalGrid[] Locations;
        public GridBounds Bounds;
    }
    
    [Flags]
    public enum WorldCellType : ulong
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
    
    public struct GridBounds
    {
        public Vector2 Max;
        public Vector2 Min;
    }

    public delegate WorldCellType WorldGeneration(int x, int y);

    public static class WorldUtils
    {
        public static void CreateWorld(ref GameState state, Vector2Int size, WorldGeneration genFunc )
        {
            WorldSize = size;

            var world = new WorldGrid()
            {
                Locations = new LocalGrid[size.x * size.y],
                Bounds = new GridBounds()
                {
                    Max = new Vector2(size.x - 1 + WorldCellSize.x / 2f, size.y - 1 + WorldCellSize.y / 2f),
                    Min = new Vector2(0 - WorldCellSize.x / 2f, 0 - WorldCellSize.y / 2f)
                }
            };
            
            for (var y = 0; y < size.y; ++y)
            {
                for (var x = 0; x < size.x; ++x)
                {
                    var position = new Vector2Int(x, y);
                    var cellType = genFunc(x, y);
                    var index = x + y * size.x;

                    var grid = new LocalGrid()
                    {
                        Bounds = new GridBounds()
                        {
                            Max = new Vector2(size.x - 1 + LocalCellSize.x / 2f, size.y - 1 + LocalCellSize.y / 2f),
                            Min = new Vector2(0 - LocalCellSize.x / 2f, 0 - LocalCellSize.y / 2f)
                        },
                        Cells = new Cell[LocalGridSize.x * LocalGridSize.y],
                        Plants = new Plant[10],
                        PlantsCount = 0,
                        Resources = new ResourceDeposit[10],
                        ResourcesCount = 0,
                        Farmers = new Farmer[10],
                        FarmersCount = 0,
                        CellType = cellType,
                        EntitiesGrid = new Entity[100],
                        Id = index,
                        WorldPosition = position
                    };

                    FillLocalGrid(ref grid, genFunc);

                    world.Locations[index] = grid;
                }
            }

            state.WorldGrid = world;
        }

        public static void FillLocalGrid(ref LocalGrid grid, WorldGeneration genFunc)
        {
            for (var y = 0; y < LocalGridSize.y; ++y)
            {
                for (var x = 0; x < LocalGridSize.x; ++x)
                {
                    var cellType = genFunc(grid.WorldPosition.x * x, grid.WorldPosition.y * y);
                    var cell = new Cell
                    {
                        Type = cellType,
                        Position = new Vector3(x, y, 0),
                        ContainsContent = false
                    };
                    
                    if (cellType == WorldCellType.Rock)
                    {
                        //generate random resource deposit or no

                        var rand = Random.Range(0f, 1f);

                        if (rand <= ResourceDepositChance)
                        {
                            cell.ContainsContent = true;
                            cell.Content = CellContent.ResourceDeposit;
                            cell.ContentEntity = CreateRandomResourceDeposit(ref grid,new Vector3Int(x, y, 0));
                        }
                    }

                    grid.Cells[x + y * LocalGridSize.x] = cell;
                }
            }
        }

        public static WorldCellType PerlinNoise(int x, int y)
        {
            var rand = Mathf.PerlinNoise(x * 0.1f + WorldSeed, y * 0.1f + WorldSeed);

            if (rand >= 0.6f)
            {
                return WorldCellType.Rock;
            }
            else
            {
                return WorldCellType.Soil;
            }
        }

        public static void FillCellContent(ref LocalGrid grid, Vector3Int position, CellContent content, int contentEntity)
        {
            grid.Cells[position.x + position.y * LocalGridSize.x].Content = content;
            grid.Cells[position.x + position.y * LocalGridSize.x].ContentEntity = contentEntity;
            grid.Cells[position.x + position.y * LocalGridSize.x].ContainsContent = true;
        }

        public static void RemoveCellContent(ref LocalGrid grid, Vector3Int position)
        {
            grid.Cells[position.x + position.y * LocalGridSize.x].ContainsContent = false;
            grid.Cells[position.x + position.y * LocalGridSize.x].ContentEntity = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref LocalGrid GetLocalGrid(ref WorldGrid worldGrid, int x, int y) =>
            ref worldGrid.Locations[x + y * WorldSize.x];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static WorldCellType GetWorldCellType(ref WorldGrid grid, int x, int y) =>
            grid.Locations[x + y * WorldSize.x].CellType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Cell GetCell(ref LocalGrid localGrid, int x, int y) => localGrid.Cells[x + y * LocalGridSize.x];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Cell GetCellReference(ref LocalGrid localGrid, int x, int y) => ref localGrid.Cells[x + y * WorldSize.x];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCellFree(ref LocalGrid localGrid, int x, int y) => localGrid.Cells[x + y * WorldSize.x].ContainsContent == false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static WorldCellType GetCellType(ref LocalGrid localGrid, int x, int y) => localGrid.Cells[x + y * WorldSize.x].Type;
    }
}