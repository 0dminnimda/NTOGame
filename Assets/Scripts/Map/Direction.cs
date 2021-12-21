using System.Collections.Generic;

namespace Map
{
    public enum Direction
    {
        None,
        N,
        E,
        S,
        W
    }

    public static class DirectionExt
    {
        public static Direction GetOpposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.N:
                    return Direction.S;
                case Direction.E:
                    return Direction.W;
                case Direction.S:
                    return Direction.N;
                case Direction.W:
                    return Direction.E;
                default:
                    throw new System.Exception(
                        "Invalid Direction: " +
                        direction.ToString());
            }
        }

        public static int GetAngle(this Direction direction)
        {
            return ((int) direction) * 90;
        }

        public static int DirectionArrayHash(IEnumerable<Direction> arr)
        {
            int hash = 17;
            foreach (int i in arr)
            {
                unchecked // Overflow is fine, just wrap
                {
                    hash *= (3 + i).GetHashCode();
                }
            }
            
            return hash;
        }
    }
}
