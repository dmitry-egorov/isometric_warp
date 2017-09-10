using System;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay.Actions
{
    public static class UIActionButtonTypeExtensions
    {
        public static DUnitAction as_a_description(this UIActionButtonType button_type)
        {
            switch (button_type)
            {
                case UIActionButtonType.Fire:
                    return DUnitAction.Create.Fire();
                case UIActionButtonType.Deploy_0:
                    return DUnitAction.Create.Deploy(0);
                case UIActionButtonType.Deploy_1:
                    return DUnitAction.Create.Deploy(1);
                case UIActionButtonType.Deploy_2:
                    return DUnitAction.Create.Deploy(2);
                case UIActionButtonType.Deploy_3:
                    return DUnitAction.Create.Deploy(3);
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
        Deploy_0, 
        Deploy_1, 
        Deploy_2, 
        Deploy_3, 
        Dock, 
    }
}