using Lanski.Behaviours;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Game.Battle;

namespace WarpSpace.UI.Gameplay.Inventory
{
    [RequireComponent(typeof(Text))]
    public class InventoryPresenter: MonoBehaviour
    {
        void Start()
        {
            var cache = FindObjectOfType<IntStringsCache>();
            var text = GetComponent<Text>();
            FindObjectOfType<BattleComponent>()
                .s_Players_Selected_Units_Cell
                .SelectMany(pu => pu.Select(u => u.s_Inventory_Contents_Cell).Cell_Or_Single_Default())
                .Subscribe(possible_content =>
                {
                    text.text = 
                        possible_content.has_a_Value(out var content)  
                            ? $"{cache.GetText(content.Matter)} matter"
                            : "No matter";
                });
        }
    }
}