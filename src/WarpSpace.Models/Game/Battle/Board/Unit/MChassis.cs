﻿using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MChassis
    {
        public MLocation Location { get; private set; }

        public MChassis(MLocation initial_location, UnitType owner_type)
        {
            _chassis_type = owner_type.Get_Chassis_Type();
            Location = initial_location;
        }

        public bool Can_Move_To(MTile destination)
        {
            var source = Location;

            return destination.Is_Passable_By(_chassis_type)
                && source.Is_Adjacent_To(destination)
            ;
        }

        internal void Update_the_Location(MLocation new_location)
        {
            new_location.Is_Empty().Otherwise_Throw("Location is not empty");
                
            Location = new_location;
        }

        private readonly ChassisType _chassis_type;
    }
}