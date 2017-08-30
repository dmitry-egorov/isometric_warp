using System;
using System.Runtime.Remoting.Messaging;
using JetBrains.Annotations;

namespace Lanski.Structures
{
    public static class Slot
    {
        public static Slot<T> From<T>(T obj) where T : class => obj;
    }
    
    /// <summary>
    /// A nullable reference (for improved code semantics)
    /// </summary>
    public struct Slot<T>
        where T: class
    {
        private readonly T _obj;

        public Slot(T obj) => _obj = obj;

        
        [Pure]public bool Has_Nothing() => !has_something();
        [Pure]public bool has_something() => _obj != null;

        [Pure]public bool doesnt_have(out T o) => !Has_a_Value(out o);

        [Pure]public bool has_a(out T o) => Has_a_Value(out o);
        [Pure]public bool Has_a_Value(out T o)
        {
            o = _obj;
            return has_something();
        }
        
        public MustHave must_have_something() => new MustHave(this);

        public Slot<TResult> flat_map<TResult>(Func<T, Slot<TResult>> selector) where TResult : class => Has_a_Value(out var value) ? selector(value) : null;
        public Slot<TResult> map<TResult>(Func<T, TResult> selector) where TResult : class => Has_a_Value(out var value) ? selector(value) : null;
        
        public T value_or(T defaultValue) => Has_a_Value(out var value) ? value : defaultValue;
        
        public static implicit operator Slot<T> (T val)
        {
            return new Slot<T>(val);
        }
        
        public class MustHave
        {
            private readonly Slot<T> _r;
            internal MustHave(Slot<T> r) => _r = r;
            public T otherwise(Exception exception) => _r.Has_a_Value(out var value) ? value : throw exception;
        }
    }
}