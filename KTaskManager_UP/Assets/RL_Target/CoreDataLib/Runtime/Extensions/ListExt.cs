using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreDataLib
{
    /// <summary>
    /// Certain extension methods for regular dotnet List<T>.
    /// Uses reference equal instead of == operator for performance reasons
    /// Ref: https://blog.unity.com/technology/custom-operator-should-we-keep-it
    /// </summary>
    public static class ListExt
    {
        /// <summary>
        /// Removes duplicate entries and return the unique ones in a list while maintaining input sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listPoints"></param>
        /// <returns></returns>
        public static List<T> RemoveDuplicates<T>(this List<T> listPoints)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < listPoints.Count; i++)
            {
                if (!result.Contains(listPoints[i]))
                    result.Add(listPoints[i]);
            }
            return result;
        }

        
    }
}