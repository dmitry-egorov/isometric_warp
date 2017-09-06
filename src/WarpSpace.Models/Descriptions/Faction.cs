namespace WarpSpace.Models.Descriptions
{
    public enum Faction
    {
        Player,
        Natives
    }
    
    public static class FactionExtensions
    {
        public static bool Is_Hostile_Towards(this Faction faction, Faction other) => faction != other;
    }
}