using UnityEngine;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Game.Battle.BoardGenerators
{
    public abstract class BoardSettings : ScriptableObject
    {
        public abstract DBoard s_Description { get; }
    }
}