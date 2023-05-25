using System;
using Godot;

namespace Godot
{
    public struct Vector2Int
    {
        public static Vector2Int Zero => new Vector2Int(0, 0);
        public static Vector2Int One => new Vector2Int(1, 1);
        public static Vector2Int NegOne => new Vector2Int(-1, -1);
        public static Vector2Int Up => new Vector2Int(0, -1);
        public static Vector2Int Down => new Vector2Int(0, 1);
        public static Vector2Int Left => new Vector2Int(-1, 0);
        public static Vector2Int Right => new Vector2Int(1, 0);

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
            return new Vector2Int((int)vector.x, (int)vector.y);
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2Int operator -(Vector2Int a)
        {
            return a * -1;
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return a + -b;
        }

        public static Vector2Int operator *(Vector2Int a, int b)
        {
            return new Vector2Int(a.X * b, a.Y * b);
        }

        public static Vector2Int operator *(Vector2Int a, float b)
        {
            return new Vector2Int(Mathf.RoundToInt(a.X * b), Mathf.RoundToInt(a.Y * b));
        }

        public static Vector2Int operator /(Vector2Int a, int b)
        {
            return new Vector2Int(a.X / b, a.Y / b);
        }

        public static Vector2Int operator /(Vector2Int a, float b)
        {
            if (b == 0f) throw new DivideByZeroException();
            return new Vector2Int(Mathf.RoundToInt(a.X / b), Mathf.RoundToInt(a.Y / b));
        }

        public static bool operator ==(Vector2Int a, Vector2Int b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2Int a, Vector2Int b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2Int otherVec && this == otherVec;
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}