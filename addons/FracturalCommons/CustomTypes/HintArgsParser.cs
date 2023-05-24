namespace Fractural
{
    /// <summary>
    /// Used by InspectorPlugins to parse custom Godot property hint text. These plugins often use hint strings from the <see cref="HintString"/> class.
    /// </summary>
    public class HintArgsParser
    {
        private string[] _argsArray;
        private string _argsString;

        public HintArgsParser(string argsString)
        {
            _argsString = argsString;
            _argsArray = argsString.Split(',');
        }

        public bool TryGetArgs(string name)
        {
            for (int i = 0; i < _argsArray.Length; i++)
                if (_argsArray[i] == name)
                    return true;
            return false;
        }

        public bool TryGetArgs(string name, int argCount, out string[] args)
        {
            args = new string[argCount];
            for (int i = 0; i < _argsArray.Length; i++)
                if (_argsArray[i] == name && i + argCount < _argsArray.Length)
                {
                    for (int j = 0; j < argCount; j++)
                        args[j] = _argsArray[i + 1 + j];
                    return true;
                }
            return false;
        }

        public bool TryGetArgs(string name, out string arg1)
        {
            arg1 = null;
            for (int i = 0; i < _argsArray.Length; i++)
                if (_argsArray[i] == name && i + 1 < _argsArray.Length)
                {
                    arg1 = _argsArray[i + 1];
                    return true;
                }
            return false;
        }

        public bool TryGetArgs(string name, out string arg1, out string arg2)
        {
            arg1 = null;
            arg2 = null;
            for (int i = 0; i < _argsArray.Length; i++)
                if (_argsArray[i] == name && i + 2 < _argsArray.Length)
                {
                    arg1 = _argsArray[i + 1];
                    arg2 = _argsArray[i + 2];
                    return true;
                }
            return false;
        }

        public bool TryGetArgs(string name, out string arg1, out string arg2, out string arg3)
        {
            arg1 = null;
            arg2 = null;
            arg3 = null;
            for (int i = 0; i < _argsArray.Length; i++)
                if (_argsArray[i] == name && i + 3 < _argsArray.Length)
                {
                    arg1 = _argsArray[i + 1];
                    arg2 = _argsArray[i + 2];
                    arg3 = _argsArray[i + 3];
                    return true;
                }
            return false;
        }

        public bool TryGetArgs(string name, out string arg1, out string arg2, out string arg3, out string arg4)
        {
            arg1 = null;
            arg2 = null;
            arg3 = null;
            arg4 = null;
            for (int i = 0; i < _argsArray.Length; i++)
                if (_argsArray[i] == name && i + 4 < _argsArray.Length)
                {
                    arg1 = _argsArray[i + 1];
                    arg2 = _argsArray[i + 2];
                    arg3 = _argsArray[i + 3];
                    arg4 = _argsArray[i + 4];
                    return true;
                }
            return false;
        }
    }
}
