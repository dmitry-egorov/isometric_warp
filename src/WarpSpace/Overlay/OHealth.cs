using System;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Overlay
{
    [ExecuteInEditMode]
    public class OHealth: MonoBehaviour
    {
        public GameObject PointPrefab;
        public Material PointMaterial;
        public Material EmptyMaterial;

        public int Total;
        public int Current;

        private void Init()
        {
            if(_initialized)
                return;

            _initialized = true;
            
            _total_changes = new ChangeStream<int>();
            _current_changes = new ChangeStream<int>();
            
            _total_changes.Subscribe(x => Recreates_Health_Points());
            _current_changes.Subscribe(x => Sets_Point_Materials());
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

        public void Watches(Possible<MHealth> the_possible_units_health)
        {
            the_possible_last_subscription.Do(s => s());
            
            if (the_possible_units_health.Has_a_Value(out var the_units_health))
            {
                the_possible_last_subscription = 
                    the_units_health.s_States_Cell()
                    .Subscribe(state =>
                    {
                        Current = state.s_Current_Hit_Points();
                        Total = state.s_Total_Hit_Points();
                    });
            }
            
        }

        private void Recreates_Health_Points()
        {
            var total = Total;

            gameObject.DestroyChildren();

            for (var i = 0; i < total; i++)
            {
                Instantiate(PointPrefab, transform);
            }
            
            Sets_Point_Materials();
        }

        private void Sets_Point_Materials()
        {
            for (var i = 0; i < Total; i++)
            {
                transform.GetChild(i).GetComponent<Image>().material = i < Current ? PointMaterial : EmptyMaterial;
            }
        }
        
        private Possible<Action> the_possible_last_subscription;
        private ChangeStream<int> _total_changes;
        private ChangeStream<int> _current_changes;
        [NonSerialized] private bool _initialized;
    }
}