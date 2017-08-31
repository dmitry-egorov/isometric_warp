using System;
using System.Collections.Generic;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Unity.World.Battle.Board
{
    public static class OutlinesWiring
    {
        public static void Wire(PlayerModel player, Dictionary<UnitModel, Unit.Component> components_map)
        {
            player
                .SelectedUnit
                .IncludePrevious()
                .Subscribe(x => SetOutline(x.previous, x.current))
                ;

            void SetOutline(Slot<UnitModel> possible_prev_unit, Slot<UnitModel> possible_cur_unit)
            {
                if (possible_prev_unit.Has_a_Value(out var prev_unit))
                    components_map
                        .Must_Have(prev_unit).Otherwise(new InvalidOperationException("Unit component wasn't found"))
                        .Disable_the_Outline();

                if (possible_cur_unit.Has_a_Value(out var cur_unit))
                    components_map
                        .Must_Have(cur_unit).Otherwise(new InvalidOperationException("Unit component wasn't found"))
                        .Enable_the_Outline();
            }
        }
    }
}