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
            Debug.Log("Init inventory presenter");
            var cache = FindObjectOfType<IntStringsCache>();
            var text = GetComponent<Text>();
            FindObjectOfType<BattleComponent>().s_Players_Cell()
                .SelectMany(pp => pp.Select(p => p.s_Selected_Units_Cell).Cell_Or_Single_Default())
                .SelectMany(pu => pu.Select(u => u.s_Inventory_Contents_Cell()).Cell_Or_Single_Default())
                .Subscribe(possible_content =>
                {
                    text.text = 
                        possible_content.Has_a_Value(out var content1)  
                            ? $"{cache.GetText(content1.Matter)} matter"
                            : "No matter";
                });
        }
    }
}