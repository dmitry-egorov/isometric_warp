using WarpSpace.Descriptions;

namespace WarpSpace.Models.Randomness
{
    public static class RandomnessExtensions
    {
        public static LandscapeType Random_Landscape_Type(this IRandom random)
        {
            return (LandscapeType)random.Range(0, 4);
        }
    }
}