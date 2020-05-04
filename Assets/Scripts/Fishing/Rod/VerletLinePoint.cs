using UnityEngine;

namespace Fishing.Rod
{
    public struct VerletLinePoint
    {
        public Vector3 Pos{get; set;}
        public Vector3 OldPos {get; set;}
        public Vector3 Acceleration {get;}

        public VerletLinePoint(Vector3 position, Vector3 acceleration)
        {
            Pos = position;
            Acceleration = acceleration;
            OldPos = default;
        }
    }
}