using System;
using Lanski.Behaviours.Overlay;
using UnityEngine;

namespace WarpSpace.Overlay.Units
{
    [RequireComponent(typeof(RectTransform))]
    public class OPixelPerfectPanel : MonoBehaviour
    {
        private RectTransform _rect_transform;
        private Action _wiring;

        void Awake()
        {
            _rect_transform = GetComponent<RectTransform>();
            _wiring = Wire_the_Scale();

            Action Wire_the_Scale() => 
                FindObjectOfType<ReferencePixels>()
                    .PixelPerfectScaleCell
                    .Subscribe(scale => _rect_transform.localScale = new Vector3(scale, scale, 1))
            ;
        }

        void OnDestroy()
        {
            _wiring();
        }
    }
}