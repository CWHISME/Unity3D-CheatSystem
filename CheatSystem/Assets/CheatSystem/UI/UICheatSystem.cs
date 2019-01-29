//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MC.CheatNs
{
    public class UICheatSystem : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
    {

        public static UICheatSystem _instance;
        public static UICheatSystem GetInstance
        {
            get
            {
                if (!_instance)
                {
                    GameObject o = Resources.Load<GameObject>("[CheatSystem]");
                    o = GameObject.Instantiate<GameObject>(o);
                    o.name = "[CheatSystem]";
                    _instance = o.GetComponent<UICheatSystem>();
                    o.SetActive(false);
                }
                return _instance;
            }
        }

        [SerializeField]
        private Text _targetText;
        [SerializeField]
        private Text _tipsText;
        [SerializeField]
        private Text _pageTipsText;
        [SerializeField]
        private UI.UIScrollViewText _contentText;
        [SerializeField]
        private InputField _input;

        /// <summary>
        /// 所有输入命令的历史记录
        /// </summary>
        private List<string> _commandTempList = new List<string>();
        /// <summary>
        /// 历史记录下标
        /// </summary>
        private int _historyCmdIndex = -1;


        void Start()
        {
            RefreshTips();
            _contentText.Clear();
            //页码信息
            _contentText.OnPageChange = OnPageChange;
            OnPageChange(1, 1);

            CheatSystemManager.GetInstance.OnTargetChange += () => { RefreshTips(); };
            _input.onEndEdit.AddListener(OnEndEdit);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_commandTempList.Count < 1) return;

                _historyCmdIndex--;
                if (_historyCmdIndex < 0)
                    _historyCmdIndex = 0;
                _input.text = _commandTempList[_historyCmdIndex];
                _input.caretPosition = _input.text.Length;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_commandTempList.Count < 1) return;
                _historyCmdIndex++;
                if (_historyCmdIndex >= _commandTempList.Count)
                    _historyCmdIndex = _commandTempList.Count - 1;
                _input.text = _commandTempList[_historyCmdIndex];
                _input.caretPosition = _input.text.Length;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (string.IsNullOrEmpty(_input.text)) return;
                _input.text = CheatSystemManager.GetInstance.GetAutoCompltion(_input.text) + " ";
                _input.caretPosition = _input.text.Length;
            }

            if (Input.GetMouseButtonDown(0))
                CheatSystemManager.GetInstance.RayCheckTarget();
        }

        public void Active()
        {
            //CheatSystemManager.GetInstance.ResetTarget();
            RefreshTips();
            gameObject.SetActive(!gameObject.activeSelf);
            FocusClearInputField();
        }

        public void RefreshTips()
        {
            _targetText.text = ConstLanguage.Get(ConstLanguage.TargetInfo, GetCurrentTargetName());
            _tipsText.text = CheatSystemManager.GetInstance.GetCheatCommandTips();
        }

        public void ClearText()
        {
            _contentText.Clear();
        }

        public string GetText(bool justCurrentTxet = true)
        {
            return justCurrentTxet ? _contentText.text : _contentText.TotalText;
        }

        public void Passward(bool p)
        {
            _input.contentType = p ? InputField.ContentType.Password : InputField.ContentType.Standard;
        }

        /// <summary>
        /// 当前目标名字
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTargetName()
        {
            return CheatSystemManager.GetInstance.Target == null ? "Null" : CheatSystemManager.GetInstance.Target.Name;
        }

        private void OnPageChange(int index, int allPage)
        {
            _pageTipsText.text = string.Format("({0}/{1})", index, allPage);
        }

        private void OnEndEdit(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                _input.ActivateInputField();
                return;
            }

            if (_commandTempList.Count < 1 || command != _commandTempList[_commandTempList.Count - 1])
            {
                _commandTempList.Add(command);
                //仅记录三十条
                if (_commandTempList.Count > 30)
                    _commandTempList.RemoveAt(0);
            }

            _historyCmdIndex = _commandTempList.Count;

            string res = CheatSystemManager.GetInstance.RunCommand(command);
            //若是密码，这儿的回显也处理一下
            if (_input.contentType == InputField.ContentType.Password)
                command = new string('*', command.Length);
            if (!string.IsNullOrEmpty(res))
                _contentText.AddLine(string.Format("->{0}\n->{1}", command, res));

            FocusClearInputField();
        }

        /// <summary>
        /// 清除输入数据，并激活输入框
        /// </summary>
        private void FocusClearInputField()
        {
            _input.text = "";
            _input.ActivateInputField();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _input.ActivateInputField();
        }
    }
}