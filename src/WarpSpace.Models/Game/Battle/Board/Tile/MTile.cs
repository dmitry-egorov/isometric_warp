﻿using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public class MTile
    {
        public readonly Index2D Position;
        public readonly MLandscape MLandscape;

        public AdjacentRef<MTile> Adjacent { get; private set; }
        
        public ICell<TileSite> Site_Cell => _site_cell;

        public bool Is_Occupied => Site.Is_Occupied();

        public MTile(Index2D position, TileDescription desc, UnitFactory unit_factory)
        {
            Position = position;
            MLandscape = new MLandscape(desc.Type);

            var content = desc.Initial_Content;
            _site_cell = new ValueCell<TileSite>(Create_Site());

            TileSite Create_Site()
            {
                if (content.Is_a_Structure(out var structure_description))
                    return Create_Structure_Site(structure_description);

                var location = new MLocation(this, Possible.Empty<MUnit>());

                if (content.Is_Empty())
                    return location;

                var unit = content.Must_Be_a_Unit();
                unit_factory.Create_a_Unit(unit, location);
                
                return location;
            }
        }

        public void Init(AdjacentRef<MTile> adjacent_tiles)
        {
            Adjacent = adjacent_tiles;
        }

        public MLocation Must_Have_a_Location() => Site.Must_Be_a_Location();
        public bool Has_a_Location(out MLocation unit) => Site.Is_a_Location(out unit);
        public bool Has_a_Unit(out MUnit unit) => Site.Has_a_Unit(out unit);
        public bool Is_Passable_By(ChassisType chassis_type) => MLandscape.Is_Passable_By(chassis_type) && !Is_Occupied;
        public bool Is_Adjacent_To(MTile destination) => Position.Is_Adjacent_To(destination.Position);
        public Direction2D Get_Direction_To(MTile destination) => Position.Direction_To(destination.Position);
        public bool Has_a_Structure(out MStructure structure) => Site.Is_a_Structure(out structure);

        internal void Create_Debris(Possible<InventoryContent> inventory_content)
        {
            var debris = StructureDescription.Create.Debris(TileHelper.GetOrientation(Position), inventory_content);
            Set_Structure(debris);
        }
        
        internal void Reset_Structure()
        {
            Site.Is_a_Structure().Otherwise_Throw("Can't reset structure on a tile, since the tile doesn't conatin a structure");
            Site = Create_Location_Empty_Site();
        }

        private TileSite Site
        {
            get => _site_cell.Value;
            set => _site_cell.Value = value;
        }

        private void Set_Structure(StructureDescription debris)
        {
            Site.Is_Empty().Otherwise_Throw("Site must be empty before it can contain a structure");
            Site = Create_Structure_Site(debris);
        }

        private TileSite Create_Location_Empty_Site()
        {
            var unit_slot = new MLocation(this, Possible.Empty<MUnit>());
            return new TileSite(unit_slot);
        }

        private TileSite Create_Structure_Site(StructureDescription structure_description)
        {
            var structure = new MStructure(structure_description, this);
            return new TileSite(structure);
        }

        private readonly ValueCell<TileSite> _site_cell;
    }
}