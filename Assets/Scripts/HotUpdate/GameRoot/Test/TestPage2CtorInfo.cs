using UnityEngine;

namespace AIOFramework.Runtime
{
    public class TestPage2CtorInfo : UICtorInfo
    {
        public override string Location { get; } = "Assets/ArtAssets/UI/Tester/TestPage_2.prefab";
        public override UIGroupEnum Group { get; } = UIGroupEnum.Normal;

        public override bool PauseCoveredUI { get; } = true;
        
        public override bool Multiple { get; } = true;
    }
}