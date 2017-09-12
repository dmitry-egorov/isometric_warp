using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MUnitAction
    {
        public MUnitAction(MUnit the_owner, DUnitAction the_desc)
        {
            its_owner = the_owner; 
            its_desc = the_desc;

            its_avalability_cell = selects_the_availability_cell();
            
            ICell<bool> selects_the_availability_cell()
            {
                if (its_desc.is_a_Fire_Action())
                    return its_owner.s_Weapon.s_Can_Fire_Cell;

                if (its_desc.is_a_Deploy_Action(out var the_deploy))
                    return its_owner.s_can_Deploy_Cell(the_deploy.s_bay_slot_index);

                if (its_desc.is_a_Dock_Action())
                    return its_owner.s_can_Move_Cell;

                if (its_desc.is_a_Move_Action())
                    return its_owner.s_can_Move_Cell;

                if (its_desc.is_an_Interact_Action())
                    return Cell.From(true);
            
                throw new InvalidOperationException("Unknow action type");
            }
        }

        public ICell<bool> s_Availability_Cell => its_avalability_cell;
        
        public Possible<UnitCommand> s_possible_Command_at(MTile the_tile)
        {
            if (its_desc.is_a_Fire_Action())
            {
                var the_owners_weapon = its_owner.s_Weapon;
                if (the_owners_weapon.can_Fire_At_a_Unit_At(the_tile, out var the_target_unit))
                    return UnitCommand.Create.Fire(the_owners_weapon, the_target_unit);
            }
            else if (its_desc.is_a_Deploy_Action(out var the_deploy))
            {
                if (its_owner.can_Deploy(the_deploy.s_bay_slot_index, the_tile, out var the_docked_unit, out var the_target_location))
                    return UnitCommand.Create.Move(the_docked_unit, the_target_location);
            }
            else if (its_desc.is_a_Dock_Action())
            {
                if (its_owner.can_Dock_At(the_tile, out var the_target_location))
                    return UnitCommand.Create.Move(its_owner, the_target_location);
            }
            else if (its_desc.is_a_Move_Action())
            {
                if (its_owner.can_Move_To(the_tile, out var the_tiles_location))
                    return UnitCommand.Create.Move(its_owner, the_tiles_location);
            }
            else if (its_desc.is_an_Interact_Action())
            {
                if (its_owner.can_Interact_With_a_Structure_At(the_tile, out var the_target_structure))
                    return UnitCommand.Create.Interact(its_owner, the_target_structure);
            }
            
            return  Possible.Empty<UnitCommand>();
        }
        
        public bool @is(DUnitAction the_desc) => the_desc.Equals(its_desc);
        
        private readonly MUnit its_owner;
        private readonly DUnitAction its_desc;
        private readonly ICell<bool> its_avalability_cell;
    }
}