using Godot;

/// <summary>
/// Utilities used by Fractural Studios.
/// </summary>
namespace Fractural.Utils
{
	public static class DrawUtils
	{
		/// <summary>
		/// Draws lines that connects one point to the next, with the last point connecting back to the first point. 
		/// This is workaround for <see cref="CanvasItem.DrawPolyline"/> not handling corners correct (It distorts them in a weird way).
		/// </summary>
		/// <param name="item">The drawer</param>
		/// <param name="points">Points to connect</param>
		/// <param name="color">Color of the segments</param>
		/// <param name="width">Width of the segments</param>
		/// <param name="antialiased">Antialias toggle</param>
		public static void DrawSegmentedPolyline(this CanvasItem item, Vector2[] points, Color color, float width = 1, bool antialiased = false)
		{
			if (points == null || points.Length == 0) return;

			var previousPoint = points[points.Length - 1];
			foreach (var point in points)
			{
				item.DrawLine(previousPoint, point, color, width, antialiased);
				previousPoint = point;
			}
		}

		/// <summary>
		/// Draws lines that connects one point to the next, with the last point connecting back to the first point. 
		/// This is workaround for <see cref="CanvasItem.DrawPolyline"/> not handling corners correct (It distorts them in a weird way).
		/// </summary>
		/// <param name="item">The drawer</param>
		/// <param name="points">Points to connect</param>
		/// <param name="color">Colors of each segment</param>
		/// <param name="width">Width of all segments</param>
		/// <param name="antialiased">Antialias toggle</param>
		public static void DrawSegmentedPolyline(this CanvasItem item, Vector2[] points, Color[] color, float width = 1, bool antialiased = false)
		{
			if (points == null || points.Length == 0) return;
			if (color.Length != points.Length)
			{
				GD.PrintErr("DrawSegmentedPolyline: Expected color array to have the same length as the points array!");
				return;
			}

			var previousPoint = points[points.Length - 1];
			int i = 0;
			foreach (var point in points)
			{
				item.DrawLine(previousPoint, point, color[i++], width, antialiased);
				previousPoint = point;
			}
		}
	}
}