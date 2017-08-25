using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Lanski.Reflection;
using UnityEngine;

namespace Lanski.Behaviours
{
    [ExecuteInEditMode]
    public abstract class EditorInitializable: MonoBehaviour
    {
        [NonSerialized] private bool _initialized;

        [NonSerialized] private Func<object>[] _getters;
        [NonSerialized] private object[] _lastValues;

        public void Update()
        {
            TryInit();
            
            if (Application.isPlaying)
                return;
            
            if (DidAnyParameterChange())
            {
                OnParameterChanged();
                UpdateLastValues();
            }
        }

        protected abstract void Init();
        protected abstract void OnParameterChanged();

        private void TryInit()
        {
            if (_initialized)
                return;
            _initialized = true;
            
            Init();
            OnParameterChanged();

            if (Application.isPlaying)
                return;

            _getters = GetFieldGetters();
            _lastValues = new object[_getters.Length];
            UpdateLastValues();
        }

        private bool DidAnyParameterChange()
        {
            for (var i = 0; i < _getters.Length; i++)
            {
                var value = _getters[i]();
                var lastValue = _lastValues[i];

                if (!Equals(value, lastValue))
                    return true;
            }

            return false;
            
            //return _getters
            //    .Select((x, i) => (x, i))
            //    .Any(g => !Equals(g.Item1(), _lastValues[g.Item2]));
        }

        private void UpdateLastValues()
        {
            for (var i = 0; i < _getters.Length; i++)
            {
                _lastValues[i] = _getters[i]();
            }
        }

        private Func<object>[] GetFieldGetters()
        {
            return 
                GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Select(f => new Func<object>(() => f.GetValue(this)))
                    .ToArray();
        }
    }
}