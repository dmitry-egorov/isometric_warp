using System;
using WarpSpace.Models.Descriptions;

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(button_type), button_type, null);
            }
        }
    }
    
    public enum UIActionButtonType
    {
        Fire,
    }
}