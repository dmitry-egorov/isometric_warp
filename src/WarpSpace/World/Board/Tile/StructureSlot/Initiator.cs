using System;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.World.Board.Tile.Descriptions;

namespace WarpSpace.World.Board.Tile.StructureSlot
{
    public class Initiator
    {
        private readonly GameObject _entrancePrefab;

        public Initiator(GameObject entrancePrefab)
        {
            _entrancePrefab = entrancePrefab;
        }

        public ComponentSpec? Init(StructureDescription? description)
        {
            return description.Select(s => new ComponentSpec(GetPrefab(s.Type), s.Orientation.ToRotation()));
        }

        private GameObject GetPrefab(StructureType type)
        {
            switch (type)
            {
                case StructureType.Entrance:
                    return _entrancePrefab;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}