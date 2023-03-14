using System.Text;
using Godot;
using Godot.Collections;

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
    /// Aggregation of subresults using key strings
    /// Any ErrorResultMap subresults get squashed into a Result on add and all its
    /// supresults are copied over.
    /// The keys of those subresults are not changed, so they should be unique.
    [Tool]
    public class BatchResult : Result
    {
        public Dictionary<string, Result> errors = new Dictionary<string, Result>();
        public void Clear() { errors = new Dictionary<string, Result>(); }


        /// Add the subresult for the given key. Does not add if the subresult provided is OK.
        public void Add(string key, Result subresult)
        {
            if (subresult.IsOk) { return; }
            if (Error == Error.Ok) { SetError(Error.Failed); }
            if (!errors.ContainsKey(key)) { errors[key] = subresult; }
        }
        /// Adds a sliced copy of the subresult for the given key. Does not add if the subresult provided is OK.
        public void Add<TResult>(string key, TResult subresult) where TResult : Result { Add(key, new Result(subresult)); }

        /// Adds a slice of the ErrorResultMap. Also adds all errors within.
        /// Does not add if the subresult provided is OK
        public void Merge(BatchResult other, string rootErrorKey)
        {
            if (other.IsOk) { return; }
            if (Error == Error.Ok) { SetError(Error.Failed); }
            // Slice the other batch result down to just it's error and place it at the root key
            if (!errors.ContainsKey(rootErrorKey)) { errors[rootErrorKey] = new Result(other); }
            // Copy over each result found in subresult
            foreach (var kv in other.errors)
            {
                if (!errors.ContainsKey(kv.Key)) { errors[kv.Key] = kv.Value; }
            }
        }

        // Removes an error. Does not change overall fail state. Use from_<> afterward to try to set to OK.
        public bool Remove(string key) => errors.Remove(key);


        // When errors is empty, returns the base Result._to_string() result
        // When errors is not empty prints
        // Returns "<>" when error is not OK
        //   Leaves off the <error_message> section if empty.
        //   Leaves off the <error> section if error is just FAILED
        public override string ToString()
        {
            var msg = base.ToString();
            if (errors.Count == 0) { return msg; }

            var builder = new StringBuilder(msg);
            builder.Append(":");
            foreach (var error in errors.Values)
            {
                foreach (var line in error.ToString().Split("\n", false))
                {
                    builder.Append("\n  ").Append(line);
                }
            }
            return builder.ToString();
        }


        protected BatchResult() { } //Default constructor for Godot

        /// Provide an operation description that fits the form "<Operation> succeeded"
        public BatchResult(string operationDescription) : base(operationDescription) { }
        // Provide an operation description that fits the form "<Operation> succeeded"
        public BatchResult(string operationDescription, Error error, string errorMessage = "")
          : base(operationDescription, error, errorMessage)
        { }
        public BatchResult(Result other, string errorMessage = "")
          : base(other, errorMessage)
        { }
        // Provide an operation description that fits the form "<Operation> succeeded"
        public BatchResult(string operationDescription, Result other, string errorMessage = "")
          : base(operationDescription, other, errorMessage)
        { }


        // Ensures failure if errors were added.
        public override void SetError(Error error, string errorMessage = "")
        {
            base.SetError(error, errorMessage);
            if (IsOk && errors.Count > 0) { SetError(Error.Failed); }
        }

        // Ensures failure if errors were added.
        public override void SetError(Result other, string errorMessage = "")
        {
            base.SetError(other, errorMessage);
            if (IsOk && errors.Count > 0) { SetError(Error.Failed); }
        }
    };
}
