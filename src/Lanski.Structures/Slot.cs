using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using JetBrains.Annotations;

namespace Lanski.Structures
{
    public static class Slot
    {
        public static Slot<T> Empty<T>() => new Slot<T>();
        public static Slot<T> From<T>(T obj) => new Slot<T>(true, obj);
        public static Slot<T> As_a_Slot<T>(this T obj) => From(obj);
    }
    
    /// <summary>
    /// A nullable reference (for improved code semantics)
    /// </summary>
    public struct Slot<T>
    {
        private readonly bool _has_a_value;
        private readonly T _obj;

        public Slot(bool has_a_value, T obj)
        {
            _has_a_value = has_a_value;
            _obj = obj;
        }


        [Pure]public bool Has_Nothing() => !Has_a_Value();
        [Pure]public bool Has_a_Value() => _has_a_value;

        [Pure]public bool Doesnt_Have(out T o) => !Has_a_Value(out o);

        [Pure]public bool Has_a_Value(out T o)
        {
            o = _obj;
            return Has_a_Value();
        }
        
        [Pure] public T Must_Have_a_Value() => Has_a_Value(out var value) ? value : throw new InvalidOperationException();

        public Slot<TResult> SelectMany<TResult>(Func<T, Slot<TResult>> selector) => Has_a_Value(out var value) ? selector(value) : default(Slot<TResult>);
        public Slot<TResult> Select<TResult>(Func<T, TResult> selector) => Has_a_Value(out var value) ? new Slot<TResult>(true, selector(value)) : default(Slot<TResult>);
        
        public T Value_Or(T defaultValue) => Has_a_Value(out var value) ? value : defaultValue;
        
        public bool Equals(Slot<T> other) => _has_a_value == other._has_a_value && EqualityComparer<T>.Default.Equals(_obj, other._obj);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Slot<T> && Equals((Slot<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_has_a_value.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(_obj);
            }
        }
    }
}