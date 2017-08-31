using System;
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
        private readonly Spacial2D _entrance_spacial;
        
        private readonly RepeatAllStream<UnitAdded> _stream_of_added_units = new RepeatAllStream<UnitAdded>();
        private readonly RepeatAllStream<UnitModel> _stream_of_destoryed_units = new RepeatAllStream<UnitModel>();
        public IStream<UnitAdded> Stream_Of_Added_Units => _stream_of_added_units;
        public IStream<UnitModel> Stream_Of_Destroyed_Units => _stream_of_destoryed_units;

        public BoardModel(TileModel[,] tiles, Spacial2D entranceSpacial)
        {
            _entrance_spacial = entranceSpacial;
            Tiles = tiles;
        }

        public void WarpInMothership()
        {
            var position = _entrance_spacial.Position;
            var orientation = _entrance_spacial.Orientation;
            
            var source = Tiles.Get(position);
            var initialTile = Tiles.Get(position + orientation);
            var mothership = new UnitModel(UnitType.Mothership, initialTile, Faction.Players);
            
            Add(mothership, source);
        }

        public void AddUnit(UnitType type, Index2D position, Index2D sourcePosition, Faction faction)
        {
            var initialTile = Tiles.Get(position);
            var sourceTile = Tiles.Get(sourcePosition);
            
            var unit = new UnitModel(type, initialTile, faction);
            Add(unit, sourceTile);
        }

        public void Add(UnitModel unit, TileModel source)
        {
            var wirings = new Action[0]; 
            wirings = new[]
            {
                Wire_Destruction(),
                Wire_Movement()
            }; 
            
            _stream_of_added_units.Next(new UnitAdded(unit, source));
        
            Action Wire_Movement() => 
                unit
                    .Current_Tile_Cell
                    .IncludePrevious()
                    .Subscribe(p =>
                    {
                        if (p.previous.Has_a_Value(out var prev))
                            prev.ResetUnit();
                            
                        p.current.SetUnit(unit);
                    });
            
            Action Wire_Destruction()
            {
                return unit
                    .Stream_Of_Destroyed_Events
                    .Subscribe(_ => Destroy());

                void Destroy()
                {
                    Send_Destroyed_Event();
                    Unwire();

                    void Send_Destroyed_Event() => _stream_of_destoryed_units.Next(unit);

                    void Unwire()
                    {
                        foreach (var wiring in wirings)
                            wiring();
                    }
                }
            }
        }

        public struct UnitAdded
        {
            public readonly UnitModel Unit;
            public readonly TileModel SourceTile;

            public UnitAdded(UnitModel unit, TileModel sourceTile)
            {
                Unit = unit;
                SourceTile = sourceTile;
            }
        }
    }
}