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
            
            if (DidAnyParameterChange())
            {
                PublishParametersChanged();
            }
        }

        protected abstract void Init();
        protected abstract void OnParameterChanged();

        private void TryInit()
        {
            if (_initialized)
                return;
            _initialized = true;
            
            _getters = GetFieldGetters();
            _lastValues = new object[_getters.Length];

            Init();

            PublishParametersChanged();

        }

        private bool DidAnyParameterChange()
        {
            return _getters
                .Select((x, i) => (x, i))
                .Any(g => !Equals(g.Item1(), _lastValues[g.Item2]));
        }

        private void PublishParametersChanged()
        {
            UpdateLastValues();
            OnParameterChanged();
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