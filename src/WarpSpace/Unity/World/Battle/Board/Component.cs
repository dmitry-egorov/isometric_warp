using System;
using System.Collections.Generic;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Board.Unit.Weapon;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Unity.World.Battle.Board.Tile.Landscape;
using static WarpSpace.Unity.World.Battle.Board.Tile.Landscape.HighlightType;

namespace WarpSpace.Unity.World.Battle.Board
{
    public class Component : MonoBehaviour
    {
        public OwnSettings Settings;

        public void Init(BoardModel board, PlayerModel player)
        {
            this.DestroyChildren();

            var tile_components = CreateTiles();
            var components_map = new Dictionary<UnitModel, Unit.Component>();
            
            Wire_Units_Creation();
            Wire_the_Tile_Highlights();
            Wire_the_Selected_Units_Outline();

            Tile.Component[,] CreateTiles() => board.Tiles.Map((tile, index) =>
            {
                var n = board.Tiles.GetFitNeighbours(index).Map(t => t.Landscape.Type);
                return Tile.Component.Create(Settings.TilePrefab, transform, tile, n, board.Tiles.GetDimensions(), player);
            });

            Action Wire_Units_Creation() => board.UnitAddedStream.Subscribe(Create_and_Wire_a_Component_for_the_Unit);

            void Create_and_Wire_a_Component_for_the_Unit(BoardModel.UnitAdded e)
            {
                var unit = e.Unit;
                var tile = unit.CurrentTileCell.Value;
                var source_tile = e.SourceTile;
                var prefab = Select_the_Prefab();
                var tile_component = tile_components.Get(tile.Position);

                Create_the_Units_Component(); 

                void Create_the_Units_Component() => Unit.Component.Create(prefab, tile_component.UnitSlot.transform, unit, source_tile, tile_components, components_map);
                
                GameObject Select_the_Prefab()
                {
                    switch (e.Unit.Type)
                    {
                        case UnitType.Mothership: return Settings.MothershipPrefab;
                        case UnitType.Tank:       return Settings.TankPrefab;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            void Wire_the_Selected_Units_Outline()
            {
                player
                    .SelectedUnit
                    .IncludePrevious()
                    .Subscribe(x => SetOutline(x.previous, x.current));

                void SetOutline(Slot<UnitModel> possible_prev_unit, Slot<UnitModel> possible_cur_unit)
                {
                    var _ = 
                        possible_prev_unit.Has_a_Value(out var prev_unit)
                     && components_map
                        .Must_Have(prev_unit)
                        .Otherwise(new InvalidOperationException("Unit component wasn't found"))
                        .Disable_the_Outline();

                    var __ = 
                        possible_cur_unit.Has_a_Value(out var cur_unit)
                     && components_map
                        .Must_Have(cur_unit)
                        .Otherwise(new InvalidOperationException("Unit component wasn't found"))
                        .Enable_the_Outline();
                }
            }
            
            void Wire_the_Tile_Highlights()
            {
                player
                    .SelectionCell
                    .SelectMany(get_Selection_and_Tile)
                    .IncludePrevious()
                    .Subscribe(x =>
                    {
                        if (x.previous.has_a_value(out var prev))
                        {
                            reset_highlight_of_the(prev.tile);
                            reset_highlights_of_the_tiles_adjacent_to(prev.tile);
                        }

                        if (x.current.has_a_value(out var cur))
                        {
                            var tile = cur.tile;
                            var unit = cur.selection.Unit;
                            var weaponSlot = cur.selection.WeaponSlot;
                            
                            set_placeholder_highlight_of_the(tile);
                            set_highlights_of_the_tiles_adjacent_to(tile, unit, weaponSlot);
                        }
                    });
                
                IStream<(PlayerModel.PlayersSelection selection, TileModel tile)?> get_Selection_and_Tile(PlayerModel.PlayersSelection? selection_slot) => 
                    selection_slot.has_a_value(out var selection) 
                    ? selection.Unit.CurrentTileCell.Select(tile => NullableEx.From((selection, tile))) 
                    : new NullableCell<(PlayerModel.PlayersSelection, TileModel)>(null);

                bool reset_highlight_of_the(TileModel tile) => landscape_component_of(tile).set_highlight(None);
                void reset_highlights_of_the_tiles_adjacent_to(TileModel tile)
                {
                    foreach (var adjacent_tile_ref in tile.Adjacent.All)
                    {
                        if (adjacent_tile_ref.doesnt_have(out var adjacent_tile)) 
                            continue;

                        landscape_component_of(adjacent_tile).set_highlight(None);
                    }
                }

                bool set_placeholder_highlight_of_the(TileModel tile) => landscape_component_of(tile).set_highlight(HighlightType.Unit);

                void set_highlights_of_the_tiles_adjacent_to(TileModel tile, UnitModel selected_unit, Slot<WeaponModel> curWeapon)
                {
                    foreach (var adjacent_tile_slot in tile.Adjacent.All)
                    {
                        var _ =
                            adjacent_tile_slot.Has_a_Value(out var adjacent_tile) 
                         && the_highlight_for(selected_unit, adjacent_tile, curWeapon).@is(out var highlights_type)
                         && highlights_type.is_not(None)
                         && landscape_component_of(adjacent_tile).is_the(out var adjasent_landscapes_component)
                         && adjasent_landscapes_component.set_highlight(highlights_type);
                    }
                }

                Tile.Landscape.Component landscape_component_of(TileModel tile) => tile_components.Get(tile.Position).Landscape;
                TheHighlightFor the_highlight_for(UnitModel unit, TileModel tile, Slot<WeaponModel> weapon) => new TheHighlightFor(unit, tile, weapon);
            }
        }

        private struct TheHighlightFor
        {
            private readonly UnitModel _selected_unit;
            private readonly TileModel _target_tile;
            private readonly Slot<WeaponModel> _selected_weapon_slot;

            public TheHighlightFor(UnitModel selected_unit, TileModel target_tile, Slot<WeaponModel> selected_weapon_slot)
            {
                _selected_unit = selected_unit;
                _target_tile = target_tile;
                _selected_weapon_slot = selected_weapon_slot;
            }

            public bool @is(out HighlightType highlight)
            {
                var selected_weapon_slot = _selected_weapon_slot;
                var target_tile = _target_tile;
                var selected_unit = _selected_unit;
                
                return 
                      a_Weapon_Is_Selected(out var selected_weapon)
                   && there_Is_a_Unit_At_the_Tile(out var target_unit)
                   && selected_weapon.Can_Fire_At_the(target_unit)
                   && UseWeapon.Is_the_Value_Of_the(out highlight)
                      
                   || Weapon_Is_Not_Selected()
                   && selected_unit.Can_Move_To_the(target_tile)
                   && Move.Is_the_Value_Of_the(out highlight)
                      
                   || Weapon_Is_Not_Selected()
                   && target_tile.Has_a_Structure(out var target_structure)
                   && selected_unit.Can_Interact_With_the(target_structure)
                   && Interaction.Is_the_Value_Of_the(out highlight)
                      
                   || None.Is_the_Value_Of_the(out highlight)
                ;

                bool Weapon_Is_Not_Selected() => selected_weapon_slot.Has_Nothing();
                bool a_Weapon_Is_Selected(out WeaponModel weapon) => selected_weapon_slot.Has_a_Value(out weapon);
                bool there_Is_a_Unit_At_the_Tile(out UnitModel unit) => target_tile.Has_a_Unit(out unit);
            }
        }

        [Serializable]
        public struct OwnSettings
        {
            public GameObject TilePrefab;
            public GameObject MothershipPrefab;
            public GameObject TankPrefab;
        }
    }
}