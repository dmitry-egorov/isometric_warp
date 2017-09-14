using System;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Settings
{
    [Serializable]
    public class UnitTypeModelSettings
    {
        public int TotalHitPoints;
        public int TotalMoves;
        public int BaySize;
        public WeaponType WeaponType;
        public ChassisTypeSettings ChassisType;
        public int Loot;
        public int InitialInventory;
        public bool CanDock;
        public bool CanExit;
        public char SerializationSymbol;

        public MUnitType s_Description() => new MUnitType(
            TotalHitPoints,
            TotalMoves,
            BaySize,
            WeaponType,
            UnityEngine.Object.FindObjectOfType<ChassisTypeSettingsHolder>().s_Model_Of(ChassisType),
            InventoryHelper.Possible_Stuff_From(Loot),
            InventoryHelper.Possible_Stuff_From(InitialInventory),
            CanDock,
            CanExit,
            SerializationSymbol
        );
    }
}