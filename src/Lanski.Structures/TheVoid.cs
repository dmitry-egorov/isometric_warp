namespace Lanski.Structures
{
    public struct TheVoid
    {
        public static TheVoid Instance => new TheVoid();

        public bool Equals(TheVoid other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TheVoid && Equals((TheVoid) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}