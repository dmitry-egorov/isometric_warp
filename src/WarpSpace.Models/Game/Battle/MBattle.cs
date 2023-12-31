﻿using Lanski.Reactive;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board;

namespace WarpSpace.Models.Game.Battle
{
    public class MBattle
    {
        public MBattle(DBoard the_board_desc, DUnit the_mothership_desc, SignalGuard the_signal_guard, MGame the_game)
        {
            its_board = new MBoard(the_board_desc, the_mothership_desc, the_signal_guard, the_game);
        }

        public MBoard s_Board => its_board;

        public void Starts() => s_Board.Warps_In_the_Mothership();
        public void Ends_the_Turn() => s_Board.Ends_the_Turn();

        private readonly MBoard its_board;
    }
}