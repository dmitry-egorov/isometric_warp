﻿using System.Collections.Generic;
using System.Linq;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class BoardModel
    {
        public readonly TileModel[,] Tiles;
        public IReadOnlyCollection<UnitModel> Units => _units_hashset;

        public IStream<UnitCreated> Stream_Of_Unit_Creations => _unit_factory.Stream_Of_Unit_Creations;
        public IStream<UnitDestroyed> Stream_Of_Unit_Destructions => _stream_of_unit_destructions;
        public IStream<MothershipExited> Stream_Of_Exits { get; }

        public BoardModel(BoardDescription description)
        {
            _entrance_spacial = description.EntranceSpacial;
            _unit_factory = new UnitFactory();
            
            Tiles = CreaTiles();

            Stream_Of_Exits = Create_Exits_Stream();
            Wire_Unit_Creation();
            Create_Units();

            IStream<MothershipExited> Create_Exits_Stream() => 
                Stream_Of_Unit_Creations
                .Select(x => x.Unit.Stream_Of_Exits)
                .Merge();

            TileModel[,] CreaTiles()
            {
                var tiles = description.Tiles.Map((tile_desc, position) => CreateTile(position, tile_desc));
                foreach (var i in tiles.EnumerateIndex())
                {
                    var adjacent = tiles.GetAdjacent(i);
                    tiles.Get(i).Init(adjacent);
                    new List<int>().Clear();
                }
                return tiles;
                
                TileModel CreateTile(Index2D position, TileDescription desc) => new TileModel(position, desc);
            }

            void Wire_Unit_Creation()
            {
                Stream_Of_Unit_Creations
                    .Subscribe(created =>
                    {
                        var unit = created.Unit;

                        Wire_Unit_Destruction(unit);

                        _units_hashset.Add(unit);
                    });
            }
            
            void Wire_Unit_Destruction(UnitModel unit)
            {
                unit
                    .Signal_Of_the_Destruction
                    .Subscribe(destroyed =>
                    {
                        _units_hashset.Remove(unit);
                        _stream_of_unit_destructions.Next(new UnitDestroyed(unit, unit.Location));
                    });
            }

            void Create_Units()
            {
                var units = description
                    .Units
                    .EnumerateWithIndex()
                    .Select(x => x.element.Select(unit => (unit, x.index)))
                    .SkipNull();
                
                foreach (var x in units)
                {
                    var unit = x.Item1;
                    var index = x.Item2;
                    Create_a_Unit(unit.Type, index, Faction.Natives, unit.Inventory_Content);
                }
            }
        }

        public void Warp_In_the_Mothership()
        {
            var position = _entrance_spacial.Position;
            var orientation = _entrance_spacial.Orientation;
            
            Create_a_Unit(UnitType.Mothership, position + orientation, Faction.Players, null);
        }

        private void Create_a_Unit(UnitType type, Index2D position, Faction faction, InventoryContent? initial_inventory_content)
        {
            Tiles.Get(position).Has_a_Location(out var location).Otherwise_Throw("Can't add a unit to a tile occupied by a structure");
            
            _unit_factory.Create_a_Unit(type, location, faction, initial_inventory_content);
        }

        private readonly Stream<UnitDestroyed> _stream_of_unit_destructions = new Stream<UnitDestroyed>();
        private readonly Spacial2D _entrance_spacial;
        
        private readonly HashSet<UnitModel> _units_hashset = new HashSet<UnitModel>();
        private readonly UnitFactory _unit_factory;
    }
}