using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractural.SceneManagement.Transitions
{
	public class FadeTransition : SceneTransition
	{
		[Export]
		public float TransitionInDuration { get; set; }
		[Export]
		public Curve TransitionInCurve { get; set; }

		[Export]
		public float TransitionOutDuration { get; set; }
		[Export]
		public Curve TransitionOutCurve { get; set; }

		public enum State
		{
			Idle,
			TransitionIn,
			TransitionOut,
		}
		public State TransitionState { get; set; } = State.Idle;

		[Export]
		private NodePath colorRectPath;
		private ColorRect colorRect;

		private float timer;

		public override void _Ready()
		{
			colorRect = GetNode<ColorRect>(colorRectPath);
		}

		public override void _Process(float delta)
		{
			if (TransitionState == State.TransitionIn)
			{
				var originalColor = colorRect.Color;
				originalColor.a = TransitionInCurve.Interpolate(timer / TransitionInDuration);
				colorRect.Color = originalColor;
				if (timer < TransitionInDuration)
					timer += delta;
				else
				{
					TransitionState = State.Idle;
					EmitSignal(nameof(OnTransitionedIn));
				}
			} else if (TransitionState == State.TransitionOut)
			{
				var originalColor = colorRect.Color;
				originalColor.a = TransitionOutCurve.Interpolate(timer / TransitionOutDuration);
				colorRect.Color = originalColor;
				if (timer < TransitionOutDuration)
					timer += delta;
				else
				{
					TransitionState = State.Idle;
					EmitSignal(nameof(OnTransitionedOut));
				}
			}
		}

		public override void TransitionIn()
		{
			timer = 0;
			TransitionState = State.TransitionIn;
		}

		public override void TransitionOut()
		{
			timer = 0;
			TransitionState = State.TransitionOut;
		}
	}
}
