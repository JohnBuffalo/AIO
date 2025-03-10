using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AIOFramework.ObjectPool;
using Cysharp.Threading.Tasks;

namespace AIOFramework.Runtime
{
    public interface IUIManager
    {
        /// <summary>
        /// 获取界面组数量。
        /// </summary>
        int UIGroupCount { get; }

        /// <summary>
        /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        float InstanceAutoReleaseInterval { get; set; }

        /// <summary>
        /// 获取或设置界面实例对象池的容量。
        /// </summary>
        int InstanceCapacity { get; set; }

        /// <summary>
        /// 获取或设置界面实例对象池对象过期秒数。
        /// </summary>
        float InstanceExpireTime { get; set; }

        /// <summary>
        /// 获取或设置界面实例对象池的优先级。
        /// </summary>
        int InstancePriority { get; set; }

        /// <summary>
        /// 打开界面成功事件。
        /// </summary>
        event EventHandler<OpenUISuccessEventArgs> OpenUIFormSuccess;

        /// <summary>
        /// 打开界面失败事件。
        /// </summary>
        event EventHandler<OpenUIFailureEventArgs> OpenUIFormFailure;

        /// <summary>
        /// 关闭界面完成事件。
        /// </summary>
        event EventHandler<CloseUICompleteEventArgs> CloseUIFormComplete;

        void SetObjectPoolManager(IObjectPoolManager objectPoolManager);

        void SetAssetManager(IAssetManager assetManager);

        bool HasUIGroup(UIGroupEnum uiGroupName);

        IUIGroup GetUIGroup(UIGroupEnum uiGroupName);

        IUIGroup[] GetAllUIGroups();

        void GetAllUIGroups(List<IUIGroup> results);

        bool AddUIGroup(UIGroupEnum uiGroupName, IUIGroupHelper uiGroupHelper);

        bool AddUIGroup(UIGroupEnum uiGroupName, int uiGroupDepth, IUIGroupHelper uiGroupHelper);

        bool HasUI(int serialId);

        bool HasUI(string uiFormAssetName);

        IUIForm GetUI(int serialId);

        IUIForm GetUI(string uiFormAssetName);

        T GetUI<T>(string uiFormAssetName) where T : IUIForm;
        T GetUI<T>(int serialId) where T : IUIForm;

        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        IUIForm[] GetAllUI(string uiFormAssetName);

        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="results">要获取的界面。</param>
        void GetAllUI(string uiFormAssetName, List<IUIForm> results);

        /// <summary>
        /// 获取所有已加载的界面。
        /// </summary>
        /// <returns>所有已加载的界面。</returns>
        IUIForm[] GetAllLoadedUI();

        /// <summary>
        /// 获取所有已加载的界面。
        /// </summary>
        /// <param name="results">所有已加载的界面。</param>
        void GetAllLoadedUI(List<IUIForm> results);

        /// <summary>
        /// 获取所有正在加载界面的序列编号。
        /// </summary>
        /// <returns>所有正在加载界面的序列编号。</returns>
        int[] GetAllLoadingUISerialIds();

        /// <summary>
        /// 获取所有正在加载界面的序列编号。
        /// </summary>
        /// <param name="results">所有正在加载界面的序列编号。</param>
        void GetAllLoadingUISerialIds(List<int> results);

        /// <summary>
        /// 是否正在加载界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>是否正在加载界面。</returns>
        bool IsLoadingUI(int serialId);

        /// <summary>
        /// 是否是合法的界面。
        /// </summary>
        /// <param name="uiForm">界面。</param>
        /// <returns>界面是否合法。</returns>
        bool IsValidUI(IUIForm uiForm);

        /// <summary>
        /// 异步打开界面。
        /// </summary>
        /// <param name="userData"></param>
        /// <typeparam name="TUICtorInfo">UICtorInfo</typeparam>
        /// <typeparam name="TViewModel">UIViewModelBase</typeparam>
        /// <returns></returns>
        UniTask<int> OpenUI<TUICtorInfo, TViewModel>(object userData = null)
            where TUICtorInfo : UICtorInfo, new() where TViewModel : UIViewModelBase, new();
        
        /// <summary>
        /// 异步打开界面。
        /// </summary>
        /// <param name="ctorInfo">界面资源路径。</param>
        /// <param name="viewModel">视图控制层。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面的序列编号。</returns>
        UniTask<int> OpenUI(UICtorInfo ctorInfo, UIViewModelBase viewModel, object userData);

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="serialId">要关闭界面的序列编号。</param>
        void CloseUI(int serialId);

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="serialId">要关闭界面的序列编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        void CloseUI(int serialId, object userData);

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="uiForm">要关闭的界面。</param>
        void CloseUI(IUIForm uiForm);

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="uiForm">要关闭的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        void CloseUI(IUIForm uiForm, object userData);

        /// <summary>
        /// 关闭所有已加载的界面。
        /// </summary>
        void CloseAllLoadedUI();

        /// <summary>
        /// 关闭所有已加载的界面。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        void CloseAllLoadedUI(object userData);

        /// <summary>
        /// 关闭所有正在加载的界面。
        /// </summary>
        void CloseAllLoadingUI();

        /// <summary>
        /// 激活界面。
        /// </summary>
        /// <param name="uiForm">要激活的界面。</param>
        void RefocusUI(IUIForm uiForm);

        /// <summary>
        /// 激活界面。
        /// </summary>
        /// <param name="uiForm">要激活的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        void RefocusUI(IUIForm uiForm, object userData);

        /// <summary>
        /// 设置界面实例是否被加锁。
        /// </summary>
        /// <param name="uiInstance">要设置是否被加锁的界面实例。</param>
        /// <param name="locked">界面实例是否被加锁。</param>
        void SetUIInstanceLocked(object uiInstance, bool locked);

        /// <summary>
        /// 设置界面实例的优先级。
        /// </summary>
        /// <param name="uiInstance">要设置优先级的界面实例。</param>
        /// <param name="priority">界面实例优先级。</param>
        void SetUIInstancePriority(object uiInstance, int priority);
    }
}