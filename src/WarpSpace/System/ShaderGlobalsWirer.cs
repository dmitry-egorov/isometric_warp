using Lanski.Behaviours.Overlay;
using UnityEngine;

namespace WarpSpace.System
{
    [ExecuteInEditMode]
    public class ShaderGlobalsWirer : MonoBehaviour
    {
        private bool _initialized;

        void Start()
        {
            Init();
        }

        void Update()
        {
            Init();
        }

        private void Init()
        {
            if (_initialized)
                return;
            _initialized = true;

            var reference = FindObjectOfType<ReferencePixels>();
            
            reference
                .PixelPerfectScaleCell
                .Subscribe(scale => Shader.SetGlobalFloat("_PixelPerfectScale", scale));

            reference
                .PixelScaleCell
                .Subscribe(scale => Shader.SetGlobalFloat("_PixelScale", scale));
        }
    }
}