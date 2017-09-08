using Lanski.Reactive;
using UnityEngine;

namespace Lanski.Behaviours.Input
{
    public class GlobalMouseEventsSource: MonoBehaviour
    {
        public Stream<Vector3> s_Mouse_Button_Ups_Stream => its_mouse_button_ups_stream;

        public void Awake()
        {
            its_mouse_button_ups_stream = new Stream<Vector3>();
        }

        public void Update()
        {
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                s_Mouse_Button_Ups_Stream.Next(UnityEngine.Input.mousePosition);
            }
        }
        
        private Stream<Vector3> its_mouse_button_ups_stream;
    }
}