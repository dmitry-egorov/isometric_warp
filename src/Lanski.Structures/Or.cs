using JetBrains.Annotations;

namespace Lanski.Structures
{
    public struct Or<T1, T2> 
    {
        public Or(T1 v) : this() => _v1 = v.As_a_Slot();
        public Or(T2 v) : this() => _v2 = v.As_a_Slot();

        [Pure] public bool Is_a_T1(out T1 v) => _v1.Has_a_Value(out v);
        [Pure] public bool Is_a_T2(out T2 v) => _v2.Has_a_Value(out v);
        
        [Pure] public bool Is_a_T1() => _v1.Has_a_Value();
        [Pure] public bool Is_a_T2() => _v2.Has_a_Value();
        
        [Pure] public Slot<T1> As_a_T1() => _v1;
        [Pure] public Slot<T2> As_a_T2() => _v2;

        [Pure] public T1 Must_Be_a_T1() => _v1.Must_Have_a_Value();
        [Pure] public T2 Must_Be_a_T2() => _v2.Must_Have_a_Value();
        
        public static implicit operator Or<T1, T2>(T1 v) => new Or<T1, T2>(v);
        public static implicit operator Or<T1, T2>(T2 v) => new Or<T1, T2>(v);
        
        private Slot<T1> _v1;
        private Slot<T2> _v2;
    }
    
    public struct Or<T1, T2, T3> 
    {
        public Or(T1 v) : this() => _v1 = v.As_a_Slot();
        public Or(T2 v) : this() => _v2 = v.As_a_Slot();
        public Or(T3 v) : this() => _v3 = v.As_a_Slot();

        [Pure] public bool Is_a_T1(out T1 v) => _v1.Has_a_Value(out v);
        [Pure] public bool Is_a_T2(out T2 v) => _v2.Has_a_Value(out v);
        [Pure] public bool Is_a_T3(out T3 v) => _v3.Has_a_Value(out v);
        
        [Pure] public bool Is_a_T1() => _v1.Has_a_Value();
        [Pure] public bool Is_a_T2() => _v2.Has_a_Value();
        [Pure] public bool Is_a_T3() => _v3.Has_a_Value();
        
        [Pure] public Slot<T1> As_a_T1() => _v1;
        [Pure] public Slot<T2> As_a_T2() => _v2;
        [Pure] public Slot<T3> As_a_T3() => _v3;

        [Pure] public T1 Must_Be_a_T1() => _v1.Must_Have_a_Value();
        [Pure] public T2 Must_Be_a_T2() => _v2.Must_Have_a_Value();
        [Pure] public T3 Must_Be_a_T3() => _v3.Must_Have_a_Value();
        
        public static implicit operator Or<T1, T2, T3>(T1 v) => new Or<T1, T2, T3>(v);
        public static implicit operator Or<T1, T2, T3>(T2 v) => new Or<T1, T2, T3>(v);
        public static implicit operator Or<T1, T2, T3>(T3 v) => new Or<T1, T2, T3>(v);

        private Slot<T1> _v1;
        private Slot<T2> _v2;
        private Slot<T3> _v3;
    }
    
    public struct Or<T1, T2, T3, T4> 
    {
        public Or(T1 v) : this() => _v1 = v.As_a_Slot();
        public Or(T2 v) : this() => _v2 = v.As_a_Slot();
        public Or(T3 v) : this() => _v3 = v.As_a_Slot();
        public Or(T4 v) : this() => _v4 = v.As_a_Slot();

        [Pure] public bool Is_a_T1(out T1 v) => _v1.Has_a_Value(out v);
        [Pure] public bool Is_a_T2(out T2 v) => _v2.Has_a_Value(out v);
        [Pure] public bool Is_a_T3(out T3 v) => _v3.Has_a_Value(out v);
        [Pure] public bool Is_a_T4(out T4 v) => _v4.Has_a_Value(out v);
        
        [Pure] public bool Is_a_T1() => _v1.Has_a_Value();
        [Pure] public bool Is_a_T2() => _v2.Has_a_Value();
        [Pure] public bool Is_a_T3() => _v3.Has_a_Value();
        [Pure] public bool Is_a_T4() => _v4.Has_a_Value();
        
        [Pure] public Slot<T1> As_a_T1() => _v1;
        [Pure] public Slot<T2> As_a_T2() => _v2;
        [Pure] public Slot<T3> As_a_T3() => _v3;
        [Pure] public Slot<T4> As_a_T4() => _v4;

        [Pure] public T1 Must_Be_a_T1() => _v1.Must_Have_a_Value();
        [Pure] public T2 Must_Be_a_T2() => _v2.Must_Have_a_Value();
        [Pure] public T3 Must_Be_a_T3() => _v3.Must_Have_a_Value();
        [Pure] public T4 Must_Be_a_T4() => _v4.Must_Have_a_Value();
        
        public static implicit operator Or<T1, T2, T3, T4>(T1 v) => new Or<T1, T2, T3, T4>(v);
        public static implicit operator Or<T1, T2, T3, T4>(T2 v) => new Or<T1, T2, T3, T4>(v);
        public static implicit operator Or<T1, T2, T3, T4>(T3 v) => new Or<T1, T2, T3, T4>(v);
        public static implicit operator Or<T1, T2, T3, T4>(T4 v) => new Or<T1, T2, T3, T4>(v);

        private Slot<T1> _v1;
        private Slot<T2> _v2;
        private Slot<T3> _v3;
        private Slot<T4> _v4;
    }
}