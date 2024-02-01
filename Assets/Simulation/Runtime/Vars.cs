using UnityEngine;

namespace Simulation.Runtime
{
    public static class Vars
    {
        public static int DaysInMonth = 28;
        public static int MonthsInYear = 12;
        
        //World
        public static Vector2Int WorldSize = new (100, 100);
        public static Vector2Int LocalGridSize = new(100, 100);
        public static Vector2 LocalCellSize = new (1f, 1f);
        public static Vector3 WorldCellSize = new(1f, 1f);
        public static int WorldSeed = 87645;
        public static float ResourceDepositChance = 0.02f;
        public static int MinResourcesCount = 200;
        public static int MaxResourcesCount = 10000;
    }
}