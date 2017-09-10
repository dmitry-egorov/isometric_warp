using System;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Settings;

namespace WarpSpace.Overlay.Units
{
    [RequireComponent(typeof(OutlineMeshBuilder))]
    [RequireComponent(typeof(RectTransform))]
    public class OOutliner : MonoBehaviour
    {
        public void Inits_With(WUnit the_unit_component) => it.inits_with(the_unit_component);
        public void OnDestroy() => it.destructs();
        public void Update() => it.updates();

        private void inits_with(WUnit the_unit_component)
        {
            var the_unit = the_unit_component.s_Unit;
            if (!the_unit.s_Faction_is(Faction.Player))
            {
                Destroy(gameObject);
                return;
            }
            
            it.s_transform = GetComponent<RectTransform>();
            it.s_component_transform = the_unit_component.s_Transform;
            it.s_unit = the_unit;
            
            it.builds_the_mesh(s_unit);
            it.s_subscription = it.wires_the_selected_unit();

            it.is_initialized = true;
        }

        //Note: every unit' outline wires to the player
        private Action wires_the_selected_unit() => 
            FindObjectOfType<BattleComponent>()
            .s_Players_Selected_Units_Cell
            .Subscribe(handles_the_selected_unit_change)
        ;

        private void handles_the_selected_unit_change(Possible<MUnit> possibly_selected_unit)
        {
            if (possibly_selected_unit.has_a_Value(out var the_selected_unit) && the_selected_unit == it.s_unit)
            {
                it.activates();
            }
            else
            {
                it.deactivates();
            }
        }

        private void deactivates()
        {
            gameObject.Hide();
        }

        private void activates()
        {
            gameObject.Show();
        }

        private void builds_the_mesh(MUnit the_unit)
        {
            var settings_holder = FindObjectOfType<UnitSettingsHolder>();
            var mesh = settings_holder.For(the_unit.s_Type).Mesh;
            var the_destination_mesh_filter = GetComponent<MeshFilter>();
            OutlineMeshBuilder.Builds(mesh, the_destination_mesh_filter);
        }

        private void updates()
        {
            it.is_initialized.Must_Be_True_Otherwise_Throw("The OOutliner must be initialized before the first update");
            it.s_transform.rotation = it.s_component_transform.rotation;
        }
        
        private void destructs()
        {
            if(!it.is_initialized)
                return;
            
            it.s_subscription();
        }

        private OOutliner it => this;
        private bool is_initialized;
        private Transform s_transform;
        private Transform s_component_transform;
        private Action s_subscription;
        private MUnit s_unit;
    }
}