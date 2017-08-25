using System;
using System.Linq;
using Lanski.Linq;
using Lanski.UnityExtensions;
using UnityEngine;

namespace WarpSpace.World.Board
{
    [Serializable]
    public struct Settings
    {
        public GameObject TilePrefab;
        public GameObject MothershipPrefab;
        
        public Tile.Landscape.Settings Landscape;
        public Tile.StructureSlot.Settings Structure;
        public Tile.Water.Settings Water;
    }
}