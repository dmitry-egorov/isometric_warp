using System;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Settings
{
    [Serializable]
    public struct UnitTypeModelSettings
    {
        public string Name;
        public int TotalHitPoints;
        public int TotalMoves;
        public WeaponTypeSettings WeaponType;
        public ChassisTypeSettings ChassisType;
        public int Loot;
        public int InitialInventory;
        public bool CanExit;
        public char SerializationSymbol;

        public MUnitType s_Description() => new MUnitType(
            Name,
            TotalHitPoints,
            TotalMoves,
            WeaponTypeSettings.s_Model_Of(WeaponType),
            ChassisTypeSettings.s_Model_Of(ChassisType),
            InventoryHelper.Possible_Stuff_From(Loot),
            InventoryHelper.Possible_Stuff_From(InitialInventory),
            CanExit,
            SerializationSymbol
        );
    }
}