using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AIOFramework.UI
{
    public partial class UIComponent
    {
        [Serializable]
        private sealed class UIGroupDisplay
        {
            [SerializeField]
            private string _name = null;

            private int _depth = 0;

            public string Name
            {
                get
                {
                    return _name;
                }
            }

            public int Depth
            {
                get
                {
                    return _depth;
                }
            }
        }
    }
}