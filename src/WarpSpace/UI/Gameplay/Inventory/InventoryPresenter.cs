using Lanski.Behaviours;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Game.Battle;

namespace WarpSpace.UI.Gameplay.Inventory
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(ShowWhenUnitSelected))]
    public class InventoryPresenter: MonoBehaviour
    {
        void Start()
        {
            var cache = FindObjectOfType<IntStringsCache>();
            var text = GetComponent<Text>();
            FindObjectOfType<BattleComponent>()
                .Player_Cell
                .SelectMany(pp => pp.Select(p => p.Selected_Unit_Cell).Cell_Or_Single_Default())
                .SelectMany(pu => pu.Select(u => u.Inventory.Content_Cell).Cell_Or_Single_Default())
                .Subscribe(possible_content =>
                {
                    text.text = 
                        possible_content.Has_a_Value(out var content)  
                        ? $"{cache.GetText(content.Matter)} matter"
                        : "No matter";
                });
        }
    }
}