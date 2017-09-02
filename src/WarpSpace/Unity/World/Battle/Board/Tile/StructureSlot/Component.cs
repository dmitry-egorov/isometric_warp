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

        public void Init(StructureDescription? description_slot)
        {
            if (!description_slot.Has_a_Value(out var description))
                return;
            
            var prefab = GetPrefab(description.Type);
            var rotation = description.Orientation.ToRotation();

            var structure = Instantiate(prefab, transform);
            structure.transform.localRotation = rotation;
        }

        private GameObject GetPrefab(StructureType type)
        {
            switch (type)
            {
                case StructureType.Entrance:
                    return Settings.EntrancePrefab;
                case StructureType.Exit:
                    return Settings.ExitPrefab;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        [Serializable]
        public struct OwnSettings
        {
            public GameObject EntrancePrefab;
            public GameObject ExitPrefab;
        }
    }
}