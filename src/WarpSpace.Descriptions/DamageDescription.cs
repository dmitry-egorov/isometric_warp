using WarpSpace.Models.Game.Battle.Board.Unit.Weapon;

namespace WarpSpace.Descriptions
{
    public struct DamageDescription
    {
        public readonly int Amount;
        public readonly DamageType Type;

        public DamageDescription(int amount, DamageType type)
        {
            Amount = amount;
            Type = type;
        }
    }
}