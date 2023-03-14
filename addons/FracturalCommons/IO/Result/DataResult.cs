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
    public class DataResult<T> : Result
    {
        private bool hasData = false;
        public override bool HasData => hasData;

        private T data = default!;
        public void ClearData() { data = default!; }

        public T Data
        {
            get => hasData ? data : throw new System.Exception("Result does not contain data. Check HasData first.");
            set
            {
                hasData = true;
                data = value;
            }
        }

        public bool TryGetData(out T data) { data = Data; return hasData; }
        public bool TryGetData<U>(out U data)
        {
            if (hasData && Data is U u) { data = u; return true; }
            else { data = default!; return false; }
        }


        protected DataResult() { } //Default constructor for Godot

        /// Provide an operation description that fits the form "<Operation> succeeded"
        public DataResult(string operationDescription)
          : base(operationDescription)
        { }
        /// Provide an operation description that fits the form "<Operation> succeeded"
        public DataResult(string operationDescription, Error error, string errorMessage = "")
          : base(operationDescription, error, errorMessage)
        { }
        public DataResult(Result other, string errorMessage = "")
          : base(other, errorMessage)
        { }
        /// Provide an operation description that fits the form "<Operation> succeeded"
        public DataResult(string operationDescription, Result other, string errorMessage = "")
          : base(operationDescription, other, errorMessage)
        { }
        /// Provide an operation description that fits the form "<Operation> succeeded"
        public DataResult(string operationDescription, T data)
          : base(operationDescription)
        {
            Data = data;
        }
    };
}
