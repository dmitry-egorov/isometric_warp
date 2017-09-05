using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class NullableStreamExtensions
    {
        public static IStream<T> Value_Or_Empty<T>(this Possible<IStream<T>> possible_stream) where T : struct => possible_stream.Select(c => c).Value_Or(Stream.Empty<T>());
    }
}