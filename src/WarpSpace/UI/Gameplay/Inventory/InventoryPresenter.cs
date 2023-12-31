﻿using Lanski.Behaviours;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Game;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay.Inventory
{
    [RequireComponent(typeof(Text))]
    public class InventoryPresenter: MonoBehaviour
    {
        void Start()
        {
            var cache = FindObjectOfType<IntStringsCache>();
            var text = GetComponent<Text>();
            FindObjectOfType<WGame>()
                .s_Players_Selected_Units_Cell
                .SelectMany(pu => pu.Select(u => u.s_Cell_of_Inventory_Contents()).Cell_Or_Single_Default())
                .Subscribe(content =>
                {
                    var the_amount_of_matter = content.s_Amount_of_Matter;
                    text.text = 
                        the_amount_of_matter != 0  
                            ? $"{cache.GetText(the_amount_of_matter)} matter"
                            : "No matter";
                });
        }
    }
}