using UnityEngine;

namespace Map
{
    public struct Coordinates
    {
        public int x;
        public int y;

        public Coordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }

        // public Vector2 ToVector2()
        // {
        //     return new Vector2(x, y);
        // }

        public Vector3 ToVector3()
        {
            return new Vector3(x, 0, y);
        }

        /// <summary>
        /// Returns the coordinate for an adjacent area based on the supplied direction.
        /// </summary>
        /// <param name="direction">
        /// The direction the adjacent coordinate is at.
        /// </param>
        /// <returns>
        /// A new set of coordinates for that are adjacent to this coordinate.
        /// </returns>
        public Coordinates GetAdjacentCoordinate(Direction direction)
        {
            int x = this.x;
            int y = this.y;
            switch (direction)
            {
                case Direction.N:
                    y += 1;
                    break;
                case Direction.E:
                    x += 1;
                    break;
                case Direction.S:
                    y -= 1;
                    break;
                case Direction.W:
                    x -= 1;
                    break;
                default:
                    break;
            }

            return new Coordinates(x, y);
        }
    }
}
