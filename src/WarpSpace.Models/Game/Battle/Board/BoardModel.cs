using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class BoardModel
    {
        public readonly TileModel[,] Tiles;
        
        public IStream<UnitCreated> Stream_Of_Unit_Creations => _stream_of_unit_creations;
        public IStream<UnitDestroyed> Stream_Of_Unit_Destructions => _stream_of_unit_destructions;

        public BoardModel(BoardDescription description, InteractorFactory interactor_factory)
        {
            _entrance_spacial = description.EntranceSpacial;

            var tiles = description.Tiles.Map((tile_desc, position) => CreateTile(position, tile_desc));
            foreach (var i in tiles.EnumerateIndex())
            {
                var adjacent = tiles.GetAdjacent(i);
                tiles.Get(i).Init(adjacent);new List<int>().Clear();
            }
            
            Tiles = tiles;
            
            TileModel CreateTile(Index2D position, TileDescription desc) => new TileModel(position, desc, interactor_factory);
        }

        public void Warp_In_the_Mothership()
        {
            var position = _entrance_spacial.Position;
            var orientation = _entrance_spacial.Orientation;
            
            Create_a_Unit(UnitType.Mothership, position + orientation, Faction.Players, null);
        }

        public void Create_a_Unit(UnitType type, Index2D position, Faction faction, InventoryContent? initial_inventory_content)
        {
            if (!Tiles.Get(position).Has_a_Unit_Slot(out var slot))
                throw new InvalidOperationException("Can't add a unit to a tile occupied by a structure");

            var unit = slot.Create_A_Unit(type, faction, initial_inventory_content);
            Add();
            
            void Add()
            {
                Wire_the_Destruction();
                Signal_the_Creation();

                void Signal_the_Creation()
                {
                    var unit_created = new UnitCreated(unit, unit.Location);
                    _stream_of_unit_creations.Next(unit_created);                    
                }
        
                void Wire_the_Destruction()
                {
                    unit
                        .Signal_Of_the_Destruction
                        .PublishTo(_stream_of_unit_destructions);
                }
            }
        }

        private readonly RepeatAllStream<UnitDestroyed> _stream_of_unit_destructions = new RepeatAllStream<UnitDestroyed>();
        private readonly RepeatAllStream<UnitCreated> _stream_of_unit_creations = new RepeatAllStream<UnitCreated>();
        private readonly Spacial2D _entrance_spacial;
    }
}