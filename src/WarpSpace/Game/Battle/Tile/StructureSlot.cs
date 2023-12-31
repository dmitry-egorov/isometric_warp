﻿using System;
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
            var its_transform = transform;
            tile.s_Sites_Cell.Subscribe(CreateStructure);
            
            void CreateStructure(MTileOccupant the_tiles_site)
            {
                gameObject.DestroyChildren();
            
                if (!the_tiles_site.is_a_Structure(out var structure))
                    return;

                var description = structure.s_Description;
                var prefab = GetPrefab(description);
                var rotation = description.s_Orientation.s_Rotation();

                var structure_component = Instantiate(prefab, its_transform);
                structure_component.transform.localRotation = rotation;
            }
        }

        private GameObject GetPrefab(DStructure structure)
        {
            if (structure.is_an_Entrance())
                return Settings.EntrancePrefab;
            if (structure.is_an_Exit())
                return Settings.ExitPrefab;
            if (structure.is_a_Debris())
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