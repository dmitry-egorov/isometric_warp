using Lanski.Reactive;
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
        public readonly InventoryModel Inventory;

        public ICell<TileModel> Cell_of_the_Current_Tile => _chassis.Current_Tile;
        public TileModel Current_Tile => Cell_of_the_Current_Tile.Value;
        public IStream<UnitDestroyed> Stream_Of_Single_Destroyed_Event => _destructor.Stream_Of_Single_Destroyed_Event;
        public Faction Faction { get; }
        public bool Is_Alive => Health.Is_Alive;

        public UnitModel(UnitType type, TileModel initial_tile, Faction faction, InventoryContent? initial_inventory_content)
        {
            Type = type;

            var destructor = new DestructorModel(this);
            
            Weapon = new WeaponModel(type.Get_Weapon_Type(), this);
            Health = HealthModel.From(type, destructor);
            Faction = faction;
            Inventory = InventoryModel.From(initial_inventory_content);

            _chassis = ChassisModel.From(this, initial_tile);
            _destructor = destructor;
        }

        public bool Try_to_Move_To(TileModel tile) => _chassis.Try_to_Move_To(tile);
        public bool Try_to_Interact_With(StructureModel structure) => structure.Interactor.Try_to_Interact_With(this);
        public void Take(DamageDescription damage) => Health.Take(damage);
        public void Take(InventoryContent? loot) => Inventory.Add(loot);
        
        public bool Can_Move_To(TileModel destination) => _chassis.Can_Move_To(destination);
        public bool Can_Interact_With(StructureModel structure) => structure.Interactor.Can_Interact_With(this);
        public bool Is_At(TileModel tile) => Current_Tile == tile;
        
        private readonly DestructorModel _destructor;
        private readonly ChassisModel _chassis;
    }
}