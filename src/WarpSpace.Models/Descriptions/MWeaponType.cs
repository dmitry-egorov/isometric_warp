namespace WarpSpace.Models.Descriptions
{
    public class MWeaponType
    {
        public readonly int s_Max_Uses;
        public readonly DDamage s_Damage;

        public MWeaponType(string the_name, int the_max_uses, DDamage the_damage)
        {
            its_name = the_name;
            s_Max_Uses = the_max_uses;
            s_Damage = the_damage;
        }

        public override string ToString() => its_name;

        private readonly string its_name;
    }
}