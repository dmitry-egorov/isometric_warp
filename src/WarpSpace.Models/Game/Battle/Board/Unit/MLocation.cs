using System;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MLocation: IEquatable<MLocation>
    {
        public MLocation(Or<MTile, MBay> owner): this(owner, Possible.Empty<MUnit>()) {}
        public MLocation(Or<MTile, MBay> owner, Possible<MUnit> possible_unit)
        {
            s_owner = owner;
            s_unit = possible_unit;
        }

        public bool is_Adjacent_To(MLocation the_other) => 
            it.is_a_Tile(out var its_tile) 
            && the_other.is_a_Tile(out var the_others_tile) 
            && its_tile.is_Adjacent_To(the_others_tile)
        ;

        public Possible<MUnit> s_Unit => it.s_unit;
        public MTile s_Tile => it.s_tile;

        public bool is_Passable_By(ChassisType the_chassis_type) => it.is_a_Bay() || it.is_a_Tile(out var the_tile) && the_tile.is_Passable_By(the_chassis_type); 
        public bool is_Accessible_From(MLocation the_other_location) => it.s_tile.is_Adjacent_To(the_other_location.s_Tile);
        public bool is_Adjacent_To(MStructure structure) => is_a_Tile(out var own_tile) && own_tile.is_Adjacent_To(structure.Location);
        public void must_be_Empty() => it.is_Empty().Must_Be_True();
        public bool is_Occupied() => !it.is_Empty();
        public bool has_a_Unit() => is_Occupied();
        public bool has_a_Unit(out MUnit unit) => it.s_unit.has_a_Value(out unit);
        public bool is_Empty() => it.s_unit.Has_Nothing();
        
        public Possible<MTile> as_a_Tile() => it.s_owner.as_a_T1();
        public bool @is(MTile tile) => it.is_a_Tile(out var own_tile) && own_tile == tile;
        public bool is_a_Tile() => it.s_owner.is_a_T1();
        public bool is_a_Tile(out MTile tile) => it.s_owner.is_a_T1(out tile);
        public bool is_a_Bay() => it.s_owner.is_a_T2();
        public bool is_a_Bay(out MBay bay) => it.s_owner.is_a_T2(out bay);
        public MTile must_be_a_Tile() => it.s_owner.must_be_a_T1();
        public MBay must_be_a_Bay() => it.s_owner.must_be_a_T2();
        
        public bool Equals(MLocation other) => ReferenceEquals(it, other);

        internal void s_Occupant_Becomes(MUnit unit)
        {
            it.is_Empty().Otherwise_Throw("Can't set a unit on an occupied location.");
            
            it.s_occupant_becomes(unit);
        }

        internal void Becomes_Empty()
        {
            it.is_Occupied().Otherwise_Throw("Can't reset a unit on an empty location.");

            it.s_occupant_becomes(Possible.Empty<MUnit>());
        }

        private void s_occupant_becomes(Possible<MUnit> possible_occupant) => it.s_unit = possible_occupant;

        private MLocation it => this;
        
        private MTile s_tile => it.is_a_Tile(out var self) ? self : it.must_be_a_Bay().s_Owner.must_be_At_a_Tile();
        
        private readonly Or<MTile, MBay> s_owner;
        private Possible<MUnit> s_unit;

    }
}