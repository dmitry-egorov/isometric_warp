using System;
using Lanski.Reactive;
using Lanski.UnityExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace WarpSpace.UI.Gameplay.Units
{
    [ExecuteInEditMode]
    public class HealthComponent: MonoBehaviour
    {
        public GameObject PointPrefab;
        public Material PointMaterial;
        public Material EmptyMaterial;

        public int Total;
        public int Current;

        private ChangeStream<int> _total_changes;
        private ChangeStream<int> _current_changes;
        [NonSerialized] private bool _initialized;

        private void Init()
        {
            if(_initialized)
                return;

            _initialized = true;

            _total_changes = new ChangeStream<int>();
            _current_changes = new ChangeStream<int>();
            
            _total_changes.Subscribe(x => RecreatePoints());
            _current_changes.Subscribe(x => Set_Point_Materials());
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            Init();
            
            _total_changes.Update(Total);
            _current_changes.Update(Current);
        }

        private void RecreatePoints()
        {
            var total = Total;

            gameObject.DestroyChildren();

            for (var i = 0; i < total; i++)
            {
                Instantiate(PointPrefab, transform);
            }
            
            Set_Point_Materials();
        }

        private void Set_Point_Materials()
        {
            for (var i = 0; i < Total; i++)
            {
                transform.GetChild(i).GetComponent<Image>().material = i < Current ? PointMaterial : EmptyMaterial;
            }
        }
    }
}