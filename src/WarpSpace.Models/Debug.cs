using Lanski.Reactive;

namespace WarpSpace.Models
{
    public static class Debug
    {
        private static readonly Stream<string> _log = new Stream<string>();
        public static IStream<string> TheLog => _log;

        public static void Log(string message) => _log.Next(message);
        public static bool SemanticLog(string message)
        {
            Log(message);
            return true;
        }
    }
}