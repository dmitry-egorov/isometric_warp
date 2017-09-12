using System;
using System.Collections.Generic;
using Lanski.Reactive;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static WarpSpace.Models.Descriptions.Faction;

namespace WarpSpace.Overlay.Units
{
    [ExecuteInEditMode]
    public class OPointsCounter: MonoBehaviour
    {
        public CounterType Type;
        public Material PointMaterial;
        public Material EmptyMaterial;

        public int Total;
        public int Current;

        public void Start() => it_inits();
        public void Update() => it_updates();
        public void OnDestroy() => it_destructs();

        private void it_inits()
        {
            if (it_is_initialized)
                return;

            its_unit = gameObject.GetComponentInParent<IUnitReference>().s_Unit;
            if (!its_avalable_for_the_unit())
            {
                Destroy(gameObject);
                return;
            }
            its_subscription = it_wires_the_source();
            
            its_transform = transform;

            its_prototype_child = its_transform.GetChild(0).GetComponent<Image>();
            its_children = new List<Image> { its_prototype_child };
            
            its_changes_stream = new ChangeStream<int, int>();
            its_changes_stream.Subscribe(x => it_updates_its_children());
            
            it_is_initialized = true;
        }

        private void it_updates()
        {
            it_inits();

            its_changes_stream?.updates_with(Total, Current);
        }
        
        private void it_destructs() => its_subscription?.Invoke();

        private bool its_avalable_for_the_unit()
        {
            switch (Type)
            {
                case CounterType.Health:
                    return true;
                case CounterType.Moves:
                    return its_unit.Belongs_To(the_Player_Faction);
                case CounterType.CanFire:
                    return its_unit.Belongs_To(the_Player_Faction);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Action it_wires_the_source()
        {
            switch (Type)
            {
                case CounterType.Health:
                    return it_wires_the_health();
                case CounterType.Moves:
                    return it_wires_the_moves();
                case CounterType.CanFire:
                    return it_wires_the_weapon();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Action it_wires_the_weapon()
        {
            Total = 1;
            return its_unit.s_can_Fire_Cell.Subscribe(moves_left => Current = moves_left ? 1 : 0);
        }

        private Action it_wires_the_moves()
        {
            var the_unit = its_unit;

            Total = the_unit.s_Total_Moves;
            return the_unit.s_Moves_Left_Cell.Subscribe(moves_left => Current = moves_left);
        }

        private Action it_wires_the_health()
        {
            return its_unit.s_Health_States_Cell.Subscribe(state =>
            {
                Current = state.s_Current_Hit_Points;
                Total = state.s_Total_Hit_Points;
            });
        }

        private void it_updates_its_children()
        {
            it_adds_missing_children();
            it_updates_children_visibility();
        }

        private void it_updates_children_visibility()
        {
            for (var i = 0; i < its_children.Count; i++)
            {
                var the_child = its_children[i];
                the_child.gameObject.SetActive(i < Total);
                the_child.material = i < Current ? PointMaterial : EmptyMaterial;
            }
        }

        private void it_adds_missing_children()
        {
            while (its_children.Count < Total)
            {
                var child = Instantiate(its_prototype_child, its_transform);
                its_children.Add(child);
            }
        }

        
        private bool it_is_initialized;
        private MUnit its_unit;
        private ChangeStream<int, int> its_changes_stream;
        private IList<Image> its_children;
        private Image its_prototype_child;
        private Transform its_transform;
        private Action its_subscription;

        public enum CounterType
        {
            Health,
            Moves,
            CanFire
        }
    }
}