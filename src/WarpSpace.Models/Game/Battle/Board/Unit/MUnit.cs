using System.Collections.Generic;
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
        internal MUnit(UnitType the_type, Faction the_faction, Possible<Stuff> possible_stuff, MLocation the_initial_location, SignalGuard the_signal_guard)
        {
            its_type = the_type;
            its_faction = the_faction;

            its_mover             = new MMover(this, the_initial_location, the_signal_guard);
            its_weapon            = new MWeapon(this, the_signal_guard);
            its_health            = new MHealth(this, the_signal_guard);
            its_inventory         = new MInventory(possible_stuff, the_signal_guard);
            its_looter            = new MLooter(this);
            its_interactor        = new MInteractor(this, the_signal_guard);
            its_destructor        = new MDestructor(this, the_signal_guard);
            its_possible_docker   = MDocker.From(this);
            its_possible_bay      = MBay.From(this, the_signal_guard);
            its_actions_container = new MActionsContainer(this);
        }

        public UnitType   s_Type      => its_type;
        public Faction    s_Faction   => its_faction;
        public MWeapon    s_Weapon    => its_weapon;
        public MHealth    s_Health    => its_health;
        public MInventory s_Inventory => its_inventory;
        public MMover     s_Mover     => its_mover;
        public MLooter    s_Looter    => its_looter;
        public Possible<MBay> s_Possible_Bay => its_possible_bay;
        public IReadOnlyList<MUnitAction> s_Regular_Actions => its_actions_container.s_Regular_Actions;
        public MLocation s_Location => its_mover.s_Location;
        public MTile s_Tile => its_mover.s_Tile; 
        public Possible<Stuff> s_Inventory_Content => its_inventory.s_Stuff;

        public bool is_Docked => its_mover.is_Docked;
        public bool is_Alive => its_health.is_Normal;
        public bool can_Move => its_mover.can_Move;

        public ICell<HealthState> s_Health_States_Cell => its_health.s_States_Cell;
        public ICell<bool> s_can_Move_Cell => its_mover.s_can_Move_Cell;

        public ICell<Possible<Stuff>> s_Inventory_Contents_Cell => its_inventory.s_Stuffs_Cell;
        public IStream<Movement> s_Movements_Stream => its_mover.s_Movements_Stream;
        public IStream<bool> s_Dock_States_Stream => its_mover.s_Dock_States_Stream;
        public IStream<TheVoid> s_Destruction_Signal => its_destructor.s_Destruction_Signal;
        public IStream<TheVoid> s_Exit_Signal => its_interactor.s_Exit_Signal;
        
        public Possible<MUnitAction> s_possible_action_for(DUnitAction the_action_desc) => its_actions_container.s_possible_action_for(the_action_desc);

        public Possible<MTile> s_Location_As_a_Tile() => its_mover.s_Location_As_a_Tile(); 
        public bool is_At(MTile the_tile) => its_mover.is_At(the_tile); 
        public bool is_At_a_Tile() => its_mover.is_At_a_Tile();
        public bool is_At_a_Tile(out MTile the_tile) => its_mover.is_At_a_Tile(out the_tile);
        public bool is_At_a_Bay() => its_mover.is_At_a_Bay(); 
        public bool is_At_a_Bay(out MBay the_tile) => its_mover.is_At_a_Bay(out the_tile); 
        public MTile must_be_At_a_Tile() => its_mover.must_be_At_a_Tile();
        public bool is_Adjacent_To(MStructure the_structure) => its_mover.is_Adjacent_To(the_structure);

        public bool has_a_docked_unit_at(int the_bay_slot_index, out MUnit the_bay_unit) => this.s_possible_docked_unit_at(the_bay_slot_index).has_a_Value(out the_bay_unit);
        public bool has_a_docked_unit_at(int the_bay_slot_index) => this.s_possible_docked_unit_at(the_bay_slot_index).has_a_Value();
        public ICell<bool> s_has_a_docked_unit_at_cell(int the_bay_slot_index) => this.has_a_Bay(out var the_bay) ? the_bay.s_has_a_docked_unit_at_cell(the_bay_slot_index) : Cell.From(false);
        public Possible<MUnit> s_possible_docked_unit_at(int the_bay_slot_index) => this.has_a_Bay(out var the_bay) ? the_bay.s_possible_unit_at(the_bay_slot_index) : Possible.Empty<MUnit>();
        public bool has_an_empty_bay_slot(out MLocation the_bay_slot) => semantic_resets(out the_bay_slot) && this.has_a_Bay(out var the_bay) && the_bay.has_an_Empty_Slot(out the_bay_slot);
        public bool has_a_Bay(out MBay the_bay) => its_possible_bay.has_a_Value(out the_bay);
        public MBay must_have_a_Bay() => its_possible_bay.must_have_a_Value();
        public bool is_Within_the_Range_Of(MWeapon the_other_weapon) => its_mover.is_Adjacent_To(the_other_weapon.s_Owner);
        public bool is_Hostile_Towards(MUnit the_other_unit) => its_faction.Is_Hostile_Towards(the_other_unit.its_faction);
        public bool s_Faction_is(Faction the_requested_faction) => its_faction == the_requested_faction;
        public bool @is(UnitType the_requested_type) => its_type == the_requested_type;
        public bool is_not(UnitType the_requested_type) => its_type != the_requested_type;

        public bool can_Move_To(MLocation the_destination) => its_mover.can_Move_To(the_destination);
        public bool can_Move_To(MTile the_tile, out MLocation the_tiles_location) => its_mover.can_Move_To(the_tile, out the_tiles_location);
        public bool can_Interact_With_a_Structure_At(MTile the_tile, out MStructure the_target_structure) =>
            its_interactor.can_Interact_With_a_Structure_At(the_tile, out the_target_structure)
        ;
        
        public bool can_Perform_Special_Actions() => this.has_a_Docker(); 
        public bool has_a_Docker() => its_possible_docker.has_a_Value();
        public bool has_a_Docker(out MDocker the_docker) => its_possible_docker.has_a_Value(out the_docker);

        public bool can_Dock_At(MTile the_tile, out MLocation the_dock_location) =>
            semantic_resets(out the_dock_location) &&
            this.has_a_Docker(out var the_docker) && 
            the_docker.can_Dock_At(the_tile, out the_dock_location)
        ;

        internal void Moves_To(MLocation the_destination) => its_mover.Moves_To(the_destination);
        internal void Interacts_With(MStructure the_structure) => its_interactor.Interacts_With(the_structure);
        internal void Takes(Damage the_damage) => its_health.Takes(the_damage);
        internal void Destructs() => its_destructor.Destructs();
        internal void Takes(Possible<Stuff> the_loot) => its_inventory.Adds(the_loot);
        internal void Loots(MStructure the_structure) => its_looter.Loots(the_structure);
        internal void Finishes_the_Turn()
        {
            its_weapon.Finishes_the_Turn();
            its_mover.Finishes_the_Turn();
        }

        private readonly UnitType its_type;
        private readonly Faction its_faction;
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