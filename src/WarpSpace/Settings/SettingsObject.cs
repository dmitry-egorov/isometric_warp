using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.Settings
{
    public abstract class SettingsObject<TSettings, TModel>: ScriptableObject
        where TSettings: SettingsObject<TSettings, TModel>
    {
        protected abstract TModel Creates_a_Model();

        public static TSettings Of(TModel the_model) => s_Holder.s_Settings_Of(the_model);
        public static TModel s_Model_Of(TSettings the_model) => s_Holder.s_Model_Of(the_model);
        public static IReadOnlyList<TModel> s_All_Models => s_Holder.s_All_Models;
        public static IReadOnlyList<TSettings> s_All_Settings => s_Holder.s_All_Settings;

        private static SettingsHolder s_Holder => its_possible_holder.has_a_Value(out var the_holder) ? the_holder : (its_possible_holder = new SettingsHolder()).must_have_a_Value();
        private static Possible<SettingsHolder> its_possible_holder;

        private class SettingsHolder
        {
            public SettingsHolder()
            {
                its_settings = ResourcesLoader.Load<TSettings>();
                its_settings_map = its_settings.ToDictionary(p => p.Creates_a_Model());
                its_models_map = its_settings_map.ToDictionary(p => p.Value, p => p.Key);
                its_models = its_settings_map.Keys.ToArray();
            }

            public TModel s_Model_Of(TSettings the_settings) => its_models_map[the_settings];
            public TSettings s_Settings_Of(TModel the_type) => its_settings_map[the_type];

            public IReadOnlyList<TModel> s_All_Models => its_models;
            public IReadOnlyList<TSettings> s_All_Settings => its_settings;

            private readonly IReadOnlyDictionary<TModel, TSettings> its_settings_map;
            private readonly IReadOnlyDictionary<TSettings, TModel> its_models_map;
            private readonly IReadOnlyList<TModel> its_models;
            private readonly IReadOnlyList<TSettings> its_settings;
        }
    }
}