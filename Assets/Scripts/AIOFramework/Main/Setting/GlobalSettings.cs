using UnityEngine;

namespace AIOFramework.Setting
{
    [CreateAssetMenu(fileName = "GlobalSettings", menuName = "AIOFramework/GlobalSettings")]
    public class GlobalSettings : ScriptableObject
    {
        [Header("GameSetting")][SerializeField] private GameSetting _gameSetting;
        public GameSetting GameSetting => _gameSetting;
    }
}