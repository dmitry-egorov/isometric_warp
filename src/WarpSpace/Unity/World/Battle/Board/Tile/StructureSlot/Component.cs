using System;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Descriptions;

namespace WarpSpace.Unity.World.Battle.Board.Tile.StructureSlot
{
    public class Component: MonoBehaviour
    {
        public OwnSettings Settings;

        public void Init(StructureDescription? description)
        {
            description.Do(s =>
            {
                var prefab = GetPrefab(s.Type);
                var rotation = s.Orientation.ToRotation();
                
                var structure = Instantiate(prefab, transform);
                structure.transform.localRotation = rotation;
            });
        }

        private GameObject GetPrefab(StructureType type)
        {
            switch (type)
            {
                case StructureType.Entrance:
                    return Settings.EntrancePrefab;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        [Serializable]
        public struct OwnSettings
        {
            public GameObject EntrancePrefab;
        }
    }
}