using UnityEngine;

namespace WarpSpace.Settings
{
    public abstract class SettingsObject<TModel>: ScriptableObject
    {
        public abstract TModel Creates_a_Model();
    }
}