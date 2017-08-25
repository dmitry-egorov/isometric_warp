using System;
using System.Linq;
using System.Reflection;

namespace Lanski.Reflection
{
    public static class ReflectionSimpleExtensions
    {
        public static bool IsToStringOverriden(this Type type)
        {
            var declaringType = type.GetMethod("ToString").DeclaringType;
            return declaringType != typeof(object) && declaringType != typeof(ValueType);
        }
        
        public static bool IsSimple(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            
            return type.IsPrimitive 
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal);
        }
    }
    public static class ReflectionGenericsExtensions
    {
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public static bool HasAttribute<T>(this Type type)
            where T: Attribute
        {
            return type.GetCustomAttributes(typeof(T), true).Any();
        }

        public static bool HasAttribute<T>(this MethodInfo method)
            where T: Attribute
        {
            return method.GetCustomAttributes(typeof(T), true).Any();
        }

        public static bool HasAttribute<T>(this FieldInfo field)
            where T: Attribute
        {
            return field.GetCustomAttributes(typeof(T), true).Any();
        }

        public static bool IsAssignableTo<T>(this Type type)
        {
            return type.IsAssignableTo(typeof(T));
        }
        public static bool IsAssignableTo(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }
    }
}