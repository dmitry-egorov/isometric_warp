namespace WarpSpace.Models.Descriptions
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