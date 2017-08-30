namespace Lanski.Structures
{
    public static class NullableSemantics
    {
        public static bool theres_a<T>(Slot<T> r) where T : class => r.has_something();
        public static bool theres_no<T>(Slot<T> r) where T : class => r.Has_Nothing();
        
        public static bool theres_no<T>(T? obj) where T: struct => !theres_a(obj);
        public static bool theres_a<T>(T? obj) where T : struct => obj.HasValue; 
        
    }
}