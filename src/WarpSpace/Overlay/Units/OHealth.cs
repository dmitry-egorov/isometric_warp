using System;
using Lanski.Reactive;
using Lanski.UnityExtensions;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Overlay.Units
{
    [ExecuteInEditMode]
    public class OHealth: MonoBehaviour
    {
        public GameObject PointPrefab;
        public Material PointMaterial;
        public Material EmptyMaterial;

        public int Total;
        public int Current;

        public void Update() => it.updates();
        public void Inits_With(MUnit the_unit) => it.inits_with(the_unit);
        public void OnDestroy() => it.s_subscription();

        private void updates()
        {
            if (!it.is_initialized)
                return;

            it.s_total_changes_stream.Update(Total);
            it.s_current_changes_stream.Update(Current);
        }

        private void inits_with(MUnit the_unit)
        {
            it.s_subscription =
                the_unit.s_Health_States_Cell
                    .Subscribe(state =>
                    {
                        Current = state.s_Current_Hit_Points;
                        Total = state.s_Total_Hit_Points;
                    });
            
            it.s_transform = transform;

            it.s_total_changes_stream = new ChangeStream<int>();
            it.s_current_changes_stream = new ChangeStream<int>();
            
            it.s_total_changes_stream.Subscribe(x => it.recreates_health_points());
            it.s_current_changes_stream.Subscribe(x => it.sets_point_materials());
            
            it.is_initialized = true;
        }

        private void recreates_health_points()
        {
            gameObject.DestroyChildren();

            it.Instantiates_Health_Points();
            it.sets_point_materials();
        }
            
        private void Instantiates_Health_Points()
        {
            for (var i = 0; i < Total; i++)
            {
                Instantiate(PointPrefab, it.s_transform);
            }    
        }
            
        private void sets_point_materials()
        {
            for (var i = 0; i < Total; i++)
            {
                it.s_transform.GetChild(i).GetComponent<Image>().material = i < Current ? PointMaterial : EmptyMaterial;
            }
        }

        private OHealth it => this;
        
        private bool is_initialized;
        private Action s_subscription;
        private ChangeStream<int> s_total_changes_stream;
        private ChangeStream<int> s_current_changes_stream;
        private Transform s_transform;
    }
}