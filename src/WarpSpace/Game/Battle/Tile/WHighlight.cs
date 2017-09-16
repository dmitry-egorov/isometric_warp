using Lanski.Structures;
using UnityEngine;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

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

        Possible<Material> its_current_material() =>
                  the_player.s_Selected_Unit_is_At(its_tile)              ? UnitHighlight
                : !the_player.has_a_Command_At(its_tile, out var command) ? Possible.Empty<Material>()  
                : command.is_a_Fire_Command()                             ? UseWeaponHighlight
                : command.is_a_Tile_Move_Command()                        ? MoveHighlight 
                : command.is_a_Dock_Command()                             ? InteractionHighlight
                : command.is_an_Interact_Command()                        ? InteractionHighlight
                                                                          : Possible.Empty<Material>()
        ;

        private void it_inits()
        {
            the_player = FindObjectOfType<WGame>().s_Player;
            its_tile = GetComponentInParent<WTile>().s_Tile_Model;
            its_parents_mesh_filter = transform.parent.gameObject.GetComponent<MeshFilter>();
            its_mesh_filter = GetComponent<MeshFilter>();
            its_renderer = GetComponent<MeshRenderer>();
            
            it_builds_the_mesh_if_neccessary();
            it_is_up_to_date = false;
            the_player.Performed_an_Action.Subscribe(x => it_is_up_to_date = false);

            it_is_initialized = true;
        }

        private void it_builds_the_mesh_if_neccessary()
        {
            var the_mesh = its_parents_mesh_filter.sharedMesh;
            if (the_mesh == its_parents_last_mesh)
                return;
            
            its_mesh_filter.sharedMesh = the_mesh;
            its_parents_last_mesh = the_mesh;
        }

        private void it_updates()
        {
            if (it_is_up_to_date)
                return;
            
            it_is_initialized.Must_Be_True_Otherwise_Throw("The WHighlight must be initialized before the first update");
            
            it_builds_the_mesh_if_neccessary();
            it_updates_the_material();

            it_is_up_to_date = true;
        }
        
        private void it_updates_the_material()
        {
            var the_possible_highlight_material = its_current_material();

            if (!the_possible_highlight_material.has_a_Value(out var the_material))
            {
                its_renderer.enabled = false;
            }
            else
            {
                its_renderer.sharedMaterial = the_material;
                its_renderer.enabled = true;
            }
        }

        private bool it_is_initialized;
        
        private MPlayer the_player;
        private MTile its_tile;
        private MeshFilter its_parents_mesh_filter;
        private MeshFilter its_mesh_filter;
        private MeshRenderer its_renderer;

        private bool it_is_up_to_date;
        private Mesh its_parents_last_mesh;
    }
}