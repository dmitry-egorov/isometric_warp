using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Semantics;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public class MTile
    {
        public MTile(Index2D position, DTile desc, MUnitFactory the_board, SignalGuard the_signal_guard)
        {
            its_position = position;
            this.the_signal_guard = the_signal_guard;

            its_landscape = new MLandscape(desc.Type);
            its_sites_cell = new GuardedCell<TileSite>(it_creates_its_site(desc.Initial_Site), the_signal_guard);

            TileSite it_creates_its_site(DTileSite site)
            {
                if (site.Is_a_Structure(out var structure_description))
                    return it_create_a_structure_site(structure_description);

                var location = it_creates_a_location();

                if (site.Is_Empty())
                    return location;

                var unit_desc = site.Must_Be_a_Unit();
                the_board.Creates_a_Unit(unit_desc, location);
                
                return location;
            }
        }
        
        public void Init(FullNeighbourhood2D<MTile> the_neighbourhood)
        {
            its_neighbors = the_neighbourhood;
        }
        
        public Index2D s_Position => its_position;

        public FullNeighbourhood2D<MTile> s_Neighbors => its_neighbors;

        public ICell<TileSite> s_Sites_Cell => its_sites_cell;
        public MLandscapeType s_Landscape_Type => its_landscape.s_Type;

        public bool has_a_unit_with_an_empty_bay_slot(out MUnitLocation the_bay_slot) => 
            semantic_resets(out the_bay_slot) && 
            this.has_a_Unit(out var the_unit) && 
            the_unit.has_an_empty_bay_slot(out the_bay_slot)
        ;

        public bool has_a_Location(out MUnitLocation unit) => its_site.is_a_Location(out unit);
        public bool has_a_Unit(out MUnit unit) => its_site.has_a_Unit(out unit);
        public Passability s_Passability_With(MChassisType the_chassis_type) => its_landscape.s_Passability_With(the_chassis_type);
        public bool is_Passable_By(MChassisType chassis_type) => its_landscape.is_Passable_With(chassis_type);
        public bool is_Adjacent_To(MTile destination) => its_position.Is_Adjacent_To(destination.its_position);
        public Direction2D s_Direction_To(MTile destination) => its_position.Direction_To(destination.its_position);
        public bool has_a_Structure(out MStructure structure) => its_site.is_a_Structure(out structure);

        internal void Creates_a_Debris_with(Possible<DStuff> inventory_content)
        {
            var the_debris = DStructure.Create.Debris(TileHelper.GetOrientation(its_position), inventory_content);
            its_structure_becomes(the_debris);
        }
        
        internal void Removes_its_Structure()
        {
            its_site.is_a_Structure().Otherwise_Throw("Can't reset structure on a tile, since the tile doesn't conatin a structure");
            its_site = it_creates_an_empty_site();
        }

        private void its_structure_becomes(DStructure the_structure_desc)
        {
            its_site = it_create_a_structure_site(the_structure_desc);
        }

        private TileSite it_creates_an_empty_site() => it_creates_a_location();
        private TileSite it_create_a_structure_site(DStructure structure_description) => new MStructure(structure_description, this);
        private MUnitLocation it_creates_a_location() => new MUnitLocation(this, the_signal_guard);

        private TileSite its_site
        {
            get => its_sites_cell.s_Value;
            set => its_sites_cell.s_Value = value;
        }

        private readonly GuardedCell<TileSite> its_sites_cell;
        private readonly MLandscape its_landscape;
        private readonly Index2D its_position;
        private readonly SignalGuard the_signal_guard;
        private FullNeighbourhood2D<MTile> its_neighbors;

    }
}