using System;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay.Actions
{
    public static class UIActionButtonTypeExtensions
    {
        public static DUnitAction as_a_description(this UIActionButtonType button_type, int index)
        {
            switch (button_type)
            {
                case UIActionButtonType.Fire:
                    return DUnitAction.Create.Fire();
                case UIActionButtonType.Deploy:
                    return DUnitAction.Create.Deploy(index);
                case UIActionButtonType.Dock:
                    return DUnitAction.Create.Dock();
                default:
                    throw new ArgumentOutOfRangeException(nameof(button_type), button_type, null);
            }
        }
    }
    
    public enum UIActionButtonType
    {
        Fire, 
        Deploy, 
        Dock, 
    }
}