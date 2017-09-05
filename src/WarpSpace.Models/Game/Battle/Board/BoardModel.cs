﻿using System.Collections.Generic;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
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

            Stream_Of_Exits = Create_Exits_Stream();
            Wire_Unit_Creation();

            Tiles = CreaTiles();

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
                }
                return tiles;

                TileModel CreateTile(Index2D position, TileDescription desc) =>
                    new TileModel(position, desc, _unit_factory);
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
        }

        public void Warp_In_the_Mothership()
        {
            Debug.Log("Warp mothership");
            var position = _entrance_spacial.Position;
            var orientation = _entrance_spacial.Orientation;

            var tank = new UnitDescription(UnitType.Tank, Faction.Players, Slot.Empty<InventoryContent>(), Slot.Empty<Slot<UnitDescription>[]>());
            
            var test1 = new UnitDescription();
            Debug.Log("1");
            var test2 = new UnitDescription[1];
            Debug.Log("2");
            var test3 = new Slot<UnitDescription>[1].As_a_Slot();
            Debug.Log("3");

            var tanks_slot = tank.As_a_Slot();
            Debug.Log("4");

            //var bay_units = new [] { tanks_slot };
            var bay_units = new Slot<UnitDescription>[1].As_a_Slot();
            //var bay_units = new Slot<UnitDescription>[0].As_a_Slot();

            Debug.Log("Created bay description");

            var desc = new UnitDescription(UnitType.Mothership, Faction.Players, Slot.Empty<InventoryContent>(), bay_units);
            
            Debug.Log("Created mothership description");

            Create_a_Unit(desc, position + orientation);
        }

        private void Create_a_Unit(UnitDescription desc, Index2D position)
        {
            Tiles.Get(position).Has_a_Location(out var location)
                .Otherwise_Throw("Can't add a unit to a tile occupied by a structure");

            _unit_factory.Create_a_Unit(desc, location);
        }

        private readonly Stream<UnitDestroyed> _stream_of_unit_destructions = new Stream<UnitDestroyed>();
        private readonly Spacial2D _entrance_spacial;

        private readonly HashSet<UnitModel> _units_hashset = new HashSet<UnitModel>();
        private readonly UnitFactory _unit_factory;
    }
    
    public struct UnitDescription
    {
        public readonly UnitType Type;
        public readonly Faction Faction;
        public readonly Slot<InventoryContent> Inventory_Content;
        public readonly Slot<Slot<UnitDescription>[]> Bay_Content;

        public UnitDescription(UnitType type, Faction faction, Slot<InventoryContent> inventory_content, Slot<Slot<UnitDescription>[]> bay_content)
        {
            Type = type;
            Inventory_Content = inventory_content;
            Bay_Content = bay_content;
            Faction = faction;
        }
    }
}