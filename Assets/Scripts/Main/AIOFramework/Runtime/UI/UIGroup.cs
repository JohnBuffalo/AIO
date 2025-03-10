using System.Collections.Generic;
using Loxodon.Framework.ViewModels;

namespace AIOFramework.Runtime
{
    public class UIGroup : IUIGroup
    {
        private readonly string m_Name;
        private int m_Depth = -1;
        private int m_UIDepthGap = 50;
        private bool m_Pause;
        private readonly IUIGroupHelper m_UIGroupHelper;
        private readonly GameFrameworkLinkedList<UIViewBase> m_UILinkedList;
        private LinkedListNode<UIViewBase> m_CachedNode;
        public UIGroup(UIGroupEnum name, int depth, IUIGroupHelper uiGroupHelper)
        {
            if (uiGroupHelper == null)
            {
                throw new GameFrameworkException("UI group helper is invalid.");
            }

            m_Name = name.ToString();
            m_Pause = false;
            m_UIGroupHelper = uiGroupHelper;
            m_UILinkedList = new GameFrameworkLinkedList<UIViewBase>();
            m_CachedNode = null;
            Depth = depth;
        }

        public string Name
        {
            get { return m_Name; }
        }

        public int Depth
        {
            get { return m_Depth; }
            set
            {
                if (m_Depth == value)
                {
                    return;
                }

                m_Depth = value;
                m_UIGroupHelper.SetDepth(m_Depth);
                Refresh();
            }
        }

        /// <summary>
        /// 获取或设置界面组是否暂停。
        /// </summary>
        public bool Pause
        {
            get { return m_Pause; }
            set
            {
                if (m_Pause == value)
                {
                    return;
                }

                m_Pause = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取界面组中界面数量。
        /// </summary>
        public int UICount
        {
            get { return m_UILinkedList.Count; }
        }

        /// <summary>
        /// 获取当前界面。
        /// </summary>
        public IUIForm CurrentUI
        {
            get { return m_UILinkedList.First != null ? m_UILinkedList.First.Value : null; }
        }

        /// <summary>
        /// 获取界面组辅助器。
        /// </summary>
        public IUIGroupHelper Helper
        {
            get { return m_UIGroupHelper; }
        }

        /// <summary>
        /// 界面组轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<UIViewBase> current = m_UILinkedList.First;
            while (current != null)
            {
                if (current.Value.Paused)
                {
                    break;
                }

                m_CachedNode = current.Next;
                current.Value.OnUpdate(elapseSeconds, realElapseSeconds);
                current = m_CachedNode;
                m_CachedNode = null;
            }
        }

        public bool HasUI(int serialId)
        {
            foreach (UIViewBase ui in m_UILinkedList)
            {
                if (ui.SerialId == serialId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasUI(string uiAssetName)
        {
            if (string.IsNullOrEmpty(uiAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            foreach (var ui in m_UILinkedList)
            {
                if (ui.UIAssetName == uiAssetName)
                {
                    return true;
                }
            }

            return false;
        }

        public IUIForm GetUI(int serialId)
        {
            foreach (var ui in m_UILinkedList)
            {
                if (ui.SerialId == serialId)
                {
                    return ui;
                }
            }

            return null;
        }

        public IUIForm GetUI(string uiAssetName)
        {
            if (string.IsNullOrEmpty(uiAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            foreach (var ui in m_UILinkedList)
            {
                if (ui.UIAssetName == uiAssetName)
                {
                    return ui;
                }
            }

            return null;
        }

        public IUIForm[] GetUIArray(string uiAssetName)
        {
            if (string.IsNullOrEmpty(uiAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            List<IUIForm> results = new List<IUIForm>();
            foreach (var ui in m_UILinkedList)
            {
                if (ui.UIAssetName == uiAssetName)
                {
                    results.Add(ui);
                }
            }

            return results.ToArray();
        }

        public void GetUIList(string uiAssetName, List<IUIForm> results)
        {
            if (string.IsNullOrEmpty(uiAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (var ui in m_UILinkedList)
            {
                if (ui.UIAssetName == uiAssetName)
                {
                    results.Add(ui);
                }
            }
        }

        public IUIForm[] GetAllUI()
        {
            List<IUIForm> results = new List<IUIForm>();
            foreach (var ui in m_UILinkedList)
            {
                results.Add(ui);
            }

            return results.ToArray();
        }

        public void GetAllUI(List<IUIForm> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (var ui in m_UILinkedList)
            {
                results.Add(ui);
            }
        }

        /// <summary>
        /// 往界面组增加界面。
        /// </summary>
        /// <param name="ui">要增加的界面。</param>
        public void AddUI(UIViewBase ui)
        {
            m_UILinkedList.AddFirst(ui);
        }

        /// <summary>
        /// 从界面组移除界面。
        /// </summary>
        /// <param name="ui">要移除的界面。</param>
        public void RemoveUI(UIViewBase ui)
        {
            // if (!ui.Covered)
            // {
            //     ui.Covered = true;
            //     ui.OnCover();
            // }

            if (!ui.Paused)
            {
                ui.Paused = true;
                ui.OnPause();
            }

            if (m_CachedNode != null && m_CachedNode.Value == ui)
            {
                m_CachedNode = m_CachedNode.Next;
            }

            if (!m_UILinkedList.Remove(ui))
            {
                throw new GameFrameworkException(Utility.Text.Format(
                    "UI group '{0}' not exists specified UI form '[{1}]{2}'.", m_Name, ui.SerialId, ui.UIAssetName));
            }
        }

        /// <summary>
        /// 激活界面。
        /// </summary>
        /// <param name="ui">要激活的界面。</param>
        public void RefocusUI(UIViewBase ui)
        {
            if (ui == null)
            {
                throw new GameFrameworkException("Can not find UI form info.");
            }

            m_UILinkedList.Remove(ui);
            m_UILinkedList.AddFirst(ui);
        }

        public void Refresh()
        {
            LinkedListNode<UIViewBase> current = m_UILinkedList.First;
            bool pause = m_Pause;
            bool cover = false;
            int depth = UICount;
            while (current != null && current.Value != null)
            {
                current.Value.transform.SetAsFirstSibling();
                LinkedListNode<UIViewBase> next = current.Next;
                var uiDepth = Depth + depth-- * m_UIDepthGap;
                current.Value.OnDepthChanged(Depth, uiDepth);
                if (current.Value == null)
                {
                    return;
                }

                if (pause)
                {
                    if (!current.Value.Covered)
                    {
                        current.Value.Covered = true;
                        current.Value.OnCover();
                        if (current.Value == null)
                        {
                            return;
                        }
                    }

                    if (!current.Value.Paused)
                    {
                        current.Value.Paused = true;
                        current.Value.OnPause();
                        if (current.Value == null)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    if (current.Value.Paused)
                    {
                        current.Value.Paused = false;
                        current.Value.OnResume();
                        if (current.Value == null)
                        {
                            return;
                        }
                    }

                    if (current.Value.PauseCoveredUI)
                    {
                        pause = true;
                    }

                    if (cover)
                    {
                        if (!current.Value.Covered)
                        {
                            current.Value.Covered = true;
                            current.Value.OnCover();
                            if (current.Value == null)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (current.Value.Covered)
                        {
                            current.Value.Covered = false;
                            current.Value.OnReveal();
                            if (current.Value == null)
                            {
                                return;
                            }
                        }

                        cover = true;
                    }
                }

                current = next;
            }
        }
        
        internal void InternalGetUIList(string uiFormAssetName, List<IUIForm> results)
        {
            foreach (var ui in m_UILinkedList)
            {
                if (ui.UIAssetName == uiFormAssetName)
                {
                    results.Add(ui);
                }
            }
        }
        
        internal void InternalGetAllUIList(List<IUIForm> results)
        {
            foreach (var ui in m_UILinkedList)
            {
                results.Add(ui);
            }
        }
    }
}