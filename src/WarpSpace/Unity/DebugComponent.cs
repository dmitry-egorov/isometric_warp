using UnityEngine;

namespace WarpSpace.Unity
{
    public class DebugComponent: MonoBehaviour
    {
        void Awake()
        {
            Models.Debug.TheLog.Subscribe(Debug.Log);
        }
    }
}