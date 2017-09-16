using System;
using JetBrains.Annotations;

namespace Lanski.Structures
{
    public struct Or<T1, T2>: IEquatable<Or<T1, T2>>
    {
        public static implicit operator Or<T1, T2>(T1 v) => new Or<T1, T2>(v);
        public static implicit operator Or<T1, T2>(T2 v) => new Or<T1, T2>(v);
        
        public Or(T1 v) : this() => _v1 = v;
        public Or(T2 v) : this() => _v2 = v;

        [Pure] public bool is_a_T1(out T1 v) => _v1.has_a_Value(out v);
        [Pure] public bool is_a_T2(out T2 v) => _v2.has_a_Value(out v);
        
        [Pure] public bool is_a_T1() => _v1.has_a_Value();
        [Pure] public bool is_a_T2() => _v2.has_a_Value();
        
        [Pure] public Possible<T1> as_a_T1() => _v1;
        [Pure] public Possible<T2> as_a_T2() => _v2;

        [Pure] public T1 must_be_a_T1() => _v1.must_have_a_Value();
        [Pure] public T2 must_be_a_T2() => _v2.must_have_a_Value();

        public bool Equals(Or<T1, T2> other) => _v1.Equals(other._v1) && _v2.Equals(other._v2);
        
        public bool has_the_Same_Type_As(Or<T1, T2> the_other) =>
            _v1.has_a_Value() && the_other._v1.has_a_Value() ||
            _v2.has_a_Value() && the_other._v2.has_a_Value()
        ;
        
        private Possible<T1> _v1;
        private Possible<T2> _v2;
    }
    
    public struct Or<T1, T2, T3>: IEquatable<Or<T1, T2, T3>>
    {
        public static implicit operator Or<T1, T2, T3>(T1 v) => new Or<T1, T2, T3>(v);
        public static implicit operator Or<T1, T2, T3>(T2 v) => new Or<T1, T2, T3>(v);
        public static implicit operator Or<T1, T2, T3>(T3 v) => new Or<T1, T2, T3>(v);
        
        public Or(T1 v) : this() => _v1 = v;
        public Or(T2 v) : this() => _v2 = v;
        public Or(T3 v) : this() => _v3 = v;

        [Pure] public bool is_a_T1(out T1 v) => _v1.has_a_Value(out v);
        [Pure] public bool is_a_T2(out T2 v) => _v2.has_a_Value(out v);
        [Pure] public bool is_a_T3(out T3 v) => _v3.has_a_Value(out v);
        
        [Pure] public bool is_a_T1() => _v1.has_a_Value();
        [Pure] public bool is_a_T2() => _v2.has_a_Value();
        [Pure] public bool is_a_T3() => _v3.has_a_Value();
        
        [Pure] public Possible<T1> as_a_T1() => _v1;
        [Pure] public Possible<T2> as_a_T2() => _v2;
        [Pure] public Possible<T3> as_a_T3() => _v3;

        [Pure] public T1 must_be_a_T1() => _v1.must_have_a_Value();
        [Pure] public T2 must_be_a_T2() => _v2.must_have_a_Value();
        [Pure] public T3 must_be_a_T3() => _v3.must_have_a_Value();

        public bool Equals(Or<T1, T2, T3> other) => _v1.Equals(other._v1) && _v2.Equals(other._v2) && _v3.Equals(other._v3);
        
        public bool has_the_Same_Type_As(Or<T1, T2, T3> the_other) =>
            _v1.has_a_Value() && the_other._v1.has_a_Value() ||
            _v2.has_a_Value() && the_other._v2.has_a_Value() ||
            _v3.has_a_Value() && the_other._v3.has_a_Value()
        ;

        private Possible<T1> _v1;
        private Possible<T2> _v2;
        private Possible<T3> _v3;
    }
    
    public struct Or<T1, T2, T3, T4>: IEquatable<Or<T1, T2, T3, T4>>
    {
        public static implicit operator Or<T1, T2, T3, T4>(T1 v) => new Or<T1, T2, T3, T4>(v);
        public static implicit operator Or<T1, T2, T3, T4>(T2 v) => new Or<T1, T2, T3, T4>(v);
        public static implicit operator Or<T1, T2, T3, T4>(T3 v) => new Or<T1, T2, T3, T4>(v);
        public static implicit operator Or<T1, T2, T3, T4>(T4 v) => new Or<T1, T2, T3, T4>(v);

        public Or(T1 v) : this() => _v1 = v;
        public Or(T2 v) : this() => _v2 = v;
        public Or(T3 v) : this() => _v3 = v;
        public Or(T4 v) : this() => _v4 = v;

        [Pure] public bool is_a_T1(out T1 v) => _v1.has_a_Value(out v);
        [Pure] public bool is_a_T2(out T2 v) => _v2.has_a_Value(out v);
        [Pure] public bool is_a_T3(out T3 v) => _v3.has_a_Value(out v);
        [Pure] public bool is_a_T4(out T4 v) => _v4.has_a_Value(out v);
        
        [Pure] public bool is_a_T1() => _v1.has_a_Value();
        [Pure] public bool is_a_T2() => _v2.has_a_Value();
        [Pure] public bool is_a_T3() => _v3.has_a_Value();
        [Pure] public bool is_a_T4() => _v4.has_a_Value();
        
        [Pure] public Possible<T1> as_a_T1() => _v1;
        [Pure] public Possible<T2> as_a_T2() => _v2;
        [Pure] public Possible<T3> as_a_T3() => _v3;
        [Pure] public Possible<T4> as_a_T4() => _v4;

        [Pure] public T1 must_be_a_T1() => _v1.must_have_a_Value();
        [Pure] public T2 must_be_a_T2() => _v2.must_have_a_Value();
        [Pure] public T3 must_be_a_T3() => _v3.must_have_a_Value();
        [Pure] public T4 must_be_a_T4() => _v4.must_have_a_Value();
        
        public bool Equals(Or<T1, T2, T3, T4> other) => _v1.Equals(other._v1) && _v2.Equals(other._v2) && _v3.Equals(other._v3) && _v4.Equals(other._v4);
        
        public bool has_the_Same_Type_As(Or<T1, T2, T3, T4> the_other) =>
            _v1.has_a_Value() && the_other._v1.has_a_Value() ||
            _v2.has_a_Value() && the_other._v2.has_a_Value() ||
            _v3.has_a_Value() && the_other._v3.has_a_Value() ||
            _v4.has_a_Value() && the_other._v4.has_a_Value()
        ;

        private Possible<T1> _v1;
        private Possible<T2> _v2;
        private Possible<T3> _v3;
        private Possible<T4> _v4;
    }
    
    public struct Or<T1, T2, T3, T4, T5>: IEquatable<Or<T1, T2, T3, T4, T5>>
    {
        public static implicit operator Or<T1, T2, T3, T4, T5>(T1 v) => new Or<T1, T2, T3, T4, T5>(v);
        public static implicit operator Or<T1, T2, T3, T4, T5>(T2 v) => new Or<T1, T2, T3, T4, T5>(v);
        public static implicit operator Or<T1, T2, T3, T4, T5>(T3 v) => new Or<T1, T2, T3, T4, T5>(v);
        public static implicit operator Or<T1, T2, T3, T4, T5>(T4 v) => new Or<T1, T2, T3, T4, T5>(v);
        public static implicit operator Or<T1, T2, T3, T4, T5>(T5 v) => new Or<T1, T2, T3, T4, T5>(v);
        
        public Or(T1 v) : this() => _v1 = v;
        public Or(T2 v) : this() => _v2 = v;
        public Or(T3 v) : this() => _v3 = v;
        public Or(T4 v) : this() => _v4 = v;
        public Or(T5 v) : this() => _v5 = v;

        [Pure] public bool is_a_T1(out T1 v) => _v1.has_a_Value(out v);
        [Pure] public bool is_a_T2(out T2 v) => _v2.has_a_Value(out v);
        [Pure] public bool is_a_T3(out T3 v) => _v3.has_a_Value(out v);
        [Pure] public bool is_a_T4(out T4 v) => _v4.has_a_Value(out v);
        [Pure] public bool is_a_T5(out T5 v) => _v5.has_a_Value(out v);
        
        [Pure] public bool is_a_T1() => _v1.has_a_Value();
        [Pure] public bool is_a_T2() => _v2.has_a_Value();
        [Pure] public bool is_a_T3() => _v3.has_a_Value();
        [Pure] public bool is_a_T4() => _v4.has_a_Value();
        [Pure] public bool is_a_T5() => _v5.has_a_Value();
        
        [Pure] public Possible<T1> as_a_T1() => _v1;
        [Pure] public Possible<T2> as_a_T2() => _v2;
        [Pure] public Possible<T3> as_a_T3() => _v3;
        [Pure] public Possible<T4> as_a_T4() => _v4;
        [Pure] public Possible<T5> as_a_T5() => _v5;

        [Pure] public T1 must_be_a_T1() => _v1.must_have_a_Value();
        [Pure] public T2 must_be_a_T2() => _v2.must_have_a_Value();
        [Pure] public T3 must_be_a_T3() => _v3.must_have_a_Value();
        [Pure] public T4 must_be_a_T4() => _v4.must_have_a_Value();
        [Pure] public T5 must_be_a_T5() => _v5.must_have_a_Value();

        [Pure]public bool Equals(Or<T1, T2, T3, T4, T5> other) => _v1.Equals(other._v1) && _v2.Equals(other._v2) && _v3.Equals(other._v3) && _v4.Equals(other._v4) && _v5.Equals(other._v5);

        [Pure]public bool has_the_Same_Type_As(Or<T1, T2, T3, T4, T5> the_other) =>
            _v1.has_a_Value() && the_other._v1.has_a_Value() ||
            _v2.has_a_Value() && the_other._v2.has_a_Value() ||
            _v3.has_a_Value() && the_other._v3.has_a_Value() ||
            _v4.has_a_Value() && the_other._v4.has_a_Value() ||
            _v5.has_a_Value() && the_other._v5.has_a_Value()
        ;

        private Possible<T1> _v1;
        private Possible<T2> _v2;
        private Possible<T3> _v3;
        private Possible<T4> _v4;
        private Possible<T5> _v5;
    }
}