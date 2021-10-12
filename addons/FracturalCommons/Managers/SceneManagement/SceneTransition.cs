using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Fractural.SceneManagement
{
	public abstract class SceneTransition : Node
	{
		[Signal]
		public delegate void OnTransitionedIn();
		[Signal]
		public delegate void OnTransitionedOut();

		public abstract void TransitionIn();
		public abstract void TransitionOut();
	}
}
