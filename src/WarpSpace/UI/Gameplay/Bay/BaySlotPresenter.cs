using System;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay.Bay
{
    [RequireComponent(typeof(Image))]
    public class BaySlotPresenter : MonoBehaviour
    {
        public Material EmptyBackgroundMaterial;
        public Material UnitBackgroundMaterial;
        

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (_initialised)
                return;
            _initialised = true;
            
            _unit_mesh = GetComponentInChildren<UnitMesh>();
            _background = GetComponent<Image>();
        }

        public void Present(MLocation location)
        {
            Init();
            
            _last_subscription?.Invoke();

            _last_subscription =
                location
                    .Possible_Occupant_Cell
                    .Subscribe(Present_Unit)
            ;
        }


        private void Present_Unit(Possible<MUnit> possible_unit)
        {
            if (possible_unit.Has_a_Value(out var unit))
            {
                _unit_mesh.Present(unit.Type, unit.Faction);
                _background.material = UnitBackgroundMaterial;
            }
            else
            {
                _unit_mesh.Hide();
                _background.material = EmptyBackgroundMaterial;
            }
        }
        
        private bool _initialised;
        
        private UnitMesh _unit_mesh;
        private Image _background;
        
        private Action _last_subscription;
    }
}