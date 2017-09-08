using Lanski.Reactive;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lanski.Behaviours
{
    public class ClickSource : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
    {
        public IStream<PointerEventData> Clicks => _clicks_stream;
        public Stream<PointerEventData> Button_Downs => _button_down_stream;

        public void Awake()
        {
            Init();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Init();
            
            _clicks_stream.Next(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _button_down_stream.Next(eventData);
        }

        private void Init()
        {
            if (_initialized)
                return;
            _initialized = true;

            _clicks_stream = new Stream<PointerEventData>();
            _button_down_stream = new Stream<PointerEventData>();
        }

        private bool _initialized;
        private Stream<PointerEventData> _clicks_stream;
        private Stream<PointerEventData> _button_down_stream;
    }
}