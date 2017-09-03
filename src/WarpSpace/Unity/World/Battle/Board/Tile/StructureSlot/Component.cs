using System;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Tile.Structure;

namespace WarpSpace.Unity.World.Battle.Board.Tile.StructureSlot
{
    public class Component: MonoBehaviour
    {
        public OwnSettings Settings;

        public void Init(TileModel tile)
        {
            tile.Structure_Cell.Subscribe(CreateStructure);
        }

        private void CreateStructure(Slot<StructureModel> possible_structure)
        {
            gameObject.DestroyChildren();
            
            if (!possible_structure.Has_a_Value(out var structure))
                return;

            var description = structure.Description;
            var prefab = GetPrefab(description.TheType);
            var rotation = description.Orientation.ToRotation();

            var structure_component = Instantiate(prefab, transform);
            structure_component.transform.localRotation = rotation;
        }

        private GameObject GetPrefab(StructureDescription.Type type)
        {
            switch (type)
            {
                case StructureDescription.Type.Entrance:
                    return Settings.EntrancePrefab;
                case StructureDescription.Type.Exit:
                    return Settings.ExitPrefab;
                case StructureDescription.Type.Debris:
                    return Settings.DebriePrefab;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        [Serializable]
        public struct OwnSettings
        {
            public GameObject EntrancePrefab;
            public GameObject ExitPrefab;
            public GameObject DebriePrefab;
        }
    }
}