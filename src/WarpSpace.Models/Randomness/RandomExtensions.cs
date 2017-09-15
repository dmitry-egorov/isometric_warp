using System.Collections.Generic;

namespace WarpSpace.Models.Randomness
{
    public static class RandomExtensions
    {
        public static T Random_Element_Of<T>(this IRandom random, IReadOnlyList<T> list)
        {
            return list[random.Range(0, list.Count)];
        }
    }
}