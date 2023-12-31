﻿using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MUnitType
    {
        public readonly string s_Name;
        public readonly int s_Total_Hit_Points;
        public readonly int s_Total_Moves;
        public readonly MWeaponType s_Weapon_Type;
        public readonly MChassisType s_Chassis_Type;
        public readonly DStuff s_Remains;
        public readonly DStuff s_Initial_Inventory_Content;
        public readonly bool can_Exit;
        public readonly char s_Serialization_Symbol;
        
        public MUnitType(string the_name, int the_total_hit_points, int the_total_moves, MWeaponType the_weapon_type, MChassisType the_chassis_type, DStuff the_remains, DStuff the_initial_inventory_content, bool the_can_exit, char the_serialization_symbol)
        {
            s_Name = the_name;
            s_Remains = the_remains;
            s_Total_Moves = the_total_moves;
            s_Total_Hit_Points = the_total_hit_points;
            s_Weapon_Type = the_weapon_type;
            s_Chassis_Type = the_chassis_type;
            can_Exit = the_can_exit;
            s_Serialization_Symbol = the_serialization_symbol;
            s_Initial_Inventory_Content = the_initial_inventory_content;
        }

        public override string ToString() => s_Name;
    }
}