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

            its_avalability_cell = it_selects_the_availability_cell();
        }

        public ICell<bool> s_Availability_Cell => its_avalability_cell;
        public bool has_a_Command_at(MTile the_tile, out UnitCommand the_command) => its_possible_command_at(the_tile).has_a_Value(out the_command);
        public bool @is(DUnitAction the_desc) => the_desc.Equals(its_desc);
        public bool is_Not_Available() => !this.s_Availability_Cell.s_Value; 

        private ICell<bool> it_selects_the_availability_cell()
        {
            if (its_desc.is_a_Fire_Action())
                return its_owner.s_Weapon.s_Can_Fire_Cell;

            if (its_desc.is_a_Move_Action())
                return its_owner.s_Cell_of_can_Move();

            if (its_desc.is_an_Interact_Action())
                return Cell.From(true);
            
            throw new InvalidOperationException("Unknow action type");
        }

        private Possible<UnitCommand> its_possible_command_at(MTile the_tile)
        {
            if (its_desc.is_a_Fire_Action())
            {
                var the_owners_weapon = its_owner.s_Weapon;
                if (the_owners_weapon.can_Fire_At(the_tile))
                    return UnitCommand.Create.Fire(the_owners_weapon, the_tile);
            }
            else if (its_desc.is_a_Move_Action())
            {
                if (its_owner.can_Move_To(the_tile))
                    return UnitCommand.Create.Move(its_owner, the_tile);
            }
            else if (its_desc.is_an_Interact_Action())
            {
                if (its_owner.can_Interact_With_a_Structure_At(the_tile, out var the_target_structure))
                    return UnitCommand.Create.Interact(its_owner, the_target_structure);
            }

            return Possible.Empty<UnitCommand>();
        }

        private readonly MUnit its_owner;
        private readonly DUnitAction its_desc;
        private readonly ICell<bool> its_avalability_cell;
    }
}