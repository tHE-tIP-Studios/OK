using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class MonoBehaviourUtils
    {
        // Idea curtesy of Tomas Franco.
        /// <summary>
        /// Gets a component using a recursive strategy to get it from a child of child
        /// </summary>
        /// <param name="parent"> the object being called from</param>
        /// <typeparam name="T"> The type of component to get</typeparam>
        /// <returns> The component of generic type, returns null if none was found</returns>
        public static T GetComponentRecursive<T>(this Transform parent)
        {
            if (parent.transform.childCount != 0)
            {
                return GetComponentRecursive<T>(parent.GetChild(0));
            }

            return parent.GetComponent<T>();
        }

        public static T GetComponentRecursive<T>(this Transform parent, int stopAt, int currentIndex = 0)
        {
            if (parent.transform.childCount != 0 && currentIndex < stopAt)
            {
                return GetComponentRecursive<T>(parent.GetChild(0), stopAt, currentIndex++);
            }

            return parent.GetComponent<T>();
        }
    }
}
