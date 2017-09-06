using System;
using UnityEngine;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    public class UnitSettingsHolder: MonoBehaviour
    {
        public FactionsSettings Factions;
        public UnitsSettings Units;

        public UnitSettings Get_Settings_For(UnitType unit_type)
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

        public FactionSettings Get_Settings_For(Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return Factions.Player;
                case Faction.Natives:
                    return Factions.Natives;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }
    }
}