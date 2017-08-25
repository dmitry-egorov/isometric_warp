using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private IList<MethodInfo> _exposedMethods;

        private IDisplayer _displayerRoot;

        void OnEnable()
        {
            _displayerRoot = new FieldsDisplayer("fields");
            var type = target.GetType();

            _exposedMethods = GetExposedMethods();
            
            List<MethodInfo> GetExposedMethods()
            {
                return type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.HasAttribute<ExposeMethodInEditorAttribute>())
                    .ToList();
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
                _displayerRoot.Display(target);
            }
        }

        private interface IDisplayer
        {
            void Display(object target);
        }

        private class ToStringDisplayer : IDisplayer
        {
            private readonly string _name;
    
            public ToStringDisplayer(string name)
            {
                _name = name;
            }
    
            public void Display(object target)
            {
                if (target == null)
                {
                    DisplayNull(_name);
                    return;
                }
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_name, target.ToString());
                EditorGUILayout.EndHorizontal();
            }
        }

        private class ArrayDisplayer : IDisplayer
        {
            private readonly string _name;
            
            private IDisplayer[] _children;
            private bool _initialized;
            private Type _type;

            public ArrayDisplayer(string name)
            {
                _name = name;
            }

            public void Display(object target)
            {
                if (target == null)
                    DisplayNull(_name);

                var array = (Array)target;
                
                var open = EditorGUILayout.Foldout(_initialized, _name);
                if (open)
                {
                    TryInitialize();
                    DisplayChildren();
                }
                else
                {
                    _initialized = false;
                    _children = null;
                    _type = null;
                }
                
                void TryInitialize()
                {
                    var length = array.Length;
                    var type = array.GetType();

                    if (_initialized && _children.Length == length && _type == type)
                        return;

                    _children = CreateChildren();
                    _type = type;
                    _initialized = true;

                    IDisplayer[] CreateChildren()
                    {
                        var elementType = type.GetElementType();
                        var displayerType = GetDisplayerType(elementType);
                        
                        return Enumerable
                            .Range(0, length)
                            .Select(i => CreateDisplayer($"[{i}]", displayerType))
                            .ToArray();
                    }
                }
                
                void DisplayChildren()
                {
                    EditorGUI.indentLevel++;
                    for (var i = 0; i < array.Length; i++)
                    {
                        _children[i].Display(array.GetValue(i));
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
        
        private class UnityObjectDisplayer : IDisplayer
        {
            private readonly string _name;
    
            public UnityObjectDisplayer(string name)
            {
                _name = name;
            }
    
            public void Display(object target)
            {
                if (target == null)
                {
                    DisplayNull(_name);
                    return;
                }
                
                var obj = (Object)target;
                EditorGUILayout.ObjectField(_name, obj, target.GetType(), true);
            }
        }

        private class FieldsDisplayer: IDisplayer
        {
            private readonly string _name;
            private bool _initialized;
            private (FieldInfo info, IDisplayer node)[] _children;
            private Type _type;

            public FieldsDisplayer(string name)
            {
                _name = name;
            }
    
            public void Display(object target)
            {
                if (target == null)
                {
                    DisplayNull(_name);
                    return;
                }
                
                var open = EditorGUILayout.Foldout(_initialized, _name);
    
                if (open)
                {
                    TryInitialize();
    
                    DisplayChildren();
                }
                else
                {
                    _children = null;
                    _type = null;
                    _initialized = false;
                }
                
                void TryInitialize()
                {
                    var type = target.GetType();

                    if (_initialized && type == _type)
                        return;

                    _children = CreateChildren();
                    _type = type;
                    _initialized = true;

                    (FieldInfo info, IDisplayer node)[] CreateChildren()
                    {
                        return GetPrivateFields(target)
                            .Select(CreateChild)
                            .ToArray();
                    }

                    (FieldInfo field, IDisplayer node) CreateChild(FieldInfo field)
                    {
                        var node = CreateNode();

                        return (field, node);

                        IDisplayer CreateNode()
                        {
                            var name = field.Name;
                            var fieldType = field.FieldType;

                            var displayerType = GetDisplayerType(fieldType);

                            return CreateDisplayer(name, displayerType);
                        }
                    }
                }

                void DisplayChildren()
                {
                    EditorGUI.indentLevel++;
                    foreach (var child in _children)
                    {
                        child.node.Display(child.info.GetValue(target));
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        private static Type GetDisplayerType(Type type)
        {
            if (type.IsAssignableTo(typeof(Object)))
                return typeof(UnityObjectDisplayer);
            
            if (type.IsArray)
                return typeof(ArrayDisplayer);
                            
            if (type.IsSimple() || type.IsToStringOverriden())
                return typeof(ToStringDisplayer);
    
            return typeof(FieldsDisplayer);
        }

        private static IDisplayer CreateDisplayer(string name, Type displayerType)
        {
            if (displayerType == typeof(UnityObjectDisplayer))
                return new UnityObjectDisplayer(name);
            
            if (displayerType == typeof(ArrayDisplayer))
                return new ArrayDisplayer(name);

            if (displayerType == typeof(ToStringDisplayer))
                return new ToStringDisplayer(name);

            return new FieldsDisplayer(name);
        }

        private static void DisplayNull(string name)
        {
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(name, "null");
            EditorGUILayout.EndHorizontal();
        }
    
        private static IEnumerable<FieldInfo> GetPrivateFields(object target)
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            
            return target
                .GetType()
                .GetFields(bindingFlags)
                ;
        }
    }
}