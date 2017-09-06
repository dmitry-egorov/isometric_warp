using System;

namespace Lanski.Structures
{
    public static class BooleanSemanticExtensions
    {
        public static void Must_Be_True_Otherwise_Throw(this bool value, string message = "") => value.Otherwise_Throw(message);
        public static void Must_Be_True(this bool value) => value.Otherwise_Throw();
        public static void Otherwise_Throw(this bool value, string message = "")
        {
            if(!value)
                throw new InvalidOperationException(message);
        }
    }
}