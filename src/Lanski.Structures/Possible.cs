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
        
        [Pure]public bool has_a_Value(out T o)
        {
            o = _obj;
            return _has_a_value;
        }

        public bool Equals(Possible<T> other) => _has_a_value == other._has_a_value && (!_has_a_value || EqualityComparer<T>.Default.Equals(_obj, other._obj));
        public override string ToString() => has_a_Value(out var the_value) ? the_value.ToString() : "Empty";

        public static implicit operator Possible<T>(T value) => new Possible<T>(true, value);
    }

    public static class PossibleSemanticExtensions
    {
        public static bool exists<T>(this Possible<T> p) => p.has_a_Value();
        [Pure]public static bool has_a_Value<T>(this Possible<T> the_possible) => the_possible.has_a_Value(out var _);
        [Pure]public static bool has_Nothing<T>(this Possible<T> the_possible) => !the_possible.has_a_Value();
        [Pure] public static bool @is<T>(this Possible<T> the_possible, Func<T, bool> selector) => the_possible.has_a_Value(out var the_value) && selector(the_value);
        [Pure] public static Possible<TResult> map<TResult, T>(this Possible<T> the_possible, Func<T, TResult> selector) => the_possible.Select(selector);
        [Pure] public static Possible<TResult> fmap<TResult, T>(this Possible<T> the_possible, Func<T, Possible<TResult>> selector) => the_possible.SelectMany(selector);
        [Pure] public static Possible<TResult> SelectMany<TResult, T>(this Possible<T> the_possible, Func<T, Possible<TResult>> selector) => the_possible.has_a_Value(out var value) ? selector(value) : default(Possible<TResult>);
        [Pure] public static Possible<TResult> Converted_With<TResult, T>(this Possible<T> the_possible, Func<T, TResult> selector) => the_possible.Select(selector);
        [Pure] public static Possible<TResult> Select<TResult, T>(this Possible<T> the_possible, Func<T, TResult> selector) => the_possible.has_a_Value(out var value) ? new Possible<TResult>(true, selector(value)) : default(Possible<TResult>);
        [Pure] public static T must_have_a_Value<T>(this Possible<T> the_possible) => the_possible.has_a_Value(out var value) ? value : throw new InvalidOperationException("Must have a value");
        [Pure] public static T s_Value_Or<T>(this Possible<T> the_possible, T defaultValue) => the_possible.has_a_Value(out var value) ? value : defaultValue;
    }
    
}