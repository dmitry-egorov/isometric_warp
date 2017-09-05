using System;
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
        public readonly UnitType Type;

        public readonly MWeapon Weapon;
        
        public Faction Faction { get; }
        public bool Is_Docked => Location.Is_a_Bay();
        public bool Is_Alive => Health.Is_Alive;
        public bool Is_Dead => Health.Is_Dead;
        public int Total_Hit_Points => Health.Total_Hit_Points;
        public ICell<int> Current_Hit_Points_Cell => Health.Current_Hit_Points_Cell;
        public ICell<Possible<InventoryContent>> Inventory_Content_Cell => Inventory.Content_Cell;

        public IStream<MothershipExited> Stream_Of_Exits => _stream_of_exits;
        public IStream<UnitMoved> Stream_Of_Movements => _stream_of_movements;
        public IStream<bool> Stream_Of_Dockings { get; }
        public IStream<MUnit> Signal_Of_the_Destruction => _signal_of_the_destruction;
        
        public MUnit(UnitType unit_type, Faction faction, Possible<InventoryContent> inventory, MLocation initial_location)
        {
            Type = unit_type;
            Faction = faction;

            Weapon = MWeapon.From(Type, this);
            Health = MHealth.From(Type, this);
            Inventory = MInventory.From(inventory);

            Chassis = new MChassis(initial_location, Type);
            Possible_Bay = Type.Has_a_Bay(out var size) ? new MBay(size, this).As_a_Slot() : Possible.Empty<MBay>();
            Stream_Of_Dockings = Stream_Of_Movements.Select(x => x.Destination.Is_a_Bay());
        }

        public Possible<MTile> Location_As_a_Tile() => Location.As_a_Tile(); 
        public bool Is_At(MTile the_tile) => Location.Is(the_tile); 
        public bool Is_At_a_Tile(out MTile the_tile) => Location.Is_a_Tile(out the_tile); 
        public bool Can_Move_To(MTile the_destination) => Chassis.Can_Move_To(the_destination);
        public bool Is_Adjacent_To(MUnit the_unit) => Location.Is_Adjacent_To(the_unit.Location);
        public bool Is_Adjacent_To(MStructure the_structure) => Location.Is_Adjacent_To(the_structure);
        public bool Has_a_Bay(out MBay the_bay) => Possible_Bay.Has_a_Value(out the_bay);
        public MBay Must_Have_a_Bay() => Possible_Bay.Must_Have_a_Value();
        public MBay Must_Be_At_a_Bay() => Location.Must_Be_a_Bay();
        public MTile Must_Be_At_a_Tile() => Location.Must_Be_a_Tile();
        public bool Is_At_a_Tile() => Location.Is_a_Tile(); 
        public bool Is_At_a_Bay() => Location.Is_a_Bay(); 

        public bool Can_Interact_With(MStructure the_structure) => 
            Is_Adjacent_To(the_structure) && 
            (
                the_structure.Is_an_Exit() && Type == UnitType.Mothership || 
                the_structure.Is_a_Debris()
            )
        ;

        public void Move_To(MTile destination)
        {
            Can_Move_To(destination).Otherwise_Throw("Can't move the unit to the destination");

            var new_location = destination.Must_Have_a_Location(); //Shouldn't be able to move to a tile without a location 
            var old_location = Location;

            Chassis.Update_the_Location(new_location);
            old_location.Reset_the_Occupant();
            new_location.Set_the_Occupant_To(this);
            
            Send_Movement(old_location, new_location);
        }

        public void Interact_With(MStructure the_structure)
        {
            Can_Interact_With(the_structure).Otherwise_Throw("Can't interact with the structure");
            
            if (the_structure.Is_an_Exit())
            {
                _stream_of_exits.Next(new MothershipExited());
            }
            else if (the_structure.Is_a_Debris(out var debris))
            {
                Take(debris.Loot);
                the_structure.Destroy();
            }
            else 
            {
                throw new InvalidOperationException("Can't interact");
            }
        }

        public void Take(DamageDescription the_damage)
        {
            Health.Take(the_damage);
            if (Is_Dead)
                Destruct();
            
            void Destruct()
            {
                var loot = Inventory.Content;

                Location.Reset_the_Occupant();
            
                if (Location.Is_a_Tile(out MTile tile_model))
                    tile_model.Create_Debris(loot);

                if (Location.Is_a_Bay(out var bay))
                    bay.Owner.Take(loot);

                Send_Destruction();
            }
        }
        
        private MLocation Location => Chassis.Location;
        private readonly MChassis Chassis;
        private readonly Possible<MBay> Possible_Bay;
        private readonly MHealth Health;
        private readonly MInventory Inventory;

        private void Take(Possible<InventoryContent> the_loot) => Inventory.Add(the_loot);

        private void Send_Destruction() => _signal_of_the_destruction.Next(this);
        private void Send_Movement(MLocation old_location, MLocation new_location) => _stream_of_movements.Next(new UnitMoved(this, old_location, new_location));

        private readonly Stream<UnitMoved> _stream_of_movements = new Stream<UnitMoved>();
        private readonly Signal<MUnit> _signal_of_the_destruction = new Signal<MUnit>();
        private readonly Stream<MothershipExited> _stream_of_exits = new Stream<MothershipExited>();

    }
}