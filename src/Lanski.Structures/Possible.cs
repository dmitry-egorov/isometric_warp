using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lanski.Structures
{
    public static class Possible
    {
        public static Possible<T> Empty<T>() => new Possible<T>();
        public static Possible<T> From<T>(T obj)  => new Possible<T>(true, obj);
        
        public static Possible<TheVoid> as_a_Possible(this bool flag) => flag ? From(TheVoid.Instance) : Empty<TheVoid>(); 
        public static Possible<T> as_a_Possible<T>(this T value) => From(value); 
    }
    
    /// <summary>
    /// A nullable reference (for improved code semantics)
    /// </summary>
    public struct Possible<T>: IEquatable<Possible<T>>
    {
        private readonly bool _has_a_value;
        private readonly T _obj;

        public Possible(bool has_a_value, T obj)
        {
            _has_a_value = has_a_value;
            _obj = obj;
        }


        [Pure]public bool Does_Not_Have_a_Value() => Has_Nothing();
        [Pure]public bool Has_Nothing() => !has_a_Value();
        [Pure]public bool has_a_Value() => _has_a_value;

        [Pure]public bool Does_Not_Have_a_Value(out T o) => !has_a_Value(out o);

        public void Do(Action<T> action)
        {
            if (has_a_Value(out var value))
                action(value);
        }
        
        [Pure]public bool has_a_Value(out T o)
        {
            o = _obj;
            return has_a_Value();
        }
        
        [Pure] public T must_have_a_Value() => has_a_Value(out var value) ? value : throw new InvalidOperationException("Must have a value");

        [Pure] public bool @is(Func<T, bool> selector) => this.has_a_Value(out var the_value) && selector(the_value);
        [Pure] public Possible<TResult> map<TResult>(Func<T, TResult> selector) => Select(selector);
        [Pure] public Possible<TResult> fmap<TResult>(Func<T, Possible<TResult>> selector) => SelectMany(selector);
        [Pure] public Possible<TResult> SelectMany<TResult>(Func<T, Possible<TResult>> selector) => has_a_Value(out var value) ? selector(value) : default(Possible<TResult>);
        [Pure] public Possible<TResult> Select<TResult>(Func<T, TResult> selector) => has_a_Value(out var value) ? new Possible<TResult>(true, selector(value)) : default(Possible<TResult>);
        
        public T s_Value_Or(T defaultValue) => has_a_Value(out var value) ? value : defaultValue;
        
        public bool Equals(Possible<T> other) => _has_a_value == other._has_a_value && (!_has_a_value || EqualityComparer<T>.Default.Equals(_obj, other._obj));

        public static implicit operator Possible<T>(T value) => new Possible<T>(true, value);
    }
}