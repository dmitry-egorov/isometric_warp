using System;

namespace Lanski.Structures
{
    public static class BooleanSemanticExtensions
    {
        public static void Otherwise_Throw(this bool value, string message = "")
        {
            if(!value)
                throw new InvalidOperationException(message);
        }
    }
}