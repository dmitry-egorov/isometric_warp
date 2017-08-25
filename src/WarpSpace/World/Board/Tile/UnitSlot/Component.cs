using JetBrains.Annotations;
using UnityEngine;

namespace WarpSpace.World.Board.Tile.UnitSlot
{
    public class Component: MonoBehaviour
    {
        [CanBeNull] private Unit.Component _unit;

        public Unit.Component Unit => _unit;

        public void Reset()
        {
            _unit = null;
        }

        public void Set(Unit.Component unit)
        {
            _unit = unit;
        }
    }
}