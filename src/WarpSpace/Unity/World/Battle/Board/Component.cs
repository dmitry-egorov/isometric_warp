using System;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Models.Game.Battle.Player;
using BoardModel = WarpSpace.Models.Game.Battle.Board.Model;
using UnitAdded = WarpSpace.Models.Game.Battle.Board.Model.UnitAdded;

namespace WarpSpace.Unity.World.Battle.Board
{
    public class Component : MonoBehaviour
    {
        public OwnSettings Settings;

        public void Init(BoardModel board, Model player)
        {
            this.DestroyChildren();

            var tile_components = CreateTiles();
            
            WireUnitCreation();
            WireTileHighlights();
            
            Tile.Component[,] CreateTiles()
            {
                return board.Tiles.Map((tile, index) =>
                {
                    var n = board.Tiles.GetFitNeighbours(index).Map(t => t.Landscape.Type);
                    return Tile.Component.Create(Settings.TilePrefab, transform, tile, n, board.Tiles.GetDimensions(), player);
                });
            }
            
            void WireUnitCreation()
            {
                board.UnitAddedStream.Subscribe(Create_and_Wire_a_Component_for_the_Unit);
            }
            
            void Create_and_Wire_a_Component_for_the_Unit(UnitAdded e)
            {
                var unit = e.Unit;
                var tile = unit.CurrentTile.Value;
                var source_tile = e.SourceTile;
                var rotation = source_tile.GetDirectionTo(tile).ToRotation();   
                
                var tile_component = tile_components.Get(tile.Position);
                var unit_component = Unit.Component.Create(Settings.MothershipPrefab, rotation, tile_component.UnitSlot.transform); // TODO: Select unit prefab

                WireUnitMovement();
                WireUnitSelection();
                
                void WireUnitMovement()
                {
                    unit
                        .CurrentTile
                        .IncludePrevious()
                        .Subscribe(x =>
                        {
                            var cur_tile = tile_components.Get(x.current.Position);
                            var prev_tile = tile_components.Get((x.previous ?? source_tile).Position);

                            var orientation = prev_tile.Model.GetDirectionTo(cur_tile.Model);
                                
                            unit_component.MoveTo(cur_tile, orientation);
                        });
                }
                
                void WireUnitSelection()
                {
                    player
                        .SelectedUnitCell
                        .IncludePrevious()
                        .Subscribe(x =>
                        {
                            if (x.previous == unit)
                                unit_component.DisableOutline();
    
                            if (x.current == unit)
                                unit_component.EnableOutline();
                        });
                }
            }
            
            void WireTileHighlights()
            {
                player
                    .SelectedUnitCell
                    .SelectMany(unit => unit?.CurrentTile.Select(tile => (unit, tile)))
                    .IncludePrevious()
                    .Subscribe(x =>
                    {
                        var prevTile = x.previous.Item2;
                        var curTile = x.current.Item2;
                        var curUnit = x.current.Item1;

                        if (prevTile != null)
                        {
                            tile_components.Get(prevTile.Position).Landscape.ResetHighlight();
                            foreach (var tile in prevTile.Adjacent.All)
                            {
                                if (tile != null)
                                    tile_components.Get(tile.Position).Landscape.ResetHighlight();
                            }
                        }

                        tile_components.Get(curTile.Position).Landscape.SetUnitHighlight();
                        foreach (var tile in curTile.Adjacent.All)
                        {
                            if (tile != null && curUnit.CanMoveTo(tile))
                                tile_components.Get(tile.Position).Landscape.SetMovementHighlight();
                        }
                    });
            }
        }

        [Serializable]
        public struct OwnSettings
        {
            public GameObject TilePrefab;
            public GameObject MothershipPrefab;
        }
    }
}