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
        private readonly Spacial2D _entranceSpacial;
        
        private readonly RepeatAllStream<UnitAdded> _unitAddedStream = new RepeatAllStream<UnitAdded>();
        public IStream<UnitAdded> UnitAddedStream => _unitAddedStream;

        public BoardModel(TileModel[,] tiles, Spacial2D entranceSpacial)
        {
            _entranceSpacial = entranceSpacial;
            Tiles = tiles;
        }

        public void WarpInMothership()
        {
            var position = _entranceSpacial.Position;
            var orientation = _entranceSpacial.Orientation;
            
            var source = Tiles.Get(position);
            var initialTile = Tiles.Get(position + orientation);
            var mothership = new UnitModel(UnitType.Mothership, initialTile, true);
            
            Add(mothership, source);
        }

        public void AddUnit(UnitType type, Index2D position, Index2D sourcePosition, bool isOwnedByPlayer)
        {
            var initialTile = Tiles.Get(position);
            var sourceTile = Tiles.Get(sourcePosition);
            
            var unit = new UnitModel(type, initialTile, isOwnedByPlayer);
            Add(unit, sourceTile);
        }

        public void Add(UnitModel unit, TileModel source)
        {
            var wirings = new Action[0]; 
            wirings = new[]
            {
                wire_Destruction(),
                wire_Movement()
            }; 
            
            _unitAddedStream.Next(new UnitAdded(unit, source));
        
            Action wire_Movement() => 
                unit
                    .CurrentTileCell
                    .IncludePrevious()
                    .Subscribe(p =>
                    {
                        if (p.previous.Has_a_Value(out var prev))
                            prev.ResetUnit();
                            
                        p.current.SetUnit(unit);
                    });
            
            Action wire_Destruction()
            {
                return unit
                    .Destroyed
                    .Subscribe(_ => destroy());

                void destroy()
                {
                    unwire();
                    
                    void unwire()
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