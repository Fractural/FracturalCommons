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

namespace Fractural.Results
{
    [Tool]
    public class JsonParseResult : DataResult<object>
    {
        public JsonParseResult FromJsonResult(JSONParseResult jsonResult)
        {
            SetError(Error.Failed, $"Line { jsonResult.ErrorLine }:  { jsonResult.ErrorString }");
            if (jsonResult.Error == Error.Ok) { Data = jsonResult.Result; }
            return this;
        }

        protected JsonParseResult() { } //Default constructor for Godot
                                        // See 'from_json_result'
        public JsonParseResult(JSONParseResult jsonResult = null!) : base("Parse JSON string")
        {
            if (jsonResult != null) { FromJsonResult(jsonResult); }
        }
    };
}
