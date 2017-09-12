using System;
using UnityEngine;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    public class UnitSettingsHolder: MonoBehaviour
    {
        public FactionsSettings Factions;
        public UnitsSettings Units;

        public UnitSettings For(UnitType unit_type)
        {
            switch (unit_type)
            {
                case UnitType.a_Mothership:
                    return Units.Mothership;
                case UnitType.a_Tank:
                    return Units.Tank;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit_type), unit_type, null);
            }
        }

        public FactionSettings For(Faction faction)
        {
            switch (faction)
            {
                case Faction.the_Player_Faction:
                    return Factions.Player;
                case Faction.the_Natives:
                    return Factions.Natives;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }
    }
}