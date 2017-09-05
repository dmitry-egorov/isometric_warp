using System;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;

namespace WarpSpace.Settings
{
    [Serializable]
    public struct UnitSettings
    {
        public MovementSettings Movement;
        public Mesh Mesh;
    }
}