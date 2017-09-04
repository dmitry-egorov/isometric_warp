using System;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Unity.World.Battle.Board.Tile.StructureSlot
{
    public class Component: MonoBehaviour
    {
        public OwnSettings Settings;

        public void Init(TileModel tile)
        {
            tile.Site_Cell.Subscribe(CreateStructure);
        }

        private void CreateStructure(TileSite tile_site)
        {
            gameObject.DestroyChildren();
            
            if (!tile_site.Is_a_Structure(out var structure))
                return;

            var description = structure.Description;
            var prefab = GetPrefab(description.TheType);
            var rotation = description.Orientation.To_Rotation();

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