using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarpSpace.Settings
{
    public abstract class SettingsHolder<TSettings, TModel> : MonoBehaviour 
        where TSettings : SettingsObject<TModel>
    {
        public TModel s_Model_Of(TSettings the_settings) => its_models_map[the_settings];
        public TSettings s_Settings_Of(TModel the_type) => its_settings_map[the_type];

        public IReadOnlyList<TModel> s_All_Models => its_models;  
        
        public void Awake()
        {
            var unit_types_settings = Resources.FindObjectsOfTypeAll<TSettings>();
            
            its_settings_map = unit_types_settings.ToDictionary(p => p.Creates_a_Model());
            its_models_map = its_settings_map.ToDictionary(p => p.Value, p => p.Key);
            its_models = its_settings_map.Keys.ToArray();
        }


        private Dictionary<TModel, TSettings> its_settings_map;
        private Dictionary<TSettings, TModel> its_models_map;
        private TModel[] its_models;
    }
}