using UnityEngine;

namespace AIOFramework.Runtime
{
    public class TestPageCtorInfo : UICtorInfo
    {
        public override string Location { get; } = "Assets/ArtAssets/UI/Tester/TestPage.prefab";
        public override UIGroupEnum Group { get; } = UIGroupEnum.Normal;
        public override bool PauseCoveredUI { get; } = false;
        public override bool Multiple { get; } = false;
    }
}