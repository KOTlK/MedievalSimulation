using Simulation.Runtime.Entities;
using UnityEngine;
using static Simulation.Runtime.Entities.WorldUtils;
using static Simulation.Runtime.Vars;
using static Simulation.Runtime.Entities.Farming;
using static Simulation.Runtime.View.Rendering;

namespace Simulation.Runtime.Game
{
    public struct GameState
    {
        public int CurrentVisibleLocationId;
        public WorldGrid WorldGrid;
    }

    public static class GameUtils
    {
        public static GameState CurrentState;

        public static void InitializeGame()
        {
            CurrentState = new GameState();
            CreateWorld(ref CurrentState, WorldSize, PerlinNoise);
        }

        public static void UpdateInput()
        {
            
        }

        public static void ExecuteLogic()
        {
            ExecuteLocalGrids();
        }

        public static void Render()
        {
        }

        private static void ExecuteLocalGrids()
        {
            for (var i = 0; i < CurrentState.WorldGrid.Locations.Length; ++i)
            {
                TickPlants(CurrentState.WorldGrid.Locations[i].Plants, CurrentState.WorldGrid.Locations[i].PlantsCount);
                DrawFarmers(CurrentState.WorldGrid.Locations[i].Farmers, CurrentState.WorldGrid.Locations[i].FarmersCount);
            }
        }
    }
}