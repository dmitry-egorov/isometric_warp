using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MLocation
    {
        public Or<MTile, MBay> Owner { get; }
        
        public ICell<Possible<MUnit>> Possible_Occupant_Cell => _possible_occupant_cell;
        public Possible<MUnit> Possible_Occupant
        {
            get => _possible_occupant_cell.Value;
            private set => _possible_occupant_cell.Value = value;
        }

        public static class Create
        {
            public static MLocation From(MBay owner_bay) => new MLocation(owner_bay, Possible.Empty<MUnit>()); 
            public static MLocation From(MBay owner_bay, Possible<MUnit> possible_unit) => new MLocation(owner_bay, possible_unit); 
            public static MLocation From(MTile owner_tile) => new MLocation(owner_tile, Possible.Empty<MUnit>()); 
            public static MLocation From(MTile owner_tile, Possible<MUnit> possible_unit) => new MLocation(owner_tile, possible_unit); 
        }

        public MLocation(Or<MTile, MBay> owner, Possible<MUnit> possible_unit)
        {
            Owner = owner;
            _possible_occupant_cell = new ValueCell<Possible<MUnit>>(possible_unit);
        }

        public bool Is_Adjacent_To(MLocation other) => 
            other.Is_a_Tile(out var other_owner_tile)
            && Is_Adjacent_To(other_owner_tile)
        ;
        
        public bool Is_Adjacent_To(MTile tile) => 
            Is_a_Tile(out var own_tile)
            && own_tile.Is_Adjacent_To(tile)
        ;
        
        public bool Is_Adjacent_To(MStructure structure) => Is_Adjacent_To(structure.Location);
        public Possible<MTile> As_a_Tile() => Owner.As_a_T1();
        public bool Is(MTile tile) => Is_a_Tile(out var own_tile) && own_tile == tile;
        public bool Is_a_Tile() => Owner.Is_a_T1();
        public bool Is_a_Tile(out MTile tile) => Owner.Is_a_T1(out tile);
        public bool Is_a_Bay(out MBay bay) => Owner.Is_a_T2(out bay);
        public MTile Must_Be_a_Tile() => Owner.Must_Be_a_T1();
        public MBay Must_Be_a_Bay() => Owner.Must_Be_a_T2();
        
        
        public bool Has_a_Unit() => Possible_Occupant.Has_a_Value();
        public bool Has_a_Unit(out MUnit unit) => Possible_Occupant.Has_a_Value(out unit);
        public bool Is_Occupied() => !Is_Empty();
        public bool Is_Empty() => Possible_Occupant.Has_Nothing();

        internal void Set_the_Occupant_To(MUnit unit)
        {
            if (Is_Occupied())
                throw new InvalidOperationException("Can't set a unit on an occupied slot.");
            
            Set_Occupant(unit.As_a_Slot());
        }

        internal void Reset_the_Occupant()
        {
            if (Is_Empty())
                throw new InvalidOperationException("Can't reset a unit on an empty slot.");

            Set_Occupant(Possible.Empty<MUnit>());
        }

        private void Set_Occupant(Possible<MUnit> possible_occupant)
        {
            Possible_Occupant = possible_occupant;
        }

        private readonly ValueCell<Possible<MUnit>> _possible_occupant_cell;
    }
}