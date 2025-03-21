using UnityEngine;
using AIOFramework.UI;

namespace AIOFramework.Runtime
{
    public class TestPageCtorInfo : UICtorInfo
    {
        public override string Location { get; } = "Assets/ArtAssets/UI/Tester/TestPage.prefab";
        public override UIGroupEnum Group { get; } = UIGroupEnum.Normal;
        public override bool PauseCoveredUI { get; } = true;
        public override bool Multiple { get; } = false;
        
        public string Tips { get; set; }
        public int Index { get; set; }

        public override void Clear()
        {
            base.Clear();
            Tips = null;
            Index = 0;
        }

    }
    
    public class TestPageCtorInfo2 : UICtorInfo
    {
        public override string Location { get; } = "Assets/ArtAssets/UI/Tester/TestPage2.prefab";
        public override UIGroupEnum Group { get; } = UIGroupEnum.Normal;
        public override bool PauseCoveredUI { get; } = true;
        public override bool Multiple { get; } = false;
        
        public string Tips { get; set; }
        public int Index { get; set; }
        
        public override void Clear()
        {
            base.Clear();
            Tips = null;
            Index = 0;
        }
    }
    
    public class TestWindowCtorInfo : UICtorInfo
    {
        public override string Location { get; } = "Assets/ArtAssets/UI/Tester/TestWindow.prefab";
        public override UIGroupEnum Group { get; } = UIGroupEnum.Popup;
        public override bool PauseCoveredUI { get; } = false;
        public override bool Multiple { get; } = true;
        
        public string Tips { get; set; }
        public int Index { get; set; }

        public override void Clear()
        {
            base.Clear();
            Tips = null;
            Index = 0;
        }
    }
}