using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Weapon;
using static WarpSpace.Models.Descriptions.UnitType;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MUnit
    {
        internal MUnit(UnitType the_type, Faction the_faction, Possible<Stuff> possible_stuff, MLocation the_initial_location, SignalGuard the_signal_guard)
        {
            s_type = the_type;
            s_faction = the_faction;

            s_chassis = new MChassis(it, the_initial_location, the_signal_guard);
            s_weapon = new MWeapon(it, the_signal_guard);
            s_health = new MHealth(it, the_signal_guard);
            s_inventory = new MInventory(possible_stuff, the_signal_guard);
            s_looter = new MLooter(it);
            s_interactor = new MInteractor(it, the_signal_guard);
            s_destructor = new MDestructor(it, the_signal_guard);

            s_possible_bay = MBay.From(it);
        }

        public UnitType s_Type => it.s_type;
        public Faction s_Faction => it.s_faction;
        public MWeapon s_Weapon => it.s_weapon;
        public MHealth s_Health => it.s_health;
        public MInventory s_Inventory => it.s_inventory;
        public MChassis s_Chassis => it.s_chassis;
        public MLooter s_Looter => it.s_looter;
        public Possible<MBay> s_Possible_Bay => it.s_possible_bay;

        public bool is_Docked => it.s_chassis.is_Docked;
        public bool is_Alive => it.s_health.is_Normal;
        public MLocation s_Location => it.s_chassis.s_Location;
        public Possible<Stuff> s_Inventory_Content => it.s_inventory.s_Stuff;
        public ICell<HealthState> s_Health_States_Cell => it.s_health.s_States_Cell;

        public ICell<Possible<Stuff>> s_Inventory_Contents_Cell => it.s_inventory.s_Stuffs_Cell;
        public IStream<Movement> s_Movements_Stream => it.s_chassis.s_Movements_Stream;
        public IStream<bool> s_Dock_States_Stream => it.s_chassis.s_Dock_States_Stream;
        public IStream<TheVoid> s_Destruction_Signal => it.s_destructor.s_Destruction_Signal;
        public IStream<TheVoid> s_Exit_Signal => it.s_interactor.s_Exit_Signal;

        public MTile s_Tile => it.s_chassis.s_Tile; 
        public Possible<MTile> s_Location_As_a_Tile() => it.s_chassis.s_Location_As_a_Tile(); 
        public bool is_At(MTile the_tile) => it.s_chassis.is_At(the_tile); 
        public bool is_At_a_Tile() => it.s_chassis.is_At_a_Tile();
        public bool is_At_a_Tile(out MTile the_tile) => it.s_chassis.is_At_a_Tile(out the_tile);
        public bool is_At_a_Bay() => it.s_chassis.is_At_a_Bay(); 
        public bool is_At_a_Bay(out MBay the_tile) => it.s_chassis.is_At_a_Bay(out the_tile); 
        public MTile must_be_At_a_Tile() => it.s_chassis.must_be_At_a_Tile();
        public bool is_Adjacent_To(MStructure the_structure) => it.s_chassis.is_Adjacent_To(the_structure);

        public bool has_a_Bay(out MBay the_bay) => it.s_possible_bay.has_a_Value(out the_bay);
        public MBay must_have_a_Bay() => it.s_possible_bay.must_have_a_Value();
        public bool is_Within_the_Range_Of(MWeapon the_other_weapon) => it.s_chassis.is_Adjacent_To(the_other_weapon.s_Owner);
        public bool is_Hostile_Towards(MUnit the_other_unit) => it.s_faction.Is_Hostile_Towards(the_other_unit.it.s_faction);
        public bool s_Faction_is(Faction the_requested_faction) => it.s_faction == the_requested_faction;
        public bool @is(UnitType the_requested_type) => it.s_type == the_requested_type;
        public bool is_not(UnitType the_requested_type) => it.s_type != the_requested_type;

        public bool can_Move() => it.s_chassis.can_Move();
        public bool can_Move_To(MTile the_tile, out MLocation the_tiles_location) => it.s_chassis.can_Move_To(the_tile, out the_tiles_location);
        public bool can_Interact_With_a_Structure_At(MTile the_tile, out MStructure the_target_structure) =>
            it.s_interactor.can_Interact_With_a_Structure_At(the_tile, out the_target_structure)
        ;
        
        public bool can_Perform_Special_Actions() => it.can_Dock_in_general(); 
        public bool can_Dock_in_general() => it.is_not(a_Mothership);

        public bool can_Dock_At(MTile the_tile, out MLocation the_dock_location) => it.s_chassis.can_Dock_At(the_tile, out the_dock_location);
        public bool can_Undock_At(MTile the_target_tile, out MLocation the_target_location) => it.s_chassis.can_Undock_At(the_target_tile, out the_target_location);

        internal void Moves_To(MLocation the_destination) => it.s_chassis.Moves_To(the_destination);
        internal void Interacts_With(MStructure the_structure) => it.s_interactor.Interacts_With(the_structure);
        internal void Takes(Damage the_damage) => it.s_health.Takes(the_damage);
        internal void Destructs() => it.s_destructor.Destructs();
        internal void Takes(Possible<Stuff> the_loot) => it.s_inventory.Adds(the_loot);
        internal void Loots(MStructure the_structure) => it.s_looter.Loots(the_structure);
        internal void Finishes_the_Turn()
        {
            it.s_weapon.Finishes_the_Turn();
            it.s_chassis.Finishes_the_Turn();
        }

        private MUnit it => this;
        private readonly UnitType s_type;
        private readonly Faction s_faction;
        private readonly MWeapon s_weapon;
        private readonly MChassis s_chassis;
        private readonly Possible<MBay> s_possible_bay;
        private readonly MHealth s_health;
        private readonly MInventory s_inventory;
        private readonly MInteractor s_interactor;
        private readonly MDestructor s_destructor;
        private readonly MLooter s_looter;
        
    }
}