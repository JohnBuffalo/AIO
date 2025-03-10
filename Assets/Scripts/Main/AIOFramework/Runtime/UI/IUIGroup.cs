
using System.Collections.Generic;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 界面组接口。
    /// </summary>
    public interface IUIGroup
    {
        /// <summary>
        /// 获取界面组名称。
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 获取或设置界面组深度。
        /// </summary>
        int Depth
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置界面组是否暂停。
        /// </summary>
        bool Pause
        {
            get;
            set;
        }

        /// <summary>
        /// 获取界面组中界面数量。
        /// </summary>
        int UICount
        {
            get;
        }

        /// <summary>
        /// 获取当前界面。
        /// </summary>
        IUIForm CurrentUI
        {
            get;
        }

        /// <summary>
        /// 获取界面组辅助器。
        /// </summary>
        IUIGroupHelper Helper
        {
            get;
        }

        /// <summary>
        /// 界面组中是否存在界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>界面组中是否存在界面。</returns>
        bool HasUI(int serialId);

        /// <summary>
        /// 界面组中是否存在界面。
        /// </summary>
        /// <param name="uiAssetName">界面资源名称。</param>
        /// <returns>界面组中是否存在界面。</returns>
        bool HasUI(string uiAssetName);

        /// <summary>
        /// 从界面组中获取界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>要获取的界面。</returns>
        IUIForm GetUI(int serialId);

        /// <summary>
        /// 从界面组中获取界面。
        /// </summary>
        /// <param name="uiAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        IUIForm GetUI(string uiAssetName);

        /// <summary>
        /// 从界面组中获取界面。
        /// </summary>
        /// <param name="uiAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        IUIForm[] GetUIArray(string uiAssetName);

        /// <summary>
        /// 从界面组中获取界面。
        /// </summary>
        /// <param name="uiAssetName">界面资源名称。</param>
        /// <param name="results">要获取的界面。</param>
        void GetUIList(string uiAssetName, List<IUIForm> results);

        /// <summary>
        /// 从界面组中获取所有界面。
        /// </summary>
        /// <returns>界面组中的所有界面。</returns>
        IUIForm[] GetAllUI();

        /// <summary>
        /// 从界面组中获取所有界面。
        /// </summary>
        /// <param name="results">界面组中的所有界面。</param>
        void GetAllUI(List<IUIForm> results);

        void Update(float elapseSeconds, float realElapseSeconds);
    }
}
