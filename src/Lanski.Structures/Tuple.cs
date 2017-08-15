using System;

namespace Lanski.Structures
{
    public static class Tuple
    {
        public static Tuple<T1, T2> From<T1, T2>(T1 item1, T2 item2) where T1 : IEquatable<T1> where T2 : IEquatable<T2>
        {
            return new Tuple<T1, T2>(item1, item2);
        }
    }

    public struct Tuple<T1, T2>: IEquatable<Tuple<T1, T2>>
        where T1: IEquatable<T1>
        where T2: IEquatable<T2>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }

        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public bool Equals(Tuple<T1, T2> other)
        {
            return Item1.Equals(other.Item1) && Item2.Equals(other.Item2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Tuple<T1, T2> && Equals((Tuple<T1, T2>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Item1.GetHashCode() * 397) ^ Item2.GetHashCode();
            }
        }
    }
    
    public struct Tuple<T1, T2, T3>: IEquatable<Tuple<T1, T2, T3>>
        where T1: IEquatable<T1>
        where T2: IEquatable<T2>
        where T3: IEquatable<T3>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }

        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public bool Equals(Tuple<T1, T2, T3> other)
        {
            return Item1.Equals(other.Item1) && Item2.Equals(other.Item2) && Item3.Equals(other.Item3);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Tuple<T1, T2, T3> && Equals((Tuple<T1, T2, T3>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Item1.GetHashCode();
                hashCode = (hashCode * 397) ^ Item2.GetHashCode();
                hashCode = (hashCode * 397) ^ Item3.GetHashCode();
                return hashCode;
            }
        }
    }
    
    public struct Tuple<T1, T2, T3, T4>: IEquatable<Tuple<T1, T2, T3, T4>>
        where T1: IEquatable<T1>
        where T2: IEquatable<T2>
        where T3: IEquatable<T3>
        where T4: IEquatable<T4>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }
        public T4 Item4 { get; }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }
        
        public bool Equals(Tuple<T1, T2, T3, T4> other)
        {
            return Item1.Equals(other.Item1) && Item2.Equals(other.Item2) && Item3.Equals(other.Item3) && Item4.Equals(other.Item4);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Tuple<T1, T2, T3, T4> && Equals((Tuple<T1, T2, T3, T4>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Item1.GetHashCode();
                hashCode = (hashCode * 397) ^ Item2.GetHashCode();
                hashCode = (hashCode * 397) ^ Item3.GetHashCode();
                hashCode = (hashCode * 397) ^ Item4.GetHashCode();
                return hashCode;
            }
        }
    }
}