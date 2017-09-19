using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Weapon;
using static Lanski.Structures.Semantics;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MUnit
    {
        internal MUnit(int the_id, DUnit the_desc, MUnitLocation the_initial_location, MGame the_game, SignalGuard the_signal_guard)
        {
            its_id = the_id;
            its_type = the_desc.s_Type;
            its_name = $"{its_type.s_Name} {its_id}";
            its_faction = the_desc.s_Faction;

            its_mover             = new MMover(this, the_initial_location, the_signal_guard);
            its_weapon            = new MWeapon(this, the_signal_guard);
            its_health            = new MHealth(this, the_signal_guard);
            its_inventory         = new MInventory(the_desc.s_Inventory_Content, the_signal_guard);
            its_looter            = new MLooter(this);
            its_interactor        = new MInteractor(this, the_game);
            its_destructor        = new MDestructor(this, the_signal_guard);
            its_possible_docker   = MDocker.From(this);
            its_possible_bay      = MBay.From(this, the_signal_guard);
            its_actions_container = new MActionsContainer(this);
        }

        public string        s_Name     => its_name;
        public int           s_Id       => its_id;
        public MUnitType     s_Type     => its_type;
        public MFaction      s_Faction  => its_faction;
        public MWeapon       s_Weapon   => its_weapon;
        public bool          can_Exit   => its_type.can_Exit;
        public bool          can_Dock   => its_type.can_Dock;
        public MUnitLocation s_Location => its_mover.s_Location;
        public Possible<DStuff> s_Inventory_Content => its_inventory.s_Stuff;
        public Possible<Index2D> s_Position => its_mover.s_Position;

        public bool is_Alive  => its_health.is_Normal;
        public bool can_Move  => its_mover.can_Move;
        public bool is_Docked => its_mover.s_Is_Docked;


        public MWeaponType s_Weapon_Type => its_type.s_Weapon_Type;
        public int s_Total_Moves => its_type.s_Total_Moves;
        public int s_Total_Hit_Points => its_type.s_Total_Hit_Points;
        public int s_Bay_Size => its_type.s_Bay_Size;
        public MChassisType s_Chassis_Type => its_type.s_Chassis_Type;
        public Possible<DStuff> s_Loot => its_type.s_Loot; 

        public ICell<HealthState> s_Health_States_Cell => its_health.s_States_Cell;
        public ICell<bool> s_can_Move_Cell => its_mover.s_Can_Move_Cell;
        public ICell<bool> s_can_Fire_Cell => its_weapon.s_Can_Fire_Cell; 
        public ICell<Possible<DStuff>> s_Inventory_Contents_Cell => its_inventory.s_Stuffs_Cell;
        public ICell<int> s_Moves_Left_Cell => its_mover.s_Moves_Left_Cell;
        public ICell<bool> s_Is_Docked_Cell => its_mover.s_Is_Docked_Cell;
        public IStream<Movement> Moved => its_mover.Moved;
        public IStream<TheVoid> Destructed => its_destructor.Destructed;

        public Possible<MUnitAction> s_possible_Action_For(DUnitAction the_action_desc) => its_actions_container.s_possible_Action_For(the_action_desc);
        public Possible<UnitCommand> s_Regular_Command_At(MTile the_tile) => its_actions_container.s_Regular_Command_At(the_tile);

        public bool is_At_a_Tile() => its_mover.is_At_a_Tile();
        public bool is_At_a_Tile(out MTile the_tile) => its_mover.is_At_a_Tile(out the_tile);
        public bool is_At_a_Bay(out MBay the_tile) => its_mover.is_At_a_Bay(out the_tile); 
        public MTile must_be_At_a_Tile() => its_mover.must_be_At_a_Tile();
        public bool is_Adjacent_To(MStructure the_structure) => its_mover.is_Adjacent_To(the_structure);

        public ICell<bool> s_can_Deploy_Cell(int the_bay_slot_index) => this.has_a_Bay(out var the_bay) ? the_bay.s_can_Deploy_Cell(the_bay_slot_index) : Cell.From(false);

        public bool has_a_docked_unit_at(int the_bay_slot_index, out MUnit the_bay_unit) => this.its_possible_docked_unit_at(the_bay_slot_index).has_a_Value(out the_bay_unit);
        public bool has_an_empty_bay_slot(out MUnitLocation the_bay_slot) => semantic_resets(out the_bay_slot) && this.has_a_Bay(out var the_bay) && the_bay.has_an_Empty_Slot(out the_bay_slot);
        public bool has_a_Bay(out MBay the_bay) => its_possible_bay.has_a_Value(out the_bay);
        public bool is_Within_the_Range_Of(MWeapon the_other_weapon) => its_mover.is_Adjacent_To(the_other_weapon.s_Owner);
        public bool is_Hostile_Towards(MUnit the_other_unit) => the_other_unit.s_Faction.Is_Hostile_Towards(its_faction);
        public bool Belongs_To(MFaction the_requested_faction) => its_faction == the_requested_faction;

        public bool can_Move_To(MUnitLocation the_destination) => its_mover.can_Move_To(the_destination);
        public bool can_Move_To(MTile the_tile, out MUnitLocation the_tiles_location) => its_mover.can_Move_To(the_tile, out the_tiles_location);
        public bool can_Interact_With_a_Structure_At(MTile the_tile, out MStructure the_target_structure) =>
            its_interactor.can_Interact_With_a_Structure_At(the_tile, out the_target_structure)
        ;
        public bool can_Deploy(int the_bay_slot_index, MTile the_tile, out MUnit the_docked_unit, out MUnitLocation the_location) =>
            semantic_resets(out the_location, out the_docked_unit) &&
            this.has_a_docked_unit_at(the_bay_slot_index, out the_docked_unit) && 
            the_docked_unit.can_Move_To(the_tile, out the_location)
        ;
        
        public bool can_Perform_Special_Actions() => this.has_a_Docker(); 
        public bool has_a_Docker() => its_possible_docker.has_a_Value();

        public bool can_Dock_At(MTile the_tile, out MUnitLocation the_dock_location) =>
            semantic_resets(out the_dock_location) &&
            this.it_has_a_docker(out var the_docker) && 
            the_docker.can_Dock_At(the_tile, out the_dock_location)
        ;

        internal void Moves_To(MUnitLocation the_destination) => its_mover.Moves_To(the_destination);
        internal void Interacts_With(MStructure the_structure) => its_interactor.Interacts_With(the_structure);
        internal void Takes(DDamage the_damage) => its_health.Takes(the_damage);
        internal void Destructs() => its_destructor.Destructs();
        internal void Takes(Possible<DStuff> the_loot) => its_inventory.Adds(the_loot);
        internal void Loots(MStructure the_structure) => its_looter.Loots(the_structure);
        internal void Finishes_the_Turn()
        {
            its_weapon.Finishes_the_Turn();
            its_mover.Finishes_the_Turn();
        }
        
        private bool it_has_a_docker(out MDocker the_docker) => its_possible_docker.has_a_Value(out the_docker);
        private Possible<MUnit> its_possible_docked_unit_at(int the_bay_slot_index) => this.has_a_Bay(out var the_bay) ? the_bay.s_possible_Unit_At(the_bay_slot_index) : Possible.Empty<MUnit>();

        private readonly int its_id;
        private readonly string its_name;
        private readonly MUnitType its_type;
        private readonly MFaction its_faction;
        private readonly MWeapon its_weapon;
        private readonly MMover its_mover;
        private readonly Possible<MDocker> its_possible_docker;
        private readonly Possible<MBay> its_possible_bay;
        private readonly MHealth its_health;
        private readonly MInventory its_inventory;
        private readonly MInteractor its_interactor;
        private readonly MDestructor its_destructor;
        private readonly MLooter its_looter;
        private readonly MActionsContainer its_actions_container;
    }
}