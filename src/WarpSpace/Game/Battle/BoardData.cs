using System;
using UnityEngine;
using WarpSpace.Common;

namespace WarpSpace.Game.Battle
{
    [Serializable]
    public struct BoardData
    {
        [TextArea(8,8)] public string Tiles;
        [TextArea(8,8)] public string Units;
        public Spacial2DData Entrance;
        public Spacial2DData Exit;
    }
}