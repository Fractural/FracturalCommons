﻿using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GDC = Godot.Collections;

namespace Fractural.Utils
{
    public static class GDCollectionUtils
    {
        public static int FindIndex<T>(this GDC.Array<T> array, System.Predicate<T> predicate)
        {
            for (int i = 0; i < array.Count; i++)
                if (predicate(array[i]))
                    return i;
            return -1;
        }

        public static void ForEach<T>(this GDC.Array<T> array, Action<T> action)
        {
            foreach (var element in array)
                action(element);
        }

        public static void AddRange<T>(this GDC.Array<T> array, IEnumerable<T> enumerable)
        {
            foreach (var element in enumerable)
                array.Add(element);
        }

        public static T[] ToArray<T>(this GDC.Array<T> array)
        {
            var csharpArray = new T[array.Count];
            for (int i = 0; i < array.Count; i++)
                csharpArray[i] = array[i];
            return csharpArray;
        }

        public static int FindIndex(this GDC.Array array, System.Predicate<object> predicate)
        {
            for (int i = 0; i < array.Count; i++)
                if (predicate(array[i]))
                    return i;
            return -1;
        }

        public static void ForEach(this GDC.Array array, Action<object> action)
        {
            foreach (var element in array)
                action(element);
        }

        public static void AddRange(this GDC.Array array, IEnumerable enumerable)
        {
            foreach (var element in enumerable)
                array.Add(element);
        }

        public static T[] ToArray<T>(this GDC.Array array)
        {
            var csharpArray = new T[array.Count];
            for (int i = 0; i < array.Count; i++)
                csharpArray[i] = (T)array[i];
            return csharpArray;
        }

        /// <summary>
        /// Gets a value from a <paramref name="dictionary"/> using <paramref name="key"/>. If the value does 
        /// not exist, <paramref name="defaultReturn"/> is returned instead. This method supports dot syntax,
        /// so you can use a key of "path.to.value" to fetch a value from nested dictionaries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultReturn"></param>
        /// <returns></returns>
        public static T Get<T>(this GDC.Dictionary dictionary, string key, T defaultReturn = default)
        {
            var keys = key.Split(".");
            for (int i = 0; i < keys.Length; i++)
            {
                if (i == keys.Length - 1)
                {
                    if (dictionary.Contains(keys[i]))
                        return (T)dictionary[keys[i]];
                    return defaultReturn;
                }
                dictionary = dictionary.Get<GDC.Dictionary>(keys[i]);
                if (dictionary == null)
                    return defaultReturn;
            }
            return defaultReturn;
        }

        public static GDC.Dictionary ToGDDict(this object obj)
        {
            if (obj == null)
                return null;
            GDC.Dictionary dict = new GDC.Dictionary();
            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                dict[prop.Name] = prop.GetValue(obj, null);
            }
            return dict;
        }

        public static GDC.Array<T> ToGDArray<T>(this IEnumerable<T> array)
        {
            if (array == null)
                return null;
            var gdArray = new GDC.Array<T>();
            foreach (var elem in array)
                gdArray.Add(elem);
            return gdArray;
        }

        public static GDC.Array ToRawGDArray(this IEnumerable array)
        {
            if (array == null)
                return null;
            var gdArray = new GDC.Array();
            foreach (var elem in array)
                gdArray.Add(elem);
            return gdArray;
        }


        /// <summary>
        /// Returns either a GDC.Array or GDC.Dictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object RecurseToGDCollection(this object obj)
        {
            if (obj == null)
                return null;
            if (obj is IEnumerable<object> enumerable)
                return RecurseToRawGDArray(enumerable);
            return RecurseToGDDict(obj);
        }

        public static GDC.Array RecurseToRawGDArray(this IEnumerable array)
        {
            if (array == null)
                return null;
            var gdArray = new GDC.Array();
            foreach (var elem in array)
            {
                if (elem is IEnumerable enumerableElem)
                    gdArray.Add(enumerableElem.RecurseToRawGDArray());
                else if (Type.GetTypeCode(elem.GetType()) == TypeCode.Object)
                    gdArray.Add(elem.ToGDDict());
            }
            return gdArray;
        }

        public static GDC.Dictionary RecurseToGDDict(this object obj)
        {
            if (obj == null)
                return null;
            GDC.Dictionary dict = new GDC.Dictionary();
            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var propValue = prop.GetValue(obj, null);
                if (propValue == null)
                    dict[prop.Name] = null;
                else
                {
                    if (prop.PropertyType == typeof(IEnumerable))
                        dict[prop.Name] = ((IEnumerable)propValue).RecurseToRawGDArray();
                    else if (Type.GetTypeCode(prop.PropertyType) == TypeCode.Object)
                        dict[prop.Name] = propValue.ToGDDict();
                }
            }
            return dict;
        }
    }
}