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
        
        private HorizontalLayoutGroup _group;
        private ContentSizeFitter _fitter;

        private void Init()
        {
            if(_initialized)
                return;

            _initialized = true;
            
            _group = GetComponent<HorizontalLayoutGroup>();
            _fitter = GetComponent<ContentSizeFitter>();

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

            //_group.enabled = true;
            //_fitter.enabled = true;
            
            gameObject.DestroyChildren();

            for (var i = 0; i < total; i++)
            {
                Instantiate(PointPrefab, transform);
            }
            
            //_group.enabled = false;
            //_fitter.enabled = false;
            
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