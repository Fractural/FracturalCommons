using Godot;

namespace Fractural.Utils
{
	public static class VectorUtils
	{
		public static float DistanceToLineSegment(this Vector2 point, Vector2 lineStart, Vector2 lineEnd)
			=> Mathf.Sqrt(DistanceToLineSegmentSquared(point, lineStart, lineEnd));

		public static float DistanceToLineSegmentSquared(this Vector2 point, Vector2 lineStart, Vector2 lineEnd)
		{
			float l2 = lineStart.DistanceSquaredTo(lineEnd);
			if (l2 == 0) return lineStart.DistanceSquaredTo(point);

			float t = Mathf.Max(0, Mathf.Min(1, (point - lineStart).Dot(lineEnd - lineStart) / l2));
			Vector2 projection = lineStart + t * (lineEnd - lineStart);

			return point.DistanceSquaredTo(projection);
		}
	}
}