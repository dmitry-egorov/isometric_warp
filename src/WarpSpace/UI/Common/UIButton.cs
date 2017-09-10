using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WarpSpace.Settings;

namespace WarpSpace.UI.Common
{
    [RequireComponent(typeof(Image))]
    public class UIButton: MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
    {
        public bool Warning_Mode;
        public Stream<TheVoid> s_Presses_Stream => its_presses_stream;

        public void Awake()
        {
            its_image = GetComponent<Image>();
            the_settings = FindObjectOfType<UISettingsHolder>();
            its_presses_stream = new Stream<TheVoid>();
        }

        public void Becomes_Toggled()
        {
            it_is_disabled = false;
            it_is_toggled = true;
            updates_the_material();
        }

        public void Becomes_Normal()
        {
            it_is_toggled = false;
            it_is_disabled = false;
            updates_the_material();
        }

        public void Becomes_Disabled()
        {
            it_is_disabled = true;
            updates_the_material();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;
            
            its_pressing_is_in_progress = true;
            updates_the_material();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;

            updates_the_material();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;
            updates_the_material();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;

            updates_the_material();
            s_Presses_Stream.Next();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;
            
            its_pressing_is_in_progress = false;
        }
        
        private void updates_the_material()
        {
            its_image.material = it_is_disabled ? the_settings.DisabledButtonMaterial
                                                :
                its_pressing_is_in_progress ?
                !it_is_toggled ? the_settings.PressedButtonMaterial 
                : Warning_Mode ? the_settings.PressedWarningButtonMaterial
                               : the_settings.PressedHighlightedButtonMaterial
                                            :
                !it_is_toggled ? the_settings.NormalButtonMaterial 
                : Warning_Mode ? the_settings.NormalWarningButtonMaterial
                               : the_settings.NormalHighlightedButtonMaterial
                ;
            ;
        }
        
        private Image its_image;
        private UISettingsHolder the_settings;
        private Stream<TheVoid> its_presses_stream;

        private bool its_pressing_is_in_progress;
        private bool it_is_disabled;
        private bool it_is_toggled;

    }
}