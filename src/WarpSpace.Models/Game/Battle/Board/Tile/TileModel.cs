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

        public AdjacentRef<TileModel> Adjacent { get; private set; }
        
        private readonly ValueCell<Slot<StructureModel>> _structure_cell = new ValueCell<Slot<StructureModel>>(null);
        public ICell<Slot<StructureModel>> Structure_Cell => _structure_cell;

        private readonly ValueCell<Slot<UnitModel>> _unit_cell = new ValueCell<Slot<UnitModel>>(null);
        public ICell<Slot<UnitModel>> Unit_Cell => _unit_cell;
        
        public bool Is_Occupied => Has_a_Unit() || Has_a_Structure();

        public TileModel(Index2D position, LandscapeType landscapeType)
        {
            Position = position;
            Landscape = new LandscapeModel(landscapeType);

            Wire_Current_Unit_Destruction();
            
            void Wire_Current_Unit_Destruction() => 
                Unit_Cell
                    .SkipEmpty()
                    .SelectMany(u => u.Stream_Of_Destroyed_Events)
                    .Subscribe(_ => ResetUnit());
        }

        public void Init(AdjacentRef<TileModel> adjacentTiles)
        {
            Adjacent = adjacentTiles;
        }

        public bool Has_a_Unit(out UnitModel unit) => Unit_Cell.Has_a_Value(out unit);

        public bool IsPassableBy(ChassisType chassisType) => Landscape.Is_Passable_By(chassisType) && !Is_Occupied;
        public bool Is_Adjacent_To(TileModel destination) => Position.Is_Adjacent_To(destination.Position);
        public Direction2D GetDirectionTo(TileModel destination) => Position.Direction_To(destination.Position);
        public bool Has_a_Structure(out StructureModel structure) => Structure_Cell.Has_a_Value(out structure);
        
        public void ResetUnit()
        {
            _unit_cell.Value = null;
        }

        public void SetUnit(UnitModel model)
        {
            _unit_cell.Value = model;
        }
        
        public void Set_Structure(Slot<StructureModel> structure)
        {
            _structure_cell.Value = structure;
        }
        
        private bool Has_a_Structure() => Structure_Cell.Has_a_Value();
        private bool Has_a_Unit() => Unit_Cell.Has_a_Value();
    }
}