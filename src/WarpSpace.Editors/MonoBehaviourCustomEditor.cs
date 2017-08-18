using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lanski.Reactive;
using Lanski.Reflection;
using UnityEditor;
using UnityEditor.Expose;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WarpSpace.Editors
{
    [CanEditMultipleObjects] // Don't ruin everyone's day
    [CustomEditor(typeof(MonoBehaviour), true)] // Target all MonoBehaviours and descendants
    public class MonoBehaviourCustomEditor : Editor
    {
        private bool _displayPrivateFields;
        private IList<MethodInfo> _exposedMethods;

        private IList<Action> _privateFieldDisplayers;

        void OnEnable()
        {
            var type = target.GetType();

            _exposedMethods = GetExposedMethods();
            _privateFieldDisplayers = GetPrivateFieldDisplayers();
            
            List<MethodInfo> GetExposedMethods()
            {
                return type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.HasAttribute<ExposeMethodInEditorAttribute>())
                    .ToList();
            }
            
            List<Action> GetPrivateFieldDisplayers()
            {
                return type
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => !x.HasAttribute<SerializeField>())
                    .Select(GetFieldDisplayer).ToList();
                
                Action GetFieldDisplayer(FieldInfo field)
                {
                    var fieldType = field.FieldType;
                    var name = field.Name;
            
                    if (fieldType.IsAssignableTo(typeof(Object)))
                    {
                        return () =>
                        {
                            var obj = field.GetValue(target) as Object;
                            EditorGUILayout.ObjectField(name, obj, fieldType, true);
                        };
                    }
            
                    if (fieldType.IsAssignableToGenericType(typeof(ICell<>)))
                    {
                        var prop = fieldType.GetProperty("Value");

                        var cellName = name + " (Cell)";

                        return () =>
                        {
                            var value = field.GetValue(target);
                            var cellValue = prop.GetValue(value, null);

                            GUILayout.BeginHorizontal();
                            GUILayout.Label(cellName);
                            GUILayout.Label(cellValue.ToString());
                            GUILayout.EndHorizontal();
                        };
                    }

                    return () =>
                    {
                        var value = field.GetValue(target);

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(name);
                        GUILayout.Label(value.ToString());
                        GUILayout.EndHorizontal();
                    };
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DisplayExposedMethods();
            DisplayPrivateFields();

            void DisplayExposedMethods()
            {
                foreach (var method in _exposedMethods)
                {
                    if (GUILayout.Button("Run: " + method.Name))
                    {
                        method.Invoke(target, new object[] { });
                    }
                }
            }
            
            void DisplayPrivateFields()
            {
                if (_privateFieldDisplayers.Count == 0)
                    return;

                _displayPrivateFields = EditorGUILayout.Foldout(_displayPrivateFields, "Private Fields");

                if (!_displayPrivateFields)
                    return;

                foreach (var shower in _privateFieldDisplayers)
                {
                    shower();
                }
            }
        }
    }


}