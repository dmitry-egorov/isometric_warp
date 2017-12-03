using System;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Flow;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{                           
    public struct MTileOccupant : IEquatable<MTileOccupant>
    {   
        public static implicit operator MTileOccupant(MUnit location) => new MTileOccupant(location);
        public static implicit operator MTileOccupant(MStructure structure) => new MTileOccupant(structure);
        public static implicit operator MTileOccupant(TheVoid the_void) => new MTileOccupant(the_void);

        public static readonly MTileOccupant Empty = TheVoid.Instance;

        public MTileOccupant(MUnit unit_slot) { its_variant = unit_slot; }
        public MTileOccupant(MStructure structure) { its_variant = structure; }
        public MTileOccupant(TheVoid the_void) { its_variant = the_void; }

        public bool is_a_Unit(out MUnit unit_slot) => its_variant.is_a_T1(out unit_slot);
        public bool is_a_Structure(out MStructure structure) => its_variant.is_a_T2(out structure);
        public bool is_None() => its_variant.is_a_T3();

        public bool Equals(MTileOccupant other) => its_variant.Equals(other.its_variant);
        
        private readonly Or<MUnit, MStructure, TheVoid> its_variant;

    }
}