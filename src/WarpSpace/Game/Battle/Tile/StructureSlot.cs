using System;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Game.Battle.Tile
{
    public class StructureSlot: MonoBehaviour
    {
        public OwnSettings Settings;

        public void Init(MTile tile)
        {
            tile.Site_Cell.Subscribe(CreateStructure);
        }

        private void CreateStructure(TileSite tile_site)
        {
            gameObject.DestroyChildren();
            
            if (!tile_site.Is_a_Structure(out var structure))
                return;

            var description = structure.Description;
            var prefab = GetPrefab(description);
            var rotation = description.Orientation.To_Rotation();

            var structure_component = Instantiate(prefab, transform);
            structure_component.transform.localRotation = rotation;
        }

        private GameObject GetPrefab(StructureDescription structure)
        {
            if (structure.Is_An_Entrance())
                return Settings.EntrancePrefab;
            if (structure.Is_An_Exit())
                return Settings.ExitPrefab;
            if (structure.Is_A_Debris())
                return Settings.DebriePrefab;
            
            throw new ArgumentOutOfRangeException(nameof(structure), structure, null);
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