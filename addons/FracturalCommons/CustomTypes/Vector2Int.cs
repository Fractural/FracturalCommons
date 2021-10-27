using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Fractural
{
    public struct Vector2Int
    {
        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public static implicit operator Vector2(Vector2Int vector)
        {
            return new Vector2(vector.X, vector.Y);
		}

        public static implicit operator Vector2Int(Vector2 vector)
        {
            return new Vector2Int((int) vector.x, (int) vector.y);
		}
    }
}