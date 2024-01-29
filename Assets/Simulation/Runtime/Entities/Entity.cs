using System;
using UnityEngine;

namespace Simulation.Runtime.Entities
{
    [Flags]
    public enum EntityFlags : ulong
    {
        Tickable = 1 << 0,
        Static   = 1 << 1
    }

    [Flags]
    public enum EntityType
    {
        GroundCell,
        Unit,
        Resource,
    }
    
    
    public struct Entity
    {
        public Vector3     Position;
        public EntityFlags Flags;
        public EntityType  EntityType;
        public int         Id;
        public float       Orientation;
        public bool        IsAlive;
    }
}