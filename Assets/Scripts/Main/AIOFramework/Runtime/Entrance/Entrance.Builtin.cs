using UnityEngine;

namespace AIOFramework.Runtime
{
    public partial class Entrance : MonoBehaviour
    {
        public static BaseComponent Base { get; private set;}
        public static ResourceComponent Resource { get; private set;}
        private void InitBuiltinComponents()
        {
            Base = GameEntry.GetComponent<BaseComponent>();
            Resource = GameEntry.GetComponent<ResourceComponent>();
        }
    }
}