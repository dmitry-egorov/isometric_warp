using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MUnitType
    {
        public readonly int s_Total_Hit_Points;
        public readonly int s_Total_Moves;
        public readonly int s_Bay_Size;
        public readonly WeaponType s_Weapon_Type;
        public readonly MChassisType s_Chassis_Type;
        public readonly Possible<DStuff> s_Loot;
        public readonly Possible<DStuff> s_Initial_Inventory_Content;
        public readonly bool can_Dock;
        public readonly bool can_Exit;
        public readonly char s_Serialization_Symbol;
        
        public MUnitType(int the_total_hit_points, int the_total_moves, int the_bay_size, WeaponType the_weapon_type, MChassisType the_chassis_type, Possible<DStuff> the_loot, Possible<DStuff> the_initial_inventory_content, bool the_can_dock, bool the_can_exit, char the_serialization_symbol)
        {
            s_Loot = the_loot;
            s_Bay_Size = the_bay_size;
            s_Total_Moves = the_total_moves;
            s_Total_Hit_Points = the_total_hit_points;
            s_Weapon_Type = the_weapon_type;
            s_Chassis_Type = the_chassis_type;
            can_Dock = the_can_dock;
            can_Exit = the_can_exit;
            s_Serialization_Symbol = the_serialization_symbol;
            s_Initial_Inventory_Content = the_initial_inventory_content;
        }
    }
}