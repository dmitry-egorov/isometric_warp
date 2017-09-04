namespace WarpSpace.Models.Randomness
{
    public interface IRandom
    {
        float Range(float min_inclusive, float max_inclusive);
        int Range(int min_inclusive, int max_exclusive);
    }
}