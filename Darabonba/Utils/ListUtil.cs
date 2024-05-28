using System;
using System.Collections.Generic;
using Darabonba.Exceptions;

namespace Darabonba.Utils
{
    public class ListUtil
    {
        public static T Shift<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new DaraException
                {
                    Message = "array is empty"
                };
            }
            T first = list[0];
            list.RemoveAt(0);
            return first;
        }

        public static int Unshift<T>(List<T> array, T data)
        {
            array.Insert(0, data);
            return array.Count;
        }


        public static int Push<T>(List<T> array, T data)
        {
            array.Add(data);
            return array.Count;
        }

        public static T Pop<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new DaraException
                {
                    Message = "array is empty"
                };
            }
            T last = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return last;
        }

        public static List<T> Sort<T>(List<T> array, string order) where T : IComparable<T>
        {
            if (order == "asc")
            {
                array.Sort();
            }
            else if (order == "desc")
            {
                array.Sort((x, y) => y.CompareTo(x));
            }
            return array;
        }

        public static List<T> Concat<T>(List<T> array1, List<T> array2)
        {
            array1.AddRange(array2);
            return array1;
        }
    }
}