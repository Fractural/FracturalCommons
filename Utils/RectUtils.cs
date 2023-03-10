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
	}
}