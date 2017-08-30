using JetBrains.Annotations;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Tile.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class UnitModel
    {
        public readonly UnitType Type;
        public readonly WeaponModel Weapon;
        public readonly HealthModel Health;
        private readonly ChassisModel _chassis;

        public ICell<TileModel> CurrentTileCell => _chassis.CurrentTile; //Tile is not null
        public IStream<TheVoid> Destroyed { get; }
        public bool is_owned_by_the_player { get; }

        public UnitModel(UnitType type, TileModel initialTile, bool isOwnedByThePlayer)
        {
            Type = type;
            Weapon = new WeaponModel(type.GetWeaponType(), this);
            _chassis = ChassisModel.From(type, initialTile);
            Health = HealthModel.From(type);
            is_owned_by_the_player = isOwnedByThePlayer;

            Destroyed = get_Destroyed_stream();

            IStream<TheVoid> get_Destroyed_stream() => 
                Health
                    .IsAliveCell
                    .Where(is_alive => !is_alive)
                    .Select(_ => TheVoid.Instance)
                    .First();
        }

        public bool Can_Move_To_the(TileModel destination) => _chassis.CanMoveTo(destination);
        public bool try_to_Move_To_the(TileModel destination) => _chassis.TryMoveTo(destination);

        public bool take_the(DamageDescription damage) => Health.Take(damage);

        public bool Can_Interact_With_the(StructureModel structure) => structure.Interactor.CanBeInteractedBy(this);
        public bool try_to_Interact_with_the(StructureModel structure) => structure.Interactor.TryInteractBy(this);
    }
}