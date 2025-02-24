using System;
using System.Collections.Generic;
using GameFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIOFramework.Runtime
{
    public class PatchPage : MonoBehaviour
    {
        private class MessageBox
        {
            private GameObject _cloneObject;
            private TextMeshProUGUI _content;
            private Button _btnOK;
            private System.Action _clickOK;

            public bool ActiveSelf
            {
                get { return _cloneObject.activeSelf; }
            }

            public void Create(GameObject cloneObject)
            {
                _cloneObject = cloneObject;
                _content = cloneObject.transform.Find("tips").GetComponent<TextMeshProUGUI>();
                _btnOK = cloneObject.transform.Find("Button").GetComponent<Button>();
                _btnOK.onClick.AddListener(OnClickYes);
            }

            public void Show(string content, System.Action clickOK)
            {
                _content.text = content;
                _clickOK = clickOK;
                _cloneObject.SetActive(true);
                _cloneObject.transform.SetAsLastSibling();
            }

            public void Hide()
            {
                _content.text = string.Empty;
                _clickOK = null;
                _cloneObject.SetActive(false);
            }

            private void OnClickYes()
            {
                _clickOK?.Invoke();
                Hide();
            }
        }

        private GameObject _messageBoxObj;
        private Slider slider;
        private TextMeshProUGUI ver_txt;
        private TextMeshProUGUI info_txt;
        private readonly List<MessageBox> _msgBoxList = new List<MessageBox>();
        
        private void Awake()
        {
            BindComponents();
            AddListeners();
        }
        
        private void OnDestroy()
        {
            RemoveAllListeners();
        }
        
        private void BindComponents()
        {
            slider = transform.Find("progress").GetComponent<Slider>();
            ver_txt = transform.Find("version").GetComponent<TextMeshProUGUI>();
            info_txt = transform.Find("info").GetComponent<TextMeshProUGUI>();
            info_txt.text = "AIO Launch";
            _messageBoxObj = transform.Find("messagebox").gameObject;
            _messageBoxObj.SetActive(false);
        }

        private void AddListeners()
        {
            Entrance.Event.Subscribe(PatchStateChangeEventArgs.EventId, OnPatchStateChange);
            Entrance.Event.Subscribe(FindUpdateFilesEventArgs.EventId, OnFindUpdateFiles);
            Entrance.Event.Subscribe(PackageVersionEventArgs.EventId, OnPackageVersion);
            Entrance.Event.Subscribe(InitPackageFailedEventArgs.EventId, OnInitPackageFailed);
            Entrance.Event.Subscribe(DownloadFilesFailedEventArgs.EventId, OnDownloadFilesFailed);
            Entrance.Event.Subscribe(DownloadProgressEventArgs.EventID, OnDownloadProgress);
        }

        private void RemoveAllListeners()
        {
            Entrance.Event.Unsubscribe(PatchStateChangeEventArgs.EventId, OnPatchStateChange);
            Entrance.Event.Unsubscribe(FindUpdateFilesEventArgs.EventId, OnFindUpdateFiles);
            Entrance.Event.Unsubscribe(PackageVersionEventArgs.EventId, OnPackageVersion);
            Entrance.Event.Unsubscribe(InitPackageFailedEventArgs.EventId, OnInitPackageFailed);
            Entrance.Event.Unsubscribe(DownloadFilesFailedEventArgs.EventId, OnDownloadFilesFailed);
            Entrance.Event.Unsubscribe(DownloadProgressEventArgs.EventID, OnDownloadProgress);
        }
        
        /// <summary>
        /// 显示对话框
        /// </summary>
        private void ShowMessageBox(string content, System.Action ok)
        {
            // 尝试获取一个可用的对话框
            MessageBox msgBox = null;
            for (int i = 0; i < _msgBoxList.Count; i++)
            {
                var item = _msgBoxList[i];
                if (item.ActiveSelf == false)
                {
                    msgBox = item;
                    break;
                }
            }

            // 如果没有可用的对话框，则创建一个新的对话框
            if (msgBox == null)
            {
                msgBox = new MessageBox();
                var cloneObject = GameObject.Instantiate(_messageBoxObj, _messageBoxObj.transform.parent);
                msgBox.Create(cloneObject);
                _msgBoxList.Add(msgBox);
            }

            // 显示对话框
            msgBox.Show(content, ok);
        }
        
        void OnPatchStateChange(object sender, GameEventArgs gameEventArgs)
        {
            PatchStateChangeEventArgs args = gameEventArgs as PatchStateChangeEventArgs;
            info_txt.text = args.Tips;
        }
        
        void OnFindUpdateFiles(object sender, GameEventArgs gameEventArgs)
        {
            FindUpdateFilesEventArgs args = gameEventArgs as FindUpdateFilesEventArgs;
            Action ok = () =>
            {
                Entrance.Event.Fire(this, BeginDownloadUpdateFilesEventArgs.Create());
            };
            float sizeMB = args.TotalCount / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            ShowMessageBox(
                $"Update now? \n Total count = {args.TotalCount}, Total size = {totalSizeMB}MB",
                ok);
        }
        
        void OnPackageVersion(object sender, GameEventArgs gameEventArgs)
        {
            PackageVersionEventArgs args = gameEventArgs as PackageVersionEventArgs;
            ver_txt.text = args.PackageVersion;
        }
        
        void OnInitPackageFailed(object sender, GameEventArgs gameEventArgs)
        {
            Action callback = () =>
            {
                Log.Info("Application Quit");
                Application.Quit();
            };
            ShowMessageBox("Failed to initialize the package, please check the network and try again.", callback);
        }
        
        void OnDownloadFilesFailed(object sender, GameEventArgs gameEventArgs)
        {
            DownloadFilesFailedEventArgs args = gameEventArgs as DownloadFilesFailedEventArgs;
            Action callback = () =>
            {
                Application.Quit();
            };
            ShowMessageBox($"Download failed, please check the network and try again. \n {args.Error}", callback);
        }
        
        void OnDownloadProgress(object sender, GameEventArgs gameEventArgs)
        {
            DownloadProgressEventArgs args = gameEventArgs as DownloadProgressEventArgs;
            slider.value = (float)args.CurrentDownloadCount / args.TotalDownloadCount;
            string currentSizeMB = (args.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (args.TotalDownloadSizeBytes / 1048576f).ToString("f1");
            info_txt.text = $"{args.CurrentDownloadCount}/{args.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
        }
    }
}