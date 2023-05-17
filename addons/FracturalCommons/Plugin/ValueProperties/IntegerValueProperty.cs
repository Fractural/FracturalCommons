using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class IntegerValueProperty : ValueProperty<int>
    {
        private SpinBox _spinBox;

        public IntegerValueProperty()
        {
            // TODO: Finish integer, float, bool, and string value properties
        }
    }
}
#endif