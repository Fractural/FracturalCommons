using Godot;

namespace Fractural.Utils
{
    public static class RectUtils
    {
        public static Rect2 FromStartEnd(Vector2 start, Vector2 end)
        {
            return new Rect2(
                new Vector2(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y)),
                new Vector2(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y))
            );
        }

        public static Rect2 FromCenterExtents(Vector2 center, Vector2 extents)
        {
            return new Rect2(
                center - extents,
                extents * 2
            );
        }

        public static Rect2 FromCenterSize(Vector2 center, Vector2 size)
        {
            return new Rect2(
                center - size / 2f,
                size
            );
        }

        public static Rect2 AddPadding(this Rect2 rect, float padding)
            => AddPadding(rect, Vector2.One * padding);

        public static Rect2 AddPadding(this Rect2 rect, Vector2 padding)
        {
            rect.Position -= padding;
            rect.Size += padding * 2f;
            return rect;
        }

        public static Rect2 SetCenter(this Rect2 rect, Vector2 center)
        {
            rect.Position = center - rect.Size / 2;
            return rect;
        }
    }
}