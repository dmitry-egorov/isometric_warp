using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class LocationModel
    {
        public Or<TileModel, BayModel> Owner { get; }
        public Slot<UnitModel> Possible_Occupant { get; private set; }
        
        public static class Create
        {
            public static LocationModel From(BayModel owner_bay) => new LocationModel(owner_bay, Slot.Empty<UnitModel>()); 
            public static LocationModel From(BayModel owner_bay, Slot<UnitModel> possible_unit) => new LocationModel(owner_bay, possible_unit); 
            public static LocationModel From(TileModel owner_tile) => new LocationModel(owner_tile, Slot.Empty<UnitModel>()); 
            public static LocationModel From(TileModel owner_tile, Slot<UnitModel> possible_unit) => new LocationModel(owner_tile, possible_unit); 
        }

        public LocationModel(Or<TileModel, BayModel> owner, Slot<UnitModel> possible_unit)
        {
            Owner = owner;
            Possible_Occupant = possible_unit;
        }

        public bool Is_Adjacent_To(LocationModel other) => 
            other.Is_a_Tile(out var other_owner_tile)
            && Is_Adjacent_To(other_owner_tile)
        ;
        
        public bool Is_Adjacent_To(TileModel tile) => 
            Is_a_Tile(out var own_tile)
            && own_tile.Is_Adjacent_To(tile)
        ;
        
        public bool Is_Adjacent_To(StructureModel structure) => Is_Adjacent_To(structure.Location);
        public Slot<TileModel> As_a_Tile() => Owner.As_a_T1();
        public bool Is(TileModel tile) => Is_a_Tile(out var own_tile) && own_tile == tile;
        public bool Is_a_Tile(out TileModel tile) => Owner.Is_a_T1(out tile);
        public bool Is_a_Bay(out BayModel bay) => Owner.Is_a_T2(out bay);
        public TileModel Must_Be_a_Tile() => Owner.Must_Be_a_T1();
        public BayModel Must_Be_a_Bay() => Owner.Must_Be_a_T2();
        
        
        public bool Has_a_Unit() => Possible_Occupant.Has_a_Value();
        public bool Has_a_Unit(out UnitModel unit) => Possible_Occupant.Has_a_Value(out unit);
        public bool Is_Occupied() => !Is_Empty();
        public bool Is_Empty() => Possible_Occupant.Has_Nothing();

        internal void Set_the_Occupant_To(UnitModel unit)
        {
            if (Is_Occupied())
                throw new InvalidOperationException("Can't set a unit on an occupied slot.");
            
            Set_Occupant(unit.As_a_Slot());
        }

        internal void Reset_the_Occupant()
        {
            if (Is_Empty())
                throw new InvalidOperationException("Can't reset a unit on an empty slot.");

            Set_Occupant(Slot.Empty<UnitModel>());
        }

        private void Set_Occupant(Slot<UnitModel> possible_occupant)
        {
            Possible_Occupant = possible_occupant;
        }

    }
}