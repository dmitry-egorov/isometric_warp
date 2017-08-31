using System;
using System.Runtime.Remoting.Messaging;
using JetBrains.Annotations;

namespace Lanski.Structures
{
    public static class Slot
    {
        public static Slot<T> From<T>(T obj) where T : class => obj;
        public static Slot<T> AsSlot<T>(this T obj) where T : class => obj;
    }
    
    /// <summary>
    /// A nullable reference (for improved code semantics)
    /// </summary>
    public struct Slot<T>
        where T: class
    {
        private readonly T _obj;

        public Slot(T obj) => _obj = obj;

        
        [Pure]public bool Has_Nothing() => !Has_a_Value();
        [Pure]public bool Has_a_Value() => _obj != null;

        [Pure]public bool doesnt_have(out T o) => !Has_a_Value(out o);

        [Pure]public bool has_a(out T o) => Has_a_Value(out o);
        [Pure]public bool Has_a_Value(out T o)
        {
            o = _obj;
            return Has_a_Value();
        }
        
        public MustHave Must_Have_a_Value() => new MustHave(this);

        public Slot<TResult> flat_map<TResult>(Func<T, Slot<TResult>> selector) where TResult : class => Has_a_Value(out var value) ? selector(value) : null;
        public Slot<TResult> Select<TResult>(Func<T, TResult> selector) where TResult : class => Has_a_Value(out var value) ? selector(value) : null;
        
        public T Value_Or(T defaultValue) => Has_a_Value(out var value) ? value : defaultValue;
        
        public static implicit operator Slot<T> (T val) => new Slot<T>(val);

        public class MustHave
        {
            private readonly Slot<T> _r;
            internal MustHave(Slot<T> r) => _r = r;
            public T Otherwise(Exception exception) => _r.Has_a_Value(out var value) ? value : throw exception;
        }
    }
}