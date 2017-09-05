using WarpSpace.Models.Randomness;
using Random = UnityEngine.Random;

namespace WarpSpace.Services
{
    internal class UnityRandom : IRandom
    {
        public float Range(float min_inclusive, float max_inclusive) => Random.Range(min_inclusive, max_inclusive);
        public int Range(int min_inclusive, int max_exclusive) => Random.Range(min_inclusive, max_exclusive);
    }
}