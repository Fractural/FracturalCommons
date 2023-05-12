/*
 
MIT License

Copyright (c) 2022 Cory Beutler

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 
*/

namespace Godot
{
    /// <summary> A WeakRef class that is typed and can take Godot and CSharp objects safely. </summary>
    public class WeakRef<T> // : Reference
      where T : class
    {
        private static readonly bool useGodotWeakPtr = typeof(Godot.Object).IsAssignableFrom(typeof(T));

        private Godot.WeakRef gdWeak;
        private System.WeakReference<T> stdWeak;

        /// <summary> Create a new WeakRef with no target. </summary>
        public WeakRef() { }
        /// <summary> Create a new WeakRef pointing to the target provided. </summary>
        public WeakRef(T target) { if (target != null) { SetTarget(target); } }
        /// <summary> Implicit conversion CSharp weak reference to this safer version. </summary>
        public static implicit operator WeakRef<T>(System.WeakReference<T> weak)
          => new WeakRef<T>(weak.TryGetTarget(out var strong) ? strong : null);
        /// <summary> Implicit conversion from Godot weak reference to this typed version. Throws on type mismatch. </summary>
        public static implicit operator WeakRef<T>(Godot.WeakRef weak)
        {
            var strong = weak.GetRef();
            if (strong == null) { return new WeakRef<T>(); }
            if (strong is T t) { return new WeakRef<T>(t); }
            throw new System.ArgumentException($"Unable to cast type '{strong.GetType().Name}' to type '{typeof(T).Name}'", nameof(weak));
        }

        /// <summary> Sets the target object that is referenced. </summary>
        /// <param name="target"> The new target object. </param>
        public void SetTarget(T target)
        {
            if (target == null)
            {
                gdWeak = null;
                stdWeak = null;
            }
            else if (useGodotWeakPtr)
            {
                if (!(target is Godot.Object gdObj)) { Godot.GD.PrintErr("Failed to convert target to gd object."); return; }
                gdWeak = Godot.Object.WeakRef(gdObj);
            }
            else
            {
                stdWeak = new System.WeakReference<T>(target);
            }
        }

        /// <summary> Tries to get the target reference object. </summary>
        /// <returns> The reference object or null if it is dead or unset. </returns>
        public T GetTargetOrNull()
        {
            if (useGodotWeakPtr)
            {
                if (gdWeak != null) { return gdWeak.GetRef() as T; }
            }
            else
            {
                if (stdWeak != null && stdWeak.TryGetTarget(out var strong)) { return strong; }
            }
            return null;
        }

        /// <summary> Tries to get the target reference object. </summary>
        /// <param name="strong">
        ///   When this method returns, contains the target object, if it is available. This
        ///   parameter is treated as uninitialized.
        /// </param>
        /// <returns> true if the target was retrieved; otherwise, false. </returns>
        public bool TryGetTarget(out T strong)
        {
            strong = null; // Try pattern, null ok
            if (useGodotWeakPtr)
            {
                if (gdWeak != null && gdWeak.GetRef() is T t) { strong = t; return true; }
            }
            else
            {
                if (stdWeak != null) { return stdWeak.TryGetTarget(out strong); }
            }
            return false;
        }
    };
}
