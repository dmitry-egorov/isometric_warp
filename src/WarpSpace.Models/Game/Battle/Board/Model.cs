using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class Model
    {
        public readonly Tile.Model[,] Tiles;
        private readonly Spacial2D _entranceSpacial;
        
        private readonly Stream<UnitAdded> _unitAddedStream = new Stream<UnitAdded>();
        public IStream<UnitAdded> UnitAddedStream => _unitAddedStream;

        public Model(Tile.Model[,] tiles, Spacial2D entranceSpacial)
        {
            _entranceSpacial = entranceSpacial;
            Tiles = tiles;
        }

        public void WrapInMothership()
        {
            var position = _entranceSpacial.Position;
            var orientation = _entranceSpacial.Orientation;
            
            var initialTile = Tiles.Get(position + orientation);
            var source = Tiles.Get(position);
            var mothership = new Unit.Model(Chassis.Mothership, initialTile);
            Add(mothership, source);
        }

        public void Add(Unit.Model unit, Tile.Model source)
        {
            WireMovement();
            _unitAddedStream.Next(new UnitAdded(unit, source));
        
            void WireMovement()
            {
                unit.CurrentTile.IncludePrevious().Subscribe(p =>
                {
                    if (p.Item1 != null)
                        p.Item1.ResetUnit();

                    if (p.Item2 != null)
                        p.Item2.SetUnit(unit);
                });
            }
        }

        public struct UnitAdded
        {
            public readonly Unit.Model Unit;
            public readonly Tile.Model SourceTile;

            public UnitAdded(Unit.Model unit, Tile.Model sourceTile)
            {
                Unit = unit;
                SourceTile = sourceTile;
            }
        }
    }
}