using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using JetBrains.Annotations;

namespace Lanski.Structures
{
    public static class Possible
    {
        public static Possible<T> Empty<T>() => new Possible<T>();
        public static Possible<T> From<T>(T obj) => new Possible<T>(true, obj);
        public static Possible<T> As_a_Possible_Value<T>(this T obj) => From(obj);
    }
    
    /// <summary>
    /// A nullable reference (for improved code semantics)
    /// </summary>
    public struct Possible<T>
    {
        private readonly bool _has_a_value;
        private readonly T _obj;

        public Possible(bool has_a_value, T obj)
        {
            _has_a_value = has_a_value;
            _obj = obj;
        }


        [Pure]public bool Does_Not_Have_a_Value() => Has_Nothing();
        [Pure]public bool Has_Nothing() => !Has_a_Value();
        [Pure]public bool Has_a_Value() => _has_a_value;

        [Pure]public bool Doesnt_Have_a_Value(out T o) => !Has_a_Value(out o);

        [Pure]public bool Has_a_Value(out T o)
        {
            o = _obj;
            return Has_a_Value();
        }
        
        [Pure] public T Must_Have_a_Value() => Has_a_Value(out var value) ? value : throw new InvalidOperationException("Must have a value");

        public Possible<TResult> SelectMany<TResult>(Func<T, Possible<TResult>> selector) => Has_a_Value(out var value) ? selector(value) : default(Possible<TResult>);
        public Possible<TResult> Select<TResult>(Func<T, TResult> selector) => Has_a_Value(out var value) ? new Possible<TResult>(true, selector(value)) : default(Possible<TResult>);
        
        public T Value_Or(T defaultValue) => Has_a_Value(out var value) ? value : defaultValue;
        
        public bool Equals(Possible<T> other) => _has_a_value == other._has_a_value && EqualityComparer<T>.Default.Equals(_obj, other._obj);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Possible<T> && Equals((Possible<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_has_a_value.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(_obj);
            }
        }

        public static implicit operator Possible<T>(T value) => new Possible<T>(true, value);
    }
}