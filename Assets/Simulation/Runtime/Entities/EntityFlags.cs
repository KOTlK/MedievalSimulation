using System;

namespace Simulation.Runtime.Entities
{
    [Flags]
    public enum EntityFlags : ulong
    {
        Tickable = 1 << 0,
        Farmer = 1 << 1,
        WorldCell = 1 << 2,
    }
}