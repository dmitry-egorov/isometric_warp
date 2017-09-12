namespace WarpSpace.Models.Descriptions
{
    public enum Faction
    {
        the_Player_Faction,
        the_Natives
    }
    
    public static class FactionExtensions
    {
        public static bool Is_Hostile_Towards(this Faction faction, Faction other) => faction != other;
    }
}