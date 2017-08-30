using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile.Landscape;
using WarpSpace.Models.Game.Battle.Board.Tile.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public class TileModel
    {
        public readonly Index2D Position;
        public readonly LandscapeModel Landscape;
        public readonly Slot<StructureModel> StructureSlot;
        
        public AdjacentRef<TileModel> Adjacent { get; private set; }

        private readonly ValueCell<Slot<UnitModel>> _unitCell = new ValueCell<Slot<UnitModel>>(null);
        public ICell<Slot<UnitModel>> UnitCell => _unitCell;
        
        public bool IsOccupied => Theres_a_Unit() || Theres_a_Structure();

        public TileModel(Index2D position, LandscapeType landscapeType, Slot<StructureModel> structureSlot)
        {
            Position = position;
            StructureSlot = structureSlot;
            Landscape = new LandscapeModel(landscapeType);

            wire_current_unit_destruction();
            
            void wire_current_unit_destruction() => 
                UnitCell
                    .SkipEmpty()
                    .SelectMany(u => u.Destroyed)
                    .Subscribe(_ => ResetUnit());
        }

        public void Init(AdjacentRef<TileModel> adjacentTiles)
        {
            Adjacent = adjacentTiles;
        }
        
        public bool Has_a_Unit(out UnitModel unit) => UnitCell.has_a_value(out unit);

        public bool IsPassableBy(ChassisType chassisType) => Landscape.IsPassableBy(chassisType) && !IsOccupied;
        public bool is_adjacent_to_the(TileModel destination) => Position.IsAdjacentTo(destination.Position);
        public Direction2D GetDirectionTo(TileModel destination) => Position.DirectionTo(destination.Position);
        public bool Has_a_Structure(out StructureModel structure) => StructureSlot.Has_a_Value(out structure);

        public void ResetUnit()
        {
            _unitCell.Value = null;
        }

        public void SetUnit(UnitModel model)
        {
            _unitCell.Value = model;
        }
        
        private bool Theres_a_Structure() => StructureSlot.has_something();
        private bool Theres_a_Unit() => UnitCell.has_something();
    }
}