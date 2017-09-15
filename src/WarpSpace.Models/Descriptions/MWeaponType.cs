namespace WarpSpace.Models.Descriptions
{
    public class MWeaponType
    {
        public readonly int s_Max_Uses;
        public readonly DDamage s_Damage;

        public MWeaponType(int the_max_uses, DDamage the_damage)
        {
            s_Max_Uses = the_max_uses;
            s_Damage = the_damage;
        }
    }
}