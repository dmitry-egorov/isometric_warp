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
        private readonly ChassisModel _chassis;
        
        public readonly UnitType Type;
        public readonly WeaponModel Weapon;
        public readonly HealthModel Health;

        public ICell<TileModel> Current_Tile_Cell => _chassis.CurrentTile; //Tile is not null
        public TileModel Current_Tile => Current_Tile_Cell.Value;
        public IStream<TheVoid> Stream_Of_Destroyed_Events { get; }
        public Faction Faction { get; }
        public bool Is_Alive => Health.Is_Alive;

        public UnitModel(UnitType type, TileModel initialTile, Faction faction)
        {
            Type = type;
            Weapon = new WeaponModel(type.GetWeaponType(), this);
            _chassis = ChassisModel.From(type, initialTile);
            Health = HealthModel.From(type);
            Faction = faction;

            Stream_Of_Destroyed_Events = Get_Destroyed_stream();

            IStream<TheVoid> Get_Destroyed_stream() => 
                Health
                    .Is_Alive_Cell
                    .Where(is_alive => !is_alive)
                    .Select(_ => TheVoid.Instance)
                    .First();
        }

        public bool Can_Move_To(TileModel destination) => _chassis.Can_Move_To(destination);
        public bool Try_to_Move_To(TileModel destination) => _chassis.Try_to_Move_To(destination);

        public void Take(DamageDescription damage) => Health.Take(damage);

        public bool Can_Interact_With(StructureModel structure) => structure.Interactor.Can_Interact_With(this);
        public bool Try_to_Interact_With(StructureModel structure) => structure.Interactor.Try_to_Interact_With(this);

        public bool Is_At(TileModel tile) => Current_Tile_Cell.Value == tile;
    }
}