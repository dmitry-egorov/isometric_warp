using System;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Tile.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Board.Unit.Weapon;

namespace WarpSpace.Models.Game.Battle.Player
{
    public struct PlayerCommand
    {
        public readonly Type TheType;

        public enum Type
        {
            Fire,
            Select_Unit,
            Move,
            Interact
        }

        private PlayerCommand(Type theType) : this()
        {
            TheType = theType;
        }

        public static PlayerCommand Fire(WeaponModel weapon, UnitModel target_unit) => new PlayerCommand(Type.Fire) { FireSlot = new FireCommand(weapon, target_unit)};
        public static PlayerCommand Select_Unit(PlayerModel player, UnitModel target_unit) => new PlayerCommand(Type.Select_Unit) { SelectUnitSlot = new SelectUnitCommand(player, target_unit)};
        public static PlayerCommand Move(UnitModel unit, TileModel target_tile) => new PlayerCommand(Type.Move) { MoveSlot = new MoveCommand(unit, target_tile)};
        public static PlayerCommand Interact(UnitModel unit, StructureModel target_structure) => new PlayerCommand(Type.Interact) { InteractSlot = new InteractCommand(unit, target_structure)};

        private struct FireCommand
        {
            private readonly WeaponModel _selected_weapon;
            private readonly UnitModel _target_unit;

            public FireCommand(WeaponModel selectedWeapon, UnitModel targetUnit)
            {
                _selected_weapon = selectedWeapon;
                _target_unit = targetUnit;
            }

            public void Execute() => _selected_weapon.Try_to_Fire_At(_target_unit);
        }

        private struct SelectUnitCommand
        {
            private readonly PlayerModel _player;
            private readonly UnitModel _unit;

            public SelectUnitCommand(PlayerModel player, UnitModel unit) { _player = player; _unit = unit; }
                
            public void Execute() => _player.Select_a_Unit(_unit);
        }

        private struct MoveCommand
        {
            private readonly UnitModel _unit;
            private readonly TileModel _target_tile;

            public MoveCommand(UnitModel unit, TileModel target_tile) { _unit = unit; _target_tile = target_tile; }

            public void Execute() => _unit.Try_to_Move_To(_target_tile);
        }

        private struct InteractCommand
        {
            private readonly UnitModel _unit;
            private readonly StructureModel _target_structure;

            public InteractCommand(UnitModel unit, StructureModel target_structure) { _unit = unit; _target_structure = target_structure; }

            public void Execute() => _unit.Try_to_Interact_With(_target_structure);
        }

        private FireCommand? FireSlot;
        private SelectUnitCommand? SelectUnitSlot;
        private MoveCommand? MoveSlot;
        private InteractCommand? InteractSlot;

        public void Execute()
        {
            switch (TheType)
            {
                case Type.Fire:
                    FireSlot.Value.Execute();
                    return;
                case Type.Select_Unit:
                    SelectUnitSlot.Value.Execute();
                    return;
                case Type.Move:
                    MoveSlot.Value.Execute();
                    return;
                case Type.Interact:
                    InteractSlot.Value.Execute();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}