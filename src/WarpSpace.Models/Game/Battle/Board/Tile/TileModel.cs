﻿using Lanski.Reactive;
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
        
        public ICell<Slot<StructureModel>> Structure_Cell => _structure_cell;

        private readonly ValueCell<Slot<UnitModel>> _unit_cell = new ValueCell<Slot<UnitModel>>(null);
        public ICell<Slot<UnitModel>> Unit_Cell => _unit_cell;
        
        public bool Is_Occupied => Has_a_Unit() || Has_a_Structure();

        public TileModel(Index2D position, LandscapeType landscape_type, StructureModelFactory structure_factory)
        {
            Position = position;
            _structure_factory = structure_factory;
            Landscape = new LandscapeModel(landscape_type);
        }

        public void Init(AdjacentRef<TileModel> adjacent_tiles)
        {
            Adjacent = adjacent_tiles;
        }

        public bool Has_a_Unit(out UnitModel unit) => Unit_Cell.Has_a_Value(out unit);
        public bool Is_Passable_By(ChassisType chassis_type) => Landscape.Is_Passable_By(chassis_type) && !Is_Occupied;
        public bool Is_Adjacent_To(TileModel destination) => Position.Is_Adjacent_To(destination.Position);
        public Direction2D GetDirectionTo(TileModel destination) => Position.Direction_To(destination.Position);
        public bool Has_a_Structure(out StructureModel structure) => Structure_Cell.Has_a_Value(out structure);
        
        public void Reset_Unit()
        {
            _unit_cell.Value = null;
        }

        public void Set_Unit(UnitModel model)
        {
            _unit_cell.Value = model;
        }

        public void Set_Structure(StructureDescription? possible_structure_desc)
        {
            _structure_cell.Value = possible_structure_desc.SelectRef(structure_desc => _structure_factory.Create(structure_desc, this));
        }
        
        private bool Has_a_Structure() => Structure_Cell.Has_a_Value();
        private bool Has_a_Unit() => Unit_Cell.Has_a_Value();
        
        private readonly StructureModelFactory _structure_factory;
        private readonly ValueCell<Slot<StructureModel>> _structure_cell = new ValueCell<Slot<StructureModel>>(null);
    }
}