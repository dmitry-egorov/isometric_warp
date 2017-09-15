using System.Collections.Generic;
using UnityEngine;

namespace WarpSpace.Settings
{
    public static class ResourcesLoader
    {
        private static bool resources_are_loaded;
        
        public static IReadOnlyList<T> Load<T>() where T : ScriptableObject
        {
            if (!resources_are_loaded)
            {
                Resources.LoadAll("");
                resources_are_loaded = true;
            }
            
            return Resources.FindObjectsOfTypeAll<T>();
        }
    }
}