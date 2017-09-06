using System;
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
        public MUnit(UnitType the_units_type, Faction the_units_faction, Possible<Stuff> possible_stuff, MLocation the_initial_location, EventsGuard the_events_guard)
        {
            the_type = the_units_type;
            the_faction = the_units_faction;
            the_weapon = new MWeapon(this);
            the_health = new MHealth(this, the_events_guard);
            the_inventory = new MInventory(possible_stuff, the_events_guard);

            the_chassis = new MChassis(the_type, the_initial_location, the_events_guard);
            possible_bay = MBay.From(this);

            the_exit_signal = new GuardedStream<TheVoid>(the_events_guard);
        }

        public UnitType s_Type => the_type;
        public MWeapon s_Weapon => the_weapon;
        public Faction s_Faction => the_faction;
        public MHealth s_Health => the_health;

        public bool is_Docked() => the_chassis.is_Docked();
        public bool is_Alive() => the_health.is_Alive();

        public ICell<Possible<Stuff>> s_Inventory_Contents_Cell() => the_inventory.s_Cell_of_Content();

        public IStream<Movement> s_Movements_Stream() => the_chassis.s_Movements_Stream();
        public IStream<bool> s_Dock_States_Stream() => the_chassis.s_Dock_States_Stream();
        public IStream<TheVoid> s_Destruction_Signal() => the_health.s_Destruction_Signal();
        public IStream<TheVoid> s_Exit_Signal() => the_exit_signal;

        public Possible<MTile> s_Location_As_a_Tile() => the_location().As_a_Tile(); 
        public bool is_At(MTile the_tile) => the_location().Is(the_tile); 
        public bool is_At_a_Tile(out MTile the_tile) => the_location().Is_a_Tile(out the_tile); 
        public bool is_Adjacent_To(MUnit the_unit) => the_location().is_Adjacent_To(the_unit.the_location());
        public bool is_Adjacent_To(MStructure the_structure) => the_location().is_Adjacent_To(the_structure);
        public MTile Must_Be_At_a_Tile() => the_location().Must_Be_a_Tile();
        public bool is_At_a_Tile() => the_location().Is_a_Tile(); 
        public bool is_At_a_Bay() => the_location().is_a_Bay(); 
        public bool Can_Move_To(MTile the_destination) => the_chassis.Can_Move_To(the_destination);
        public bool Has_a_Bay(out MBay the_bay) => possible_bay.Has_a_Value(out the_bay);
        public MBay Must_Have_a_Bay() => possible_bay.Must_Have_a_Value();
        public bool is_Within_the_Range_Of(MWeapon the_other_weapon) => this.is_Adjacent_To(the_other_weapon.s_Owner);
        public bool is_Hostile_Towards(MUnit the_other_unit) => the_faction.Is_Hostile_Towards(the_other_unit.the_faction);
        public bool s_Faction_Is(Faction the_requested_faction) => the_faction == the_requested_faction; 
        private bool is_of(UnitType the_requested_type) => the_type == the_requested_type;

        public bool Can_Interact_With(MStructure the_structure) => 
            this.is_Adjacent_To(the_structure) && 
            (
                the_structure.Is_an_Exit() && this.is_of(a_Mothership) || 
                the_structure.Is_a_Debris()
            )
        ;

        internal void Moves_To(MTile the_destination)
        {
            this.Can_Move_To(the_destination).Otherwise_Throw("Can't move the unit to the destination");

            var the_new_location =
                the_destination.Must_Have_a_Location(); //Shouldn't be able to move to a tile without a location 
            var the_old_location = this.the_location();

            this.Updates_Its_Location_To(the_new_location);
            the_old_location.Resets_the_Occupant();
            the_new_location.Sets_the_Occupant_To(this);
        }

        internal void Interacts_With(MStructure the_structure)
        {
            this.Can_Interact_With(the_structure).Otherwise_Throw("Can't interact with the structure");

            if (the_structure.Is_an_Exit())
            {
                this.Sends_the_Exit_Event();
            }
            else if (the_structure.Is_a_Debris())
            {
                this.Loots(the_structure);
            }
            else
            {
                throw new InvalidOperationException("Can't interact");
            }
        }

        internal void Takes(Damage the_damage)
        {
            the_health.Takes(the_damage);
        }

        internal void Destructs()
        {
            var the_loot = the_inventory.s_content;

            the_location().Resets_the_Occupant();

            if (the_location().Is_a_Tile(out var the_tile))
                the_tile.Creates_a_Debris_With(the_loot);

            if (the_location().Is_a_Bay(out var the_bay))
                the_bay.s_Owner.Takes(the_loot);
        }

        private void Loots(MStructure the_structure)
        {
            var the_debris = the_structure.Must_Be_a_Debris();
            this.Takes(the_debris.s_Loot());
            the_structure.Destructs();
        }

        private void Updates_Its_Location_To(MLocation the_new_location)
        {
            the_chassis.Sets_the_Location_To(the_new_location);
        }

        private void Takes(Possible<Stuff> the_loot)
        {
            the_inventory.Adds(the_loot);
        }

        private void Sends_the_Exit_Event() => the_exit_signal.Next();

        private MLocation the_location() => the_chassis.s_Location();

        private readonly UnitType the_type;
        private readonly Faction the_faction;

        private readonly MWeapon the_weapon;
        private readonly MChassis the_chassis;
        private readonly Possible<MBay> possible_bay;
        private readonly MHealth the_health;
        private readonly MInventory the_inventory;

        private readonly GuardedStream<TheVoid> the_exit_signal;
    }
}