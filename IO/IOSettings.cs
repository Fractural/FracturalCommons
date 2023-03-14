using Godot;

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

/// <summary>
/// IO Tools is from cgbeutler's github repository
/// https://github.com/cgbeutler/com.monstervial.io_tools
/// </summary>
namespace Fractural.IO
{
    /// <summary> Static settings used by the IO Tools addon </summary>
    public static class IOSettings
    {
        /// Allows '_Ready' to be called on resources automatically.
        /// Called after using CSharpScript<CustRes>.New() and after IO.Load<CustRes>.
        /// Default Load will call constructors, then fill all values, then call _Ready.
        /// This allows a sort of late-init that triggers post-load.
        public const bool CallReadyOnResources = true;

        /// Location of the IO.cs file relative to the project folder.
        public const string IoScriptProjectPath = "addons/FracturalCommons/IO/IO.cs";
    };
}
