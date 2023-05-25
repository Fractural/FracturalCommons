namespace Fractural
{
    /// <summary>
    /// Global partial class used to store HintStrings for Export properties to allow them to be handled by custom InspectorPlugins.
    /// </summary>
    public partial class HintString
    {
        /// <summary>
        /// Usage: TypedDictionary,[KeyType]:[ValueType]
        /// 
        /// KeyType:
        /// - float
        /// - int
        /// - bool
        /// - string
        /// - NodePath
        /// 
        /// ValueType:
        /// - float
        /// - int
        /// - bool
        /// - string
        /// - NodePath
        /// </summary>
        public static string TypedDictionary<TKey, TValue>()
        {
            return $"{nameof(TypedDictionary)},{typeof(TKey).Name}:{typeof(TValue).Name}";
        }

    }
}