using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class UnitModel
    {
        public readonly UnitType Type;
        
        public readonly HealthModel Health;
        public readonly ChassisModel Chassis;
        public readonly WeaponModel Weapon;
        public readonly InventoryModel Inventory;
        public readonly Slot<BayModel> Possible_Bay;

        public LocationModel Location => Chassis.Location;

        public IStream<UnitMoved> Stream_Of_Movements => _stream_of_movements;
        public IStream<UnitDestroyed> Signal_Of_the_Destruction => _signal_of_the_destruction;
        
        public Faction Faction { get; }
        public bool Is_Alive => Health.Is_Alive;
        public bool Is_Dead => Health.Is_Dead;

        public IStream<MothershipExited> Stream_Of_Exits => _stream_of_exits;

        public UnitModel(UnitType unit_type, Faction faction, InventoryContent? inventory, LocationModel initial_location)
        {
            Type = unit_type;
            Faction = faction;

            Weapon = WeaponModel.From(Type, this);
            Health = HealthModel.From(Type, this);
            Inventory = InventoryModel.From(inventory);

            Chassis = new ChassisModel(initial_location, this.Type);
            Possible_Bay = Type.Has_a_Bay(out var size) ? new BayModel(size, this).As_a_Slot() : Slot.Empty<BayModel>();
        }

        public bool Is_At(TileModel the_tile) => Location.Is(the_tile); 
        public bool Can_Move_To(TileModel the_destination) => Chassis.Can_Move_To(the_destination); 
        public bool Is_Adjacent_To(StructureModel the_structure) => Location.Is_Adjacent_To(the_structure);
        public bool Has_a_Bay(out BayModel bay) => Possible_Bay.Has_a_Value(out bay);
        public TileModel Must_Be_At_a_Tile() => Location.Must_Be_a_Tile();
        public bool Can_Interact_With(StructureModel the_structure) => 
            Is_Adjacent_To(the_structure) && 
            (
                the_structure.Is_an_Exit() && Type == UnitType.Mothership || 
                the_structure.Is_a_Debris()
            )
        ;

        public void Move_To(TileModel destination)
        {
            Can_Move_To(destination).Otherwise_Throw("Can't move the unit to the destination");

            var new_location = destination.Must_Have_a_Location(); //Shouldn't be able to move to a tile without a location 
            var old_location = Location;

            Chassis.Update_the_Location(new_location);
            old_location.Reset_the_Occupant();
            new_location.Set_the_Occupant_To(this);
            
            Send_Movement(old_location, new_location);
        }

        public void Interact_With(StructureModel the_structure)
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
        }

        internal void Take(InventoryContent? the_loot) => Inventory.Add(the_loot);
        
        private void Destruct()
        {
            var loot = Inventory.Content;

            Location.Reset_the_Occupant();
            
            if (Location.Is_a_Tile(out TileModel tile_model))
                tile_model.Create_Debris(loot);

            if (Location.Is_a_Bay(out var bay))
                bay.Owner.Take(loot);

            Send_Destruction();
        }

        private void Send_Destruction() => _signal_of_the_destruction.Next(new UnitDestroyed(this, Location));
        private void Send_Movement(LocationModel old_location, LocationModel new_location) => _stream_of_movements.Next(new UnitMoved(this, old_location, new_location));

        private readonly Stream<UnitMoved> _stream_of_movements = new Stream<UnitMoved>();
        private readonly Signal<UnitDestroyed> _signal_of_the_destruction = new Signal<UnitDestroyed>();
        private readonly Stream<MothershipExited> _stream_of_exits = new Stream<MothershipExited>();

    }
}