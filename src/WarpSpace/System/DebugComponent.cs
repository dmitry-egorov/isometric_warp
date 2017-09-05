using UnityEngine;

namespace WarpSpace.System
{
    public class DebugComponent: MonoBehaviour
    {
        void Awake()
        {
            Models.Debug.TheLog.Subscribe(Debug.Log);
        }
    }
}