using UnityEngine;

namespace Simulation.Runtime.Entities
{
    public struct Entity
    {
        public Vector3     Position;
        public EntityFlags Flags;
        public int         Id;
        public float       Orientation;
        public bool        IsAlive;
    }
}