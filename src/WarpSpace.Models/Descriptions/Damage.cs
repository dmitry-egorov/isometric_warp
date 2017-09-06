namespace WarpSpace.Models.Descriptions
{
    public struct Damage
    {
        public readonly int Amount;
        public readonly DamageType Type;

        public Damage(int amount, DamageType type)
        {
            Amount = amount;
            Type = type;
        }
    }
}