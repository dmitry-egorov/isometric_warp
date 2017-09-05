using System;

namespace WarpSpace.Settings
{
    [Serializable]
    public struct FactionsSettings
    {
        public FactionSettings Player;
        public FactionSettings Natives;
    }
}