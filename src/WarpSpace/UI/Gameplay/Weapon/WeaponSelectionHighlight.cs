using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Models.Game.Battle.Board.Unit.Weapon;

namespace WarpSpace.UI.Gameplay.Weapon
{
    [RequireComponent(typeof(Image))]
    public class WeaponSelectionHighlight : MonoBehaviour
    {
        public Material NormalMaterial;
        public Material SelectedMaterial;
        
        public void Start()
        {
            var image = GetComponent<Image>();
            var battle = FindObjectOfType<Unity.World.Battle.Component>();
            Wire_Player_Slot_Variable();

            void Wire_Player_Slot_Variable() =>
                battle
                    .Player_Cell
                    .SkipEmpty()
                    .SelectMany(p => p.Selected_Weapon_Cell)
                    .Subscribe(Handle_Weapon_Selection)
            ;
            
            void Handle_Weapon_Selection(Slot<WeaponModel> weapon) => 
                image.material = weapon.Has_a_Value() 
                    ? SelectedMaterial 
                    : NormalMaterial
            ;
        }
    }
}