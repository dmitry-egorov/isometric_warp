using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MUnit
    {
        internal MUnit(int the_id, DUnit the_desc, MTile the_initial_location, MGame the_game, SignalGuard the_signal_guard)
        {
            s_Id = the_id;
            s_Type = the_desc.s_Type;
            s_Name = $"{s_Type.s_Name} {s_Id}";
            s_Faction = the_desc.s_Faction;

            s_Locator           = new MLocator(this, the_initial_location, the_signal_guard);
            s_Weapon            = new MWeapon(this, the_signal_guard);
            s_Health            = new MHealth(this, the_signal_guard);
            s_Inventory         = new MInventory(the_desc.s_Inventory_Content, the_signal_guard);
            s_Looter            = new MLooter(this);
            s_Interactor        = new MInteractor(this, the_game);
            s_Destructor        = new MDestructor(this, the_signal_guard);
            s_Actions_Container = new MActionsContainer(this);
        }

        public string            s_Name { get; }
        public int               s_Id { get; }
        public MUnitType         s_Type { get; }
        public MHealth           s_Health { get; }
        public MFaction          s_Faction { get; }
        public MLocator          s_Locator { get; }
        public MWeapon           s_Weapon { get; }
        public MInventory        s_Inventory { get; }
        public MInteractor       s_Interactor { get; }
        public MDestructor       s_Destructor { get; }
        public MLooter           s_Looter { get; }
        public MActionsContainer s_Actions_Container { get; }

        public override string ToString() => $"{s_Type} {s_Id}";

        internal void Finishes_the_Turn()
        {
            this.s_Weapon.Resets();
            this.s_Locator.Finishes_the_Turn();
        }
    }

    public static class MUnitReadExtensions
    {
        public static MWeaponType s_Weapon_Type(this MUnit the_unit) => the_unit.s_Type.s_Weapon_Type;
        public static int s_Total_Moves(this MUnit the_unit) => the_unit.s_Type.s_Total_Moves;
        public static int s_Total_Hit_Points(this MUnit the_unit) => the_unit.s_Type.s_Total_Hit_Points;
        public static MChassisType s_Chassis_Type(this MUnit the_unit) => the_unit.s_Type.s_Chassis_Type;
        public static DStuff s_Remains(this MUnit the_unit) => the_unit.s_Type.s_Remains;
        public static bool can_Exit(this MUnit the_unit) => the_unit.s_Type.can_Exit;
        
        public static MTile s_Location(this MUnit the_unit) => the_unit.s_Locator.s_Location;
        public static bool is_Alive(this MUnit the_m_unit) => the_m_unit.s_Health.is_Normal;
        public static bool can_Move(this MUnit the_unit) => the_unit.s_Locator.can_Move;
        public static bool is_Adjacent_To(this MUnit the_unit, MTile the_tile) => the_unit.s_Locator.is_Adjacent_To(the_tile);
        public static Possible<Index2D> s_Possible_Position(this MUnit the_unit) => the_unit.s_Locator.s_Position;
        public static DStuff s_Loot(this MUnit the_unit) => the_unit.s_Inventory_Content().and(the_unit.s_Remains());
        public static DStuff s_Inventory_Content(this MUnit the_unit) => the_unit.s_Inventory.s_Content;
        
        public static ICell<int> s_Cell_of_Current_Hitpoints(this MUnit the_unit) => the_unit.s_Health.s_Current_Hitpoints_Cell;
        public static ICell<bool> s_Cell_of_can_Move(this MUnit the_unit) => the_unit.s_Locator.s_Can_Move_Cell;
        public static ICell<bool> s_Cell_of_can_Fire(this MUnit the_unit) => the_unit.s_Weapon.s_Can_Fire_Cell;
        public static ICell<DStuff> s_Cell_of_Inventory_Contents(this MUnit the_unit) => the_unit.s_Inventory.s_Stuffs_Cell;
        public static ICell<int> s_Cell_of_Moves_Left(this MUnit the_unit) => the_unit.s_Locator.s_Moves_Left_Cell;

        public static IStream<Movement> Moved(this MUnit the_unit) => the_unit.s_Locator.Moved;
        public static IStream<Firing> Fired(this MUnit the_unit) => the_unit.s_Weapon.Fired;
        public static IStream<TheVoid> Been_Destroyed(this MUnit the_unit) => the_unit.s_Destructor.Destructed;
        
        public static Possible<MUnitAction> s_possible_Action_For(this MUnit the_unit, DUnitAction the_action_desc) => the_unit.s_Actions_Container.s_possible_Action_For(the_action_desc);
        public static Possible<UnitCommand> s_Regular_Command_At(this MUnit the_unit, MTile the_tile) => the_unit.s_Actions_Container.s_Regular_Command_At(the_tile);
        public static bool is_Adjacent_To(this MUnit the_unit, MStructure the_structure) => the_unit.s_Locator.is_Adjacent_To(the_structure);
        public static bool Belongs_To(this MUnit the_unit, MFaction the_faction) => the_unit.s_Faction == the_faction;
        public static bool can_Move_To(this MUnit the_unit, MTile the_destination) => the_unit.s_Locator.can_Move_To(the_destination);
        public static bool can_Interact_With_a_Structure_At(this MUnit the_unit, MTile the_tile, out MStructure the_structure) => the_unit.s_Interactor.can_Interact_With_a_Structure_At(the_tile, out the_structure);
        public static bool can_Perform_Special_Actions(this MUnit the_unit) => false;//TODO: investigate
    }

    public static class MUnitWriteExtensions
    {
        internal static void Moves_To(this MUnit the_unit, MTile the_destination) => the_unit.s_Locator.Moves_To(the_destination);
        internal static void Interacts_With(this MUnit the_unit, MStructure the_structure) => the_unit.s_Interactor.Interacts_With(the_structure);
        internal static void Takes(this MUnit the_unit, DDamage the_damage) => the_unit.s_Health.Takes(the_damage);
        internal static void Destructs(this MUnit the_unit) => the_unit.s_Destructor.Destructs();
        internal static void Takes(this MUnit the_unit, DStuff the_loot) => the_unit.s_Inventory.Adds(the_loot);
        internal static void Loots(this MUnit the_unit, MStructure the_structure) => the_unit.s_Looter.Loots(the_structure);
    }
}