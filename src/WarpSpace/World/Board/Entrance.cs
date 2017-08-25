using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.World.Unit;

namespace WarpSpace.World.Board
{
    public class Entrance
    {
        private readonly Tile.Component _tile;
        private readonly GameObject _mothershipPrefab;
        private readonly Direction2D _mothershipOrientation;

        public Entrance(Tile.Component tile, GameObject mothershipPrefab, Direction2D mothershipOrientation)
        {
            _tile = tile;
            _mothershipPrefab = mothershipPrefab;
            _mothershipOrientation = mothershipOrientation;
        }

        public void WarpPlayer()
        {
            var componentSpec = new Unit.ComponentSpec(_mothershipOrientation.ToRotation(), ChassisType.Mothership, _tile);
                
            Unit.Component.Create(_mothershipPrefab, componentSpec);
        }
    }
}