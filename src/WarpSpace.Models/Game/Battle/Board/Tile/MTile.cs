using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public class MTile: IEquatable<MTile>
    {
        public MTile(Index2D position, DTile desc, SignalGuard the_signal_guard)
        {
            its_position = position;

            its_landscape = new MLandscape(desc.s_Type);
            its_sites_cell = new GuardedCell<MTileOccupant>(TheVoid.Instance, the_signal_guard);
        }

        public void Init(FullNeighbourhood2D<MTile> the_neighbourhood)
        {
            its_neighbors = the_neighbourhood;
        }
        
        public Index2D s_Position => its_position;

        public FullNeighbourhood2D<MTile> s_Neighbors => its_neighbors;

        public ICell<MTileOccupant> s_Sites_Cell => its_sites_cell;
        public MLandscapeType s_Landscape_Type => its_landscape.s_Type;

        public bool is_Empty() => its_occupant.is_None();
        public bool has_a_Unit(out MUnit unit) => its_occupant.is_a_Unit(out unit);
        public Passability s_Passability_With(MChassisType the_chassis_type) => its_landscape.s_Passability_With(the_chassis_type);
        public bool is_Passable_By(MChassisType chassis_type) => its_landscape.is_Passable_With(chassis_type);
        public bool is_Adjacent_To(MTile destination) => its_position.is_Adjacent_To(destination.its_position);
        public Direction2D s_Direction_To(MTile destination) => its_position.Direction_To(destination.its_position);
        public bool has_a_Structure(out MStructure structure) => its_occupant.is_a_Structure(out structure);

        public override string ToString() => its_position.ToString();

        internal void s_Occupant_Becomes(MTileOccupant the_new_occupant) => its_occupant = the_new_occupant;
        internal void s_Occupant_Becomes_Empty() => this.s_Occupant_Becomes(MTileOccupant.Empty);

        internal void Creates_a_Debris_With(DStuff inventory_content)
        {
            var the_debris_desc = DStructure.Create.Debris(TileHelper.GetOrientation(its_position), inventory_content);
            var the_structure = new MStructure(the_debris_desc, this);
            its_occupant = the_structure;
        }

        private MTileOccupant its_occupant
        {
            get => its_sites_cell.s_Value;
            set => its_sites_cell.s_Value = value;
        }

        private readonly MLandscape its_landscape;
        private readonly Index2D its_position;
        private readonly GuardedCell<MTileOccupant> its_sites_cell;

        private FullNeighbourhood2D<MTile> its_neighbors;

        public bool Equals(MTile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return its_position.Equals(other.its_position);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MTile) obj);
        }

        public override int GetHashCode()
        {
            return its_position.GetHashCode();
        }

        public static bool operator ==(MTile t1, MTile t2) => t1 == null && t2 == null || t1 != null && t1.Equals(t2);
        public static bool operator !=(MTile t1, MTile t2) => !(t1 == t2);
    }

    public static class MTileSemanticExtensions
    {
    }
}