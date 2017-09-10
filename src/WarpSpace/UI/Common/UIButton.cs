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
        }

        public void Becomes_Normal()
        {
            it_is_toggled = false;
            it_is_disabled = false;
        }

        public void Becomes_Disabled()
        {
            its_image.material = the_settings.DisabledButtonMaterial;
            it_is_disabled = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;
            
            its_pressing_is_in_progress = true;
            its_material_becomes_pressed();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;
            
            its_material_becomes_normal();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;
            
            if (its_pressing_is_in_progress)
            {
                its_material_becomes_pressed();
            }
            else
            {
                its_material_becomes_normal();
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;
            
            its_material_becomes_normal();
            s_Presses_Stream.Next();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (it_is_disabled)
                return;
            
            its_pressing_is_in_progress = false;
        }
        
        private void its_material_becomes_normal()
        {
            its_image.material = 
                !it_is_toggled ? the_settings.NormalButtonMaterial 
                : Warning_Mode ? the_settings.NormalWarningButtonMaterial
                               : the_settings.NormalHighlightedButtonMaterial
            ;
        }

        private void its_material_becomes_pressed()
        {
            its_image.material = 
                !it_is_toggled ? the_settings.PressedButtonMaterial 
                : Warning_Mode ? the_settings.PressedWarningButtonMaterial
                               : the_settings.PressedHighlightedButtonMaterial
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