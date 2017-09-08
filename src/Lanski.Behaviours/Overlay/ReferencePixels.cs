using Lanski.Reactive;
using UnityEngine;

namespace Lanski.Behaviours.Overlay
{
    public class ReferencePixels: MonoBehaviour
    {
        public float ReferencePixelPerfectHeight;

        private ChangeStream<float, int> _changes;
        
        private ICell<float> _pixel_perfect_scale_cell;
        private Cell<float> _pixel_scale_cell;

        public float PixelPerfectScale => PixelPerfectScaleCell.s_Value;
        public float PixelScale => PixelScaleCell.s_Value;
        
        public ICell<float> PixelPerfectScaleCell
        {
            get
            {
                Init();
                return _pixel_perfect_scale_cell;
            }
        }
        
        public ICell<float> PixelScaleCell
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
            _pixel_scale_cell = new Cell<float>(Calculate_Pixel_Perfect_Scale());
            _changes.Subscribe(_ => _pixel_scale_cell.s_Value = Calculate_Pixel_Perfect_Scale());
            _pixel_perfect_scale_cell = _pixel_scale_cell.Select(scale => Mathf.Max(1f, Mathf.Floor(scale)));
        }


        private float Calculate_Pixel_Perfect_Scale() => Screen.height / ReferencePixelPerfectHeight;
    }
}