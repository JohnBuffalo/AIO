using System.Collections.Generic;

namespace AIOFramework.Runtime
{
    public enum UIGroupEnum : byte
    {
        Pool, //对象池层
        Background,
        HUD,
        Normal, //普通UI层
        Guide, //引导层
        Popup, //游戏类型弹窗
        Top, //Loading类Mask
        System //错误提示
    }
    public partial class UIComponent
    {
        public static Dictionary<UIGroupEnum, int> UIGroupSorting = new Dictionary<UIGroupEnum, int>()
        {
            { UIGroupEnum.Pool, 0 },
            { UIGroupEnum.Background, 1 },
            { UIGroupEnum.HUD, 1000 },
            { UIGroupEnum.Normal, 4000 },
            { UIGroupEnum.Guide, 7000 },
            { UIGroupEnum.Popup, 10000 },
            { UIGroupEnum.Top, 13000 },
            { UIGroupEnum.System, 16000 }
        };
    }
}