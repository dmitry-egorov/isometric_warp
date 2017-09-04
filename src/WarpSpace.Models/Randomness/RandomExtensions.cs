namespace WarpSpace.Models.Randomness
{
    public static class RandomExtensions
    {
        public static T Random_Element_Of<T>(this IRandom random, T[] array)
        {
            return array[random.Range(0, array.Length)];
        }
    }
}