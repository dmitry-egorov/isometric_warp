using System;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Overlay.Units;
using static WarpSpace.Game.Battle.Tile.HighlightType;

namespace WarpSpace.Game.Battle.Tile
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class WHighlight: MonoBehaviour
    {
        public Material MoveHighlight;
        public Material UnitHighlight;
        public Material InteractionHighlight;
        public Material UseWeaponHighlight;

        public void Start() => it_inits();
        public void Update() => it_updates();

        public void Updates_the_Highlight()
        {
            var the_highlight_type = its_highlight_type();

            this.its_highlight_becomes(the_highlight_type);
            
            HighlightType its_highlight_type() =>
                the_selected_unit_is_at_the_tile()                        ? Placeholder
                : !the_player.has_a_Command_At(its_tile, out var command) ? None  
                : command.is_a_Fire_Command()                             ? Attack
                : command.is_a_Tile_Move_Command()                        ? Move 
                : command.is_a_Dock_Command()                             ? Interact 
                : command.is_an_Interact_Command()                        ? Interact 
                                                                          : None
            ;
        }

        private void it_inits()
        {
            the_player = FindObjectOfType<WGame>().s_Player.must_have_a_Value();
            its_tile = GetComponentInParent<TileComponent>().s_Tile_Model;
            its_parents_mesh_filter = transform.parent.gameObject.GetComponent<MeshFilter>();
            its_mesh_filter = GetComponent<MeshFilter>();
            its_renderer = GetComponent<MeshRenderer>();
            
            it_builds_the_mesh();

            its_tile.s_Sites_Cell.Subscribe(_ => this.Updates_the_Highlight());

            it_is_initialized = true;
        }

        private void it_updates()
        {
            it_is_initialized.Must_Be_True_Otherwise_Throw("The WHighlight must be initialized before the first update");
            
            it_builds_the_mesh();
        }
        
        private void its_highlight_becomes(HighlightType type)
        {
            if (type == None)
            {
                gameObject.Hides();
            }
            else
            {
                its_renderer.sharedMaterial = SelectMaterial();
                gameObject.Shows();
            }

            Material SelectMaterial()
            {
                switch (type)
                {
                    case Move:
                        return MoveHighlight;
                    case Placeholder:
                        return UnitHighlight;
                    case Interact:
                        return InteractionHighlight;
                    case Attack:
                        return UseWeaponHighlight;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        private bool the_selected_unit_is_at_the_tile() => 
            the_player.has_a_Unit_Selected(out var the_selected_unit) && 
            the_selected_unit.is_At(its_tile)
        ;

        private void it_builds_the_mesh()
        {
            var the_mesh = its_parents_mesh_filter.sharedMesh;
            if (the_mesh == its_parents_last_mesh)
                return;
            
            its_mesh_filter.sharedMesh = the_mesh;
            its_parents_last_mesh = the_mesh;
        }

        private bool it_is_initialized;
        private MPlayer the_player;
        private MTile its_tile;
        private MeshFilter its_parents_mesh_filter;
        private MeshFilter its_mesh_filter;
        private MeshRenderer its_renderer;

        private Mesh its_parents_last_mesh;
    }
}