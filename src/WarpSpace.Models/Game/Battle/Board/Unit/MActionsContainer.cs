using System;
using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;
using Lanski.SwiftLinq;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    internal class MActionsContainer
    {
        public MActionsContainer(MUnit the_owner)
        {
            its_fire = new MUnitAction(the_owner, DUnitAction.Create.Fire());
            its_deploy_actions = it_creates_the_deploy_actions(the_owner);
            its_dock = the_owner.has_a_Docker().as_a_Possible().Select(_ => new MUnitAction(the_owner, DUnitAction.Create.Dock()));
            its_move = new MUnitAction(the_owner, DUnitAction.Create.Move());
            its_interact = new MUnitAction(the_owner, DUnitAction.Create.Interact());

            its_regular_actions = new[] { its_move, its_interact };
        }
        
        public Possible<MUnitAction> s_possible_Action_For(DUnitAction the_action_desc)
        {
            if (the_action_desc.is_a_Fire_Action())
                return its_fire;
            if (the_action_desc.is_a_Deploy_Action(out var the_deploy))
                return its_deploy_actions.s_possible_Value_At(the_deploy.s_bay_slot_index);
            if (the_action_desc.is_a_Dock_Action())
                return its_dock;
            if (the_action_desc.is_a_Move_Action())
                return its_move;
            if (the_action_desc.is_an_Interact_Action())
                return its_interact;
            
            throw new InvalidOperationException("Unknown action type");
        }

        public Possible<UnitCommand> s_Regular_Command_At(MTile the_tile)
        {
            var it = its_regular_actions.s_new_iterator();
            while (it.has_a_Value(out var the_regular_action))
            {
                if (the_regular_action.s_possible_Command_at(the_tile).has_a_Value(out var the_command))
                    return the_command;
            }

            return Possible.Empty<UnitCommand>();
        }
            
        private static IReadOnlyList<MUnitAction> it_creates_the_deploy_actions(MUnit the_owner) => 
            the_owner.s_Bay_Size
                .counted()
                .Select(the_bay_slot_index => new MUnitAction(the_owner, DUnitAction.Create.Deploy(the_bay_slot_index)))
                .ToArray()
        ;

        private readonly MUnitAction its_fire;
        private readonly IReadOnlyList<MUnitAction> its_deploy_actions;
        private readonly Possible<MUnitAction> its_dock;
        private readonly MUnitAction its_move;
        private readonly MUnitAction its_interact;
        private readonly IReadOnlyList<MUnitAction> its_regular_actions;
    }
}