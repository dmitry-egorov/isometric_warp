using System.Collections.Generic;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class MBoard
    {
        public readonly MTile[,] Tiles;
        public IReadOnlyCollection<MUnit> Units => _units_hashset;

        public IStream<MUnit> Stream_Of_Unit_Creations => _unit_factory.Stream_Of_Unit_Creations;
        public IStream<MUnit> Stream_Of_Unit_Destructions => _stream_of_unit_destructions;
        public IStream<MothershipExited> Stream_Of_Exits { get; }

        public MBoard(BoardDescription description)
        {
            _entrance_spacial = description.EntranceSpacial;
            _unit_factory = new UnitFactory();

            Stream_Of_Exits = Create_Exits_Stream();
            Wire_Unit_Creation();

            Tiles = CreaTiles();

            IStream<MothershipExited> Create_Exits_Stream() =>
                Stream_Of_Unit_Creations
                    .Select(x => x.Stream_Of_Exits)
                    .Merge();

            MTile[,] CreaTiles()
            {
                var tiles = description.Tiles.Map((tile_desc, position) => CreateTile(position, tile_desc));
                foreach (var i in tiles.EnumerateIndex())
                {
                    var adjacent = tiles.GetAdjacent(i);
                    tiles.Get(i).Init(adjacent);
                }
                return tiles;

                MTile CreateTile(Index2D position, TileDescription desc) =>
                    new MTile(position, desc, _unit_factory);
            }

            void Wire_Unit_Creation()
            {
                Stream_Of_Unit_Creations
                    .Subscribe(unit =>
                    {
                        Wire_Unit_Destruction(unit);

                        _units_hashset.Add(unit);
                    });
            }

            void Wire_Unit_Destruction(MUnit unit)
            {
                unit
                    .Signal_Of_the_Destruction
                    .Subscribe(destroyed =>
                    {
                        _units_hashset.Remove(unit);
                        _stream_of_unit_destructions.Next(unit);
                    });
            }
        }

        public void Warp_In_the_Mothership()
        {
            var position = _entrance_spacial.Position;
            var orientation = _entrance_spacial.Orientation;

            var tank1 = new UnitDescription(UnitType.Tank, Faction.Players, Possible.Empty<InventoryContent>(), Possible.Empty<IReadOnlyList<Possible<UnitDescription>>>());
            var tank2 = new UnitDescription(UnitType.Tank, Faction.Players, Possible.Empty<InventoryContent>(), Possible.Empty<IReadOnlyList<Possible<UnitDescription>>>());
            var bay_units = new List<Possible<UnitDescription>> {tank1.As_a_Slot(), tank2.As_a_Slot()};
            var desc = new UnitDescription(UnitType.Mothership, Faction.Players, Possible.Empty<InventoryContent>(), bay_units);

            Create_a_Unit(desc, position + orientation);
        }

        private void Create_a_Unit(UnitDescription desc, Index2D position)
        {
            Tiles.Get(position).Has_a_Location(out var location).Otherwise_Throw("Can't add a unit to a tile occupied by a structure");

            _unit_factory.Create_a_Unit(desc, location);
        }

        private readonly Stream<MUnit> _stream_of_unit_destructions = new Stream<MUnit>();
        private readonly Spacial2D _entrance_spacial;

        private readonly HashSet<MUnit> _units_hashset = new HashSet<MUnit>();
        private readonly UnitFactory _unit_factory;
    }
}