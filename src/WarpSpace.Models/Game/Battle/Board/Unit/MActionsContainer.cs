using System;
using System.Collections.Generic;
using Lanski.Structures;
using Lanski.SwiftLinq;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MActionsContainer
    {
        public MActionsContainer(MUnit the_owner)
        {
            its_fire = new MUnitAction(the_owner, DUnitAction.Create.Fire());
            its_move = new MUnitAction(the_owner, DUnitAction.Create.Move());
            its_interact = new MUnitAction(the_owner, DUnitAction.Create.Interact());

            its_regular_actions = new[] { its_move, its_interact };
        }
        
        public Possible<MUnitAction> s_possible_Action_For(DUnitAction the_action_desc)
        {
            if (the_action_desc.is_a_Fire_Action())
                return its_fire;
            if (the_action_desc.is_a_Move_Action())
                return its_move;
            if (the_action_desc.is_an_Interact_Action())
                return its_interact;
            
            throw new InvalidOperationException("Unknown action type");
        }

        public Possible<UnitCommand> s_Regular_Command_At(MTile the_tile)
        {
            var it = its_regular_actions.s_New_Iterator();
            while (it.has_a_Value(out var the_regular_action))
            {
                if (the_regular_action.has_a_Command_at(the_tile, out var the_command))
                    return the_command;
            }

            return Possible.Empty<UnitCommand>();
        }

        private readonly MUnitAction its_fire;
        private readonly MUnitAction its_move;
        private readonly MUnitAction its_interact;
        private readonly IReadOnlyList<MUnitAction> its_regular_actions;
    }
}