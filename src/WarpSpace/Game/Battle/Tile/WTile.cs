using Lanski.Structures;
using UnityEngine;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Tile
{
    public class WTile : MonoBehaviour
    {
        public WUnitSlot s_Unit_Slot => its_unit_slot;
        public MTile s_Tile_Model => its_tile_model;
        public WTasksHolder s_Tasks_Holder => its_tasks_holder;

        public static WTile Create(WTile prefab, Transform parent, MTile tile, Dimensions2D dimensions, MPlayer player)
        {
            var component = Instantiate(prefab, parent);
            component.Init(tile, dimensions, player);
            return component;
        }

        private void Init(MTile tile, Dimensions2D dimensions, MPlayer player)
        {
            its_tile_model = tile;
            its_tasks_holder = new WTasksHolder();
            var position = tile.s_Position;

            transform.localPosition = GetPosition(position, dimensions);
            name = $"Tile ({position.Column}, {position.Row})";
            
            var structureSlot = GetComponentInChildren<StructureSlot>();
            var playerActionsDetector = GetComponentInChildren<WPlayerActionSource>();

            its_unit_slot = GetComponentInChildren<WUnitSlot>();

            structureSlot.Init(tile);

            playerActionsDetector.Init();
            
            WirePlayerActionsDetector();

            void WirePlayerActionsDetector() => 
                playerActionsDetector
                .Actions
                .Subscribe(_ => player.Executes_a_Command_At(tile));
        }

        private static Vector3 GetPosition(Index2D i, Dimensions2D dimensions) => new Vector3(i.Column - dimensions.Columns * 0.5f, 0, dimensions.Rows * 0.5f - i.Row);
        
        private MTile its_tile_model;
        private WUnitSlot its_unit_slot;
        private WTasksHolder its_tasks_holder;
    }
}