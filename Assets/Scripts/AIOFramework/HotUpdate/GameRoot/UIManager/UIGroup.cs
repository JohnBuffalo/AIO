using System.Collections.Generic;
using AIOFramework.Runtime;

namespace AIOFramework.UI
{
    public class UIGroup : IUIGroup
    {
        private readonly string _name;
        private int _depth = -1;
        private int _uiDepthGap = 50;
        private bool _pause;
        private readonly IUIGroupHelper _uiGroupHelper;
        private readonly GameFrameworkLinkedList<UIViewBase> _uiLinkedList;
        private LinkedListNode<UIViewBase> _cachedNode;
        public UIGroup(UIGroupEnum name, int depth, IUIGroupHelper uiGroupHelper)
        {
            if (uiGroupHelper == null)
            {
                throw new GameFrameworkException("UI group helper is invalid.");
            }

            _name = name.ToString();
            _pause = false;
            _uiGroupHelper = uiGroupHelper;
            _uiLinkedList = new GameFrameworkLinkedList<UIViewBase>();
            _cachedNode = null;
            Depth = depth;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Depth
        {
            get { return _depth; }
            set
            {
                if (_depth == value)
                {
                    return;
                }

                _depth = value;
                _uiGroupHelper.SetDepth(_depth);
                Refresh();
            }
        }

        /// <summary>
        /// 获取或设置界面组是否暂停。
        /// </summary>
        public bool Pause
        {
            get { return _pause; }
            set
            {
                if (_pause == value)
                {
                    return;
                }

                _pause = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取界面组中界面数量。
        /// </summary>
        public int UICount
        {
            get { return _uiLinkedList.Count; }
        }

        /// <summary>
        /// 获取当前界面。
        /// </summary>
        public IUIForm CurrentUI
        {
            get { return _uiLinkedList.First != null ? _uiLinkedList.First.Value : null; }
        }

        /// <summary>
        /// 获取界面组辅助器。
        /// </summary>
        public IUIGroupHelper Helper
        {
            get { return _uiGroupHelper; }
        }

        /// <summary>
        /// 界面组轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<UIViewBase> current = _uiLinkedList.First;
            while (current != null)
            {
                if (current.Value.Paused)
                {
                    break;
                }

                _cachedNode = current.Next;
                current.Value.OnUpdate(elapseSeconds, realElapseSeconds);
                current = _cachedNode;
                _cachedNode = null;
            }
        }

        public bool HasUI(int serialId)
        {
            foreach (UIViewBase ui in _uiLinkedList)
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

            foreach (var ui in _uiLinkedList)
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
            foreach (var ui in _uiLinkedList)
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

            foreach (var ui in _uiLinkedList)
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
            foreach (var ui in _uiLinkedList)
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
            foreach (var ui in _uiLinkedList)
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
            foreach (var ui in _uiLinkedList)
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
            foreach (var ui in _uiLinkedList)
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
            _uiLinkedList.AddFirst(ui);
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

            if (_cachedNode != null && _cachedNode.Value == ui)
            {
                _cachedNode = _cachedNode.Next;
            }

            if (!_uiLinkedList.Remove(ui))
            {
                throw new GameFrameworkException(Utility.Text.Format(
                    "UI group '{0}' not exists specified UI form '[{1}]{2}'.", _name, ui.SerialId, ui.UIAssetName));
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

            _uiLinkedList.Remove(ui);
            _uiLinkedList.AddFirst(ui);
        }

        public void Refresh()
        {
            LinkedListNode<UIViewBase> current = _uiLinkedList.First;
            bool pause = _pause;
            bool cover = false;
            int depth = UICount;
            while (current != null && current.Value != null)
            {
                current.Value.transform.SetAsFirstSibling();
                LinkedListNode<UIViewBase> next = current.Next;
                var uiDepth = Depth + depth-- * _uiDepthGap;
                current.Value.OnDepthChanged(Depth, uiDepth);
                if (current.Value == null)
                {
                    return;
                }

                if (pause)
                {
                    if (!current.Value.Paused)
                    {
                        current.Value.Paused = true;
                        current.Value.OnPause();
                        if (current.Value == null)
                        {
                            return;
                        }
                    }
                    
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
            foreach (var ui in _uiLinkedList)
            {
                if (ui.UIAssetName == uiFormAssetName)
                {
                    results.Add(ui);
                }
            }
        }
        
        internal void InternalGetAllUIList(List<IUIForm> results)
        {
            foreach (var ui in _uiLinkedList)
            {
                results.Add(ui);
            }
        }
    }
}