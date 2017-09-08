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
        public Material Pressed_Material;
        public Material Normal_Material;
        
        public Stream<TheVoid> s_Presses_Stream => its_presses_stream;

        public void Awake()
        {
            its_image = GetComponent<Image>();
            the_settings = FindObjectOfType<UISettingsHolder>();
            its_presses_stream = new Stream<TheVoid>();
        }

        public void Becomes_Normal() => Becomes_Active_With(null, null);
        public void Becomes_Active_With(Material the_normal_material, Material the_pressed_material)
        {
            Normal_Material = the_normal_material;
            Pressed_Material = the_pressed_material;

            is_disabled = false;
            its_material_becomes_normal();
        }

        public void Becomes_Disabled()
        {
            its_image.material = the_settings.DisabledButtonMaterial;
            is_disabled = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (is_disabled)
                return;
            
            its_pressing_is_in_progress = true;
            its_material_becomes_pressed();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (is_disabled)
                return;
            
            its_material_becomes_normal();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (is_disabled)
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
            if (is_disabled)
                return;
            
            its_material_becomes_normal();
            s_Presses_Stream.Next();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (is_disabled)
                return;
            
            its_pressing_is_in_progress = false;
        }
        
        private void its_material_becomes_normal()
        {
            its_image.material = Normal_Material == null ? the_settings.NormalButtonMaterial : Normal_Material;
        }

        private void its_material_becomes_pressed()
        {
            its_image.material = Pressed_Material == null ? the_settings.PressedButtonMaterial : Pressed_Material;
        }
        
        private Image its_image;
        private UISettingsHolder the_settings;
        private Stream<TheVoid> its_presses_stream;

        private bool its_pressing_is_in_progress;
        private bool is_disabled;
        
    }
}