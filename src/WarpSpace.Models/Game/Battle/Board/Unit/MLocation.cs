using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MLocation
    {
        public MLocation(Or<MTile, MBay> owner): this(owner, Possible.Empty<MUnit>()){}
        public MLocation(Or<MTile, MBay> owner, Possible<MUnit> possible_unit)
        {
            _owner = owner;
            _possible_occupant = possible_unit;
        }

        public bool is_Adjacent_To(MLocation other) => 
            other.Is_a_Tile(out var other_owner_tile)
            && is_Adjacent_To(other_owner_tile)
        ;
        
        public bool is_Adjacent_To(MTile tile) => 
            (Is_a_Tile(out var own_tile) || Is_a_Bay(out var bay) && bay.s_Owner.is_At_a_Tile(out own_tile))//TODO: check if this is correct. Maybe created a separate method
            && own_tile.Is_Adjacent_To(tile)
        ;
        
        public bool is_Adjacent_To(MStructure structure) => is_Adjacent_To(structure.Location);
        public Possible<MTile> As_a_Tile() => _owner.As_a_T1();
        public bool Is(MTile tile) => Is_a_Tile(out var own_tile) && own_tile == tile;
        public bool Is_a_Tile() => _owner.Is_a_T1();
        public bool Is_a_Tile(out MTile tile) => _owner.Is_a_T1(out tile);
        public bool is_a_Bay() => _owner.Is_a_T2();
        public bool Is_a_Bay(out MBay bay) => _owner.Is_a_T2(out bay);
        public MTile Must_Be_a_Tile() => _owner.Must_Be_a_T1();
        public MBay Must_Be_a_Bay() => _owner.Must_Be_a_T2();
        public void Must_Be_Empty() => Is_Empty().Must_Be_True();
        public bool Has_a_Unit() => _possible_occupant.Has_a_Value();
        public bool Has_a_Unit(out MUnit unit) => _possible_occupant.Has_a_Value(out unit);
        public bool Is_Occupied() => !Is_Empty();
        public bool Is_Empty() => _possible_occupant.Has_Nothing();

        internal void Sets_the_Occupant_To(MUnit unit)
        {
            this.Is_Empty().Otherwise_Throw("Can't set a unit on an occupied location.");
            
            Set_Occupant(unit.As_a_Possible_Value());
        }

        internal void Resets_the_Occupant()
        {
            this.Is_Occupied().Otherwise_Throw("Can't reset a unit on an empty location.");

            Set_Occupant(Possible.Empty<MUnit>());
        }

        private void Set_Occupant(Possible<MUnit> possible_occupant)
        {
            _possible_occupant = possible_occupant;
        }
        
        private readonly Or<MTile, MBay> _owner;
        private Possible<MUnit> _possible_occupant;
    }
}