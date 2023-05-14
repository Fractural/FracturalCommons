using Godot;
using System;

#if TOOLS
namespace Fractural.Plugin
{
    [CSharpScript]
    [Tool]
    public class ForwardDragDropCheckButton : CheckButton
    {
        public Func<Vector2, object, bool> CanDropDataFunc { get; set; }
        public Action<Vector2, object> DropDataFunc { get; set; }

        public override bool CanDropData(Vector2 position, object data)
        {
            if (CanDropDataFunc != null)
                return CanDropDataFunc(position, data);
            return base.CanDropData(position, data);
        }

        public override void DropData(Vector2 position, object data)
        {
            if (DropDataFunc != null)
                DropDataFunc(position, data);
        }
    }
}
#endif