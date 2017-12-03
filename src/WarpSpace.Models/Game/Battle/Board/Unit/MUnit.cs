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
        internal MUnit(int the_id, DUnit the_desc, MLocation the_initial_location, MGame the_game, SignalGuard the_signal_guard)
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
            s_Possible_Docker   = MDocker.From(this);
            s_Possible_Bay      = MBay.From(this, the_signal_guard);
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
        public Possible<MBay>    s_Possible_Bay { get; }
        public Possible<MDocker> s_Possible_Docker { get; }
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
        public static int s_Bay_Size(this MUnit the_unit) => the_unit.s_Type.s_Bay_Size;
        public static MChassisType s_Chassis_Type(this MUnit the_unit) => the_unit.s_Type.s_Chassis_Type;
        public static Possible<DStuff> s_Remains(this MUnit the_unit) => the_unit.s_Type.s_Remains;
        public static bool can_Exit(this MUnit the_unit) => the_unit.s_Type.can_Exit;
        public static bool can_Dock(this MUnit the_unit) => the_unit.s_Type.can_Dock;
        
        public static bool is_Alive(this MUnit the_m_unit) => the_m_unit.s_Health.is_Normal;
        public static bool can_Move(this MUnit the_unit) => the_unit.s_Locator.can_Move;
        public static bool is_Docked(this MUnit the_unit) => the_unit.s_Locator.s_Is_Docked;
        public static bool is_Adjacent_To(this MUnit the_unit, MTile the_tile) => the_unit.s_Locator.is_Adjacent_To(the_tile);
        public static Possible<Index2D> s_Possible_Position(this MUnit the_unit) => the_unit.s_Locator.s_Possible_Position;
        public static Possible<DStuff> s_Loot(this MUnit the_unit) => the_unit.s_Inventory_Content().and(the_unit.s_Remains());
        public static Possible<DStuff> s_Inventory_Content(this MUnit the_unit) => the_unit.s_Inventory.s_Content;
        
        public static ICell<int> s_Cell_of_Current_Hitpoints(this MUnit the_unit) => the_unit.s_Health.s_Current_Hitpoints_Cell;
        public static ICell<bool> s_Cell_of_can_Move(this MUnit the_unit) => the_unit.s_Locator.s_Can_Move_Cell;
        public static ICell<bool> s_Cell_of_can_Fire(this MUnit the_unit) => the_unit.s_Weapon.s_Can_Fire_Cell;
        public static ICell<Possible<DStuff>> s_Cell_of_Inventory_Contents(this MUnit the_unit) => the_unit.s_Inventory.s_Stuffs_Cell;
        public static ICell<int> s_Cell_of_Moves_Left(this MUnit the_unit) => the_unit.s_Locator.s_Moves_Left_Cell;
        public static ICell<bool> s_Cell_of_Is_Docked(this MUnit the_unit) => the_unit.s_Locator.s_Is_Docked_Cell;
        public static ICell<bool> s_Cell_of_can_Deploy(this MUnit the_unit, int the_bay_slot_index) => the_unit.s_Possible_Bay.s_can_Deploy_Cell(the_bay_slot_index);

        public static IStream<Movement> Moved(this MUnit the_unit) => the_unit.s_Locator.Moved;
        public static IStream<Firing> Fired(this MUnit the_unit) => the_unit.s_Weapon.Fired;
        public static IStream<TheVoid> Been_Destroyed(this MUnit the_unit) => the_unit.s_Destructor.Destructed;
        
        public static Possible<MUnitAction> s_possible_Action_For(this MUnit the_unit, DUnitAction the_action_desc) => the_unit.s_Actions_Container.s_possible_Action_For(the_action_desc);
        public static Possible<UnitCommand> s_Regular_Command_At(this MUnit the_unit, MTile the_tile) => the_unit.s_Actions_Container.s_Regular_Command_At(the_tile);
        public static bool is_At_a_Bay(this MUnit the_unit, out MBay the_bay) => the_unit.s_Locator.is_At_a_Bay(out the_bay);
        public static bool is_At_a_Tile(this MUnit the_unit, out MTile the_tile) => the_unit.s_Locator.is_At_a_Tile(out the_tile);
        public static MBay must_be_At_a_Bay(this MUnit the_unit) => the_unit.s_Locator.must_be_At_a_Bay();
        public static MTile must_be_At_a_Tile(this MUnit the_unit) => the_unit.s_Locator.must_be_At_a_Tile();
        public static bool is_Adjacent_To(this MUnit the_unit, MStructure the_structure) => the_unit.s_Locator.is_Adjacent_To(the_structure);
        public static bool has_an_Empty_Bay_Slot(this MUnit the_unit, out MBaySlot the_bay_slot) => the_unit.s_Possible_Bay.has_an_Empty_Slot(out the_bay_slot);
        public static bool has_a_Bay(this MUnit the_unit, out MBay the_bay) => the_unit.s_Possible_Bay.has_a_Value(out the_bay);
        public static bool is_Within_the_Range_Of(this MUnit the_unit, MWeapon the_weapon) => the_unit.s_Locator.is_Adjacent_To(the_weapon.s_Owner.s_Locator);
        public static bool is_Hostile_Towards(this MUnit the_unit, MUnit the_other_unit) => the_other_unit.s_Faction.Is_Hostile_Towards(the_unit.s_Faction);
        public static bool Belongs_To(this MUnit the_unit, MFaction the_faction) => the_unit.s_Faction == the_faction;
        public static bool can_Move_To(this MUnit the_unit, MTile the_destination) => the_unit.s_Locator.can_Move_To(the_destination);
        public static bool can_Dock_To(this MUnit the_unit, MBaySlot the_destination) => the_unit.s_Locator.can_Dock_To(the_destination);
        public static bool can_Interact_With_a_Structure_At(this MUnit the_unit, MTile the_tile, out MStructure the_structure) => the_unit.s_Interactor.can_Interact_With_a_Structure_At(the_tile, out the_structure);
        public static bool has_a_Docked_Unit_at(this MUnit the_unit, int the_bay_slot_index, out MUnit the_bay_unit) => the_unit.s_Possible_Bay.has_a_Docked_Unit_at(the_bay_slot_index, out the_bay_unit);
        public static bool can_Deploy_a_Unit_At(this MUnit the_unit, int the_bay_slot_index, MTile the_tile, out MUnit the_docked_unit) => the_unit.s_Possible_Bay.can_Deploy_a_Unit_At(the_bay_slot_index, the_tile, out the_docked_unit);
        public static bool can_Perform_Special_Actions(this MUnit the_unit) => the_unit.has_a_Docker();
        public static bool has_a_Docker(this MUnit the_unit) => the_unit.s_Possible_Docker.has_a_Value();
        public static bool can_Dock_At(this MUnit the_unit, MTile the_tile, out MBaySlot the_bay_slot) => the_unit.s_Possible_Docker.can_Dock_At(the_tile, out the_bay_slot);
    }

    public static class MUnitWriteExtensions
    {
        internal static void Moves_To(this MUnit the_unit, MTile the_destination) => the_unit.s_Locator.Moves_To(the_destination);
        internal static void Docks_To(this MUnit the_unit, MBaySlot the_destination) => the_unit.s_Locator.Docks_To(the_destination);
        internal static void Interacts_With(this MUnit the_unit, MStructure the_structure) => the_unit.s_Interactor.Interacts_With(the_structure);
        internal static void Takes(this MUnit the_unit, DDamage the_damage) => the_unit.s_Health.Takes(the_damage);
        internal static void Destructs(this MUnit the_unit) => the_unit.s_Destructor.Destructs();
        internal static void Takes(this MUnit the_unit, Possible<DStuff> the_loot) => the_unit.s_Inventory.Adds(the_loot);
        internal static void Loots(this MUnit the_unit, MStructure the_structure) => the_unit.s_Looter.Loots(the_structure);
    }
}