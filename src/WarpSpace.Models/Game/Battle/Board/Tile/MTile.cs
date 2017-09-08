using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public class MTile
    {
        public readonly Index2D s_Position;

        public AdjacentRef<MTile> Adjacent { get; private set; }
        
        public ICell<TileSite> s_Sites_Cell => the_sites_cell;

        public bool is_Occupied => Site.is_Occupied;

        public MTile(Index2D position, TileDescription desc, UnitFactory unit_factory, SignalGuard the_signal_guard)
        {
            s_Position = position;
            the_landscape = new MLandscape(desc.Type);

            var site = desc.Initial_Site;
            the_sites_cell = new GuardedCell<TileSite>(Create_Site(), the_signal_guard);

            TileSite Create_Site()
            {
                if (site.Is_a_Structure(out var structure_description))
                    return Create_Structure_Site(structure_description);

                var location = Create_the_Location();

                if (site.Is_Empty())
                    return location;

                var unit = site.Must_Be_a_Unit();
                unit_factory.Creates_a_Unit(unit, location);
                
                return location;
            }
        }

        public void Init(AdjacentRef<MTile> adjacent_tiles)
        {
            Adjacent = adjacent_tiles;
        }
        
        public Possible<MUnit> s_possible_Unit => Site.s_possible_Unit();

        public MLocation must_have_a_Location() => Site.must_be_a_Location();
        public bool has_a_Location(out MLocation unit) => Site.is_a_Location(out unit);
        public bool has_a_Unit(out MUnit unit) => Site.has_a_Unit(out unit);
        public bool is_Passable_By(ChassisType chassis_type) => the_landscape.Is_Passable_By(chassis_type);
        public bool is_Adjacent_To(MTile destination) => s_Position.Is_Adjacent_To(destination.s_Position);
        public Direction2D Direction_To(MTile destination) => s_Position.Direction_To(destination.s_Position);
        public bool has_a_Structure(out MStructure structure) => Site.is_a_Structure(out structure);
        public LandscapeType Type_Of_the_Landscape() => the_landscape.Type;

        internal void Creates_a_Debris_with(Possible<Stuff> inventory_content)
        {
            var debris = StructureDescription.Create.Debris(TileHelper.GetOrientation(s_Position), inventory_content);
            Set_Structure(debris);
        }
        
        internal void Reset_Structure()
        {
            Site.is_a_Structure().Otherwise_Throw("Can't reset structure on a tile, since the tile doesn't conatin a structure");
            Site = Create_Location_Empty_Site();
        }

        private TileSite Site
        {
            get => the_sites_cell.s_Value;
            set => the_sites_cell.s_Value = value;
        }

        private void Set_Structure(StructureDescription debris)
        {
            Site.is_Empty.Otherwise_Throw("Site must be empty before it can contain a structure");
            Site = Create_Structure_Site(debris);
        }

        private TileSite Create_Location_Empty_Site()
        {
            var unit_slot = Create_the_Location();
            return new TileSite(unit_slot);
        }

        private MLocation Create_the_Location() => new MLocation(this);

        private TileSite Create_Structure_Site(StructureDescription structure_description)
        {
            var structure = new MStructure(structure_description, this);
            return new TileSite(structure);
        }

        private readonly GuardedCell<TileSite> the_sites_cell;
        private readonly MLandscape the_landscape;
    }
}