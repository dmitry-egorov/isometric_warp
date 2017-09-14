namespace WarpSpace.Models.Game
{
    public class MFaction
    {
        public bool Is_Hostile_Towards(MFaction other) => this != other;
    }
}