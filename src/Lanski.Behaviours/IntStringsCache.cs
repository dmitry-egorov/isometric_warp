using System.Linq;
using UnityEngine;

namespace Lanski.Behaviours
{
    public class IntStringsCache: MonoBehaviour
    {
        private static string[] _prebuiltStrings;

        void Start()
        {
            Init();
        }

        public string GetText(int amount)
        {
            Init();
            
            return amount < _prebuiltStrings.Length 
                ? _prebuiltStrings[amount] 
                : amount.ToString();
        }

        private void Init()
        {
            if (_prebuiltStrings != null)
                return;

            _prebuiltStrings = Enumerable
                .Range(0, 120)
                .Select(x => x.ToString())
                .ToArray();
        }
    }
}