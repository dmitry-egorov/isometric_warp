using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.UI.Gameplay.Weapon
{
    [RequireComponent(typeof(Image))]
    public class SelectionHighlight : MonoBehaviour
    {
        public Material NormalMaterial;
        public Material SelectedMaterial;
        
        public void Start()
        {
            var image = GetComponent<Image>();
            var battle = FindObjectOfType<BattleComponent>();
            Wire_Player_Slot_Variable();

            void Wire_Player_Slot_Variable() =>
                battle
                    .Player_Cell
                    .SelectMany(pp => pp.Select(p => p.Selected_Weapon_Cell).Cell_Or_Single_Default())
                    .Subscribe(Handle_Weapon_Selection)
            ;
            
            void Handle_Weapon_Selection(Possible<MWeapon> weapon) => 
                image.material = weapon.Has_a_Value() 
                    ? SelectedMaterial 
                    : NormalMaterial
            ;
        }
    }
}