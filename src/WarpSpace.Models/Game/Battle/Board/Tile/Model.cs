using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public class Model
    {
        public readonly Index2D Position;
        public readonly Landscape.Model Landscape;
        public readonly StructureDescription? Structure;
        
        public AdjacentRef<Model> Adjacent { get; private set; }

        private readonly RefCell<Unit.Model> _unitCell = new RefCell<Unit.Model>(null);
        public ICell<Unit.Model> Unit => _unitCell;
        
        public bool IsOccupied => Theres_a_Unit() || Theres_a_Structure();

        public Model(Index2D position, LandscapeType landscapeType, StructureDescription? structure)
        {
            Position = position;
            Structure = structure;
            Landscape = new Landscape.Model(landscapeType);
        }

        public void Init(AdjacentRef<Model> adjacentTiles)
        {
            Adjacent = adjacentTiles;
        }

        public bool IsPassableBy(Chassis chassis) => Landscape.IsPassableBy(chassis) && !IsOccupied;
        public bool IsAdjacentTo(Model destination) => Position.IsAdjacentTo(destination.Position);
        public Direction2D GetDirectionTo(Model destination) => Position.DirectionTo(destination.Position);

        public void ResetUnit()
        {
            _unitCell.Value = null;
        }

        public void SetUnit(Unit.Model model)
        {
            _unitCell.Value = model;
        }
        
        private bool Theres_a_Structure() => Structure.HasValue;
        private bool Theres_a_Unit() => Unit.Value != null;
    }
}