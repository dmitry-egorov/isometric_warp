using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;

namespace WarpSpace.World.Unit
{
    public class Component: MonoBehaviour
    {
        public GameObject Outline;
        
        private ValueCell<bool> _isSelectedCell;
        private ChassisType _chassis;
        private Board.Tile.Component _tile;

        public Board.Tile.Component Tile => _tile;
        public ChassisType Chassis => _chassis;

        public static Component Create(GameObject prefab, ComponentSpec spec)
        {
            var obj = Instantiate(prefab, spec.Tile.UnitSlot.transform).GetComponent<Component>();

            obj.Init(spec);

            return obj;
        }

        private void Init(ComponentSpec spec)
        {
            transform.localRotation = spec.Rotation;
            _chassis = spec.Chassis;
            _tile = spec.Tile;
            
            _isSelectedCell = new ValueCell<bool>(false);

            _isSelectedCell.Subscribe(_ => UpdateOutline());
            
            _tile.UnitSlot.Set(this);
            
            void UpdateOutline()
            {
                Outline.SetActive(_isSelectedCell.Value);
            }
        }

        public void SetIsSelected(bool isSelected)
        {
            _isSelectedCell.Value = isSelected;
        }

        public bool TryMoveTo(Board.Tile.Component tile)
        {
            if (!CanMoveTo(tile))
                return false;

            transform.parent = tile.UnitSlot.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = _tile.GetDirectionTo(tile).ToRotation();
            
            _tile.UnitSlot.Reset();
            _tile = tile;
            _tile.UnitSlot.Set(this);

            return true;
        }

        private bool CanMoveTo(Board.Tile.Component destination)
        {
            return destination.Landscape.IsPassableBy(this) && _tile.IsAdjacentTo(destination);
        }
    }

    public enum ChassisType
    {
        Mothership
    }
}