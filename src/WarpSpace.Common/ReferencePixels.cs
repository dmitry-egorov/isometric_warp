using Lanski.Reactive;
using UnityEngine;

namespace WarpSpace.Common
{
    public class ReferencePixels: MonoBehaviour
    {
        public float ReferencePixelPerfectHeight;

        private ChangeStream<float, int> _changes;
        
        private ICell<float> _pixel_perfect_scale_cell;
        private ValueCell<float> _pixel_scale_cell;
        
        public ICell<float> PixelPixelPerfectScaleCell
        {
            get
            {
                Init();
                return _pixel_perfect_scale_cell;
            }
        }
        
        public ICell<float> PixelPixelScaleCell
        {
            get
            {
                Init();
                return _pixel_scale_cell;
            }
        }

        void Update()
        {
            Init();
            
            _changes.Update(ReferencePixelPerfectHeight, Screen.height);
        }

        private void Init()
        {
            if (_changes != null) 
                return;
            
            _changes = new ChangeStream<float, int>();
            _pixel_scale_cell = new ValueCell<float>(Calculate_Pixel_Perfect_Scale());
            _changes.Subscribe(() => _pixel_scale_cell.Value = Calculate_Pixel_Perfect_Scale());
            _pixel_perfect_scale_cell = _pixel_scale_cell.Select(scale => Mathf.Floor(scale * 2f) / 2f);
        }


        private float Calculate_Pixel_Perfect_Scale() => Screen.height / ReferencePixelPerfectHeight;
    }
}