using System.Collections;
using System.Collections.Generic;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.SwiftLinq;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle.Tile;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Board
{
    public class WBoard : MonoBehaviour
    {
        public WTile TilePrefab;
        public WUnit UnitPrefab;
        
        public IStream<Possible<MBattle>> s_New_Battles_Stream => its_new_Battle_stream;
        public IStream<WUnit> s_Created_Units_Stream => its_created_units_stream;

        public WTile this[Index2D the_position] => its_tiles.Get(the_position);
        public WTile this[MTile the_tile] => this[the_tile.s_Position];
        public WUnit s_Unit_For(MUnit the_source_unit) => its_units_map[the_source_unit];
        public WUnitSlot s_Slot_Of(Index2D the_position) => this[the_position].s_Unit_Slot;

        public IEnumerator Ends_the_Turn()
        {
            var the_player = its_game.s_Player;
            the_player.Suspends_Until_the_Turns_End();

            yield return this.Fast_Forwards_All_Movements();

            the_player.Ends_the_Turn_and_Resumes();
        }

        public IEnumerator Fast_Forwards_All_Movements()
        {
            it_bosts_all_movements();
            while (its_units_are_moveng)
                yield return null;
            resumes_all_movements_to_normal_speed();
        }

        void Awake()
        {
            its_created_units_stream = new RepeatAllStream<WUnit>();
            its_new_Battle_stream = new RepeatAllStream<Possible<MBattle>>();
            its_limbo = FindObjectOfType<WLimbo>();
            its_transform = transform;
            its_game = GetComponentInParent<WGame>();
            its_units = new List<WUnit>();
            its_units_map = new Dictionary<MUnit, WUnit>();

            its_created_units_stream.Subscribe(wunit =>
            {
                its_units.Add(wunit);//Note: use a better structure for performance
                its_units_map.Add(wunit.s_Unit, wunit);
                wunit.s_Destruction_Signal.Subscribe(_ =>
                {
                    its_units.Remove(wunit);
                    its_units_map.Remove(wunit.s_Unit);
                });
            });
        }

        void Start()
        {
            its_game.s_Battles_Cell
                .Subscribe(battle => it_handles_a_new_battle(battle))
            ;
            
            its_game.s_Battles_Cell
                .SelectMany_Or_Empty(the_battle => the_battle.s_Board.Created_a_Unit)
                .Subscribe(the_unit => it_creates_a_unit_from(the_unit))
            ;
        }
        
        private bool its_units_are_moveng => its_units.SAny(unit => unit.is_Moving);

        private void it_handles_a_new_battle(Possible<MBattle> the_possible_battle)
        {
            gameObject.DestroyChildren();
            its_limbo.Destroys_All_Children();
            its_units.Clear();
            its_units_map.Clear();
            
            its_new_Battle_stream.Next(the_possible_battle);

            if (!the_possible_battle.has_a_Value(out var the_battle))
                return;

            var the_board = the_battle.s_Board;

            var the_player = its_game.s_Player;
            its_tiles = it_creates_the_tiles(the_board.s_Tiles, the_player);
        }

        private WTile[,] it_creates_the_tiles(MTile[,] the_board_tiles, MPlayer the_player) => 
            the_board_tiles.Map((tile, index) => WTile.Create(TilePrefab, its_transform, tile, the_board_tiles.s_Dimensions(), the_player))
        ;

        private void it_creates_a_unit_from(MUnit the_model_unit)
        {
            var world_unit = WUnit.Is_Created_From(UnitPrefab, the_model_unit);
                
            its_created_units_stream.Next(world_unit);
        }
        
        private void it_bosts_all_movements() => its_units.SForEach(the_unit =>the_unit.Fast_Forwards_the_Movement());
        private void resumes_all_movements_to_normal_speed() => its_units.SForEach(the_unit =>the_unit.Resumes_the_Movement_To_Normal_Speed());

        private RepeatAllStream<WUnit> its_created_units_stream;
        private RepeatAllStream<Possible<MBattle>> its_new_Battle_stream;
        private List<WUnit> its_units;
        private Dictionary<MUnit, WUnit> its_units_map;
        private WTile[,] its_tiles;
        private WLimbo its_limbo;
        private Transform its_transform;
        private WGame its_game;
    }
}