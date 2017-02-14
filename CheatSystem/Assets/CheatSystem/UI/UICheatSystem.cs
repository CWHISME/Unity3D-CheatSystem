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
        private UI.UIScrollViewText _contentText;
        [SerializeField]
        private InputField _input;

        private List<string> _commandTenpList = new List<string>();

        void Start()
        {
            RefreshTips();
            _contentText.text = "";

            CheatSystemManager.GetInstance.OnTargetChange += () => { RefreshTips(); };

            _input.onEndEdit.AddListener((command) =>
            {
                //if (!_return) return;
                if (string.IsNullOrEmpty(command)) return;

                if (_commandTenpList.Count < 1 || command != _commandTenpList[_commandTenpList.Count - 1])
                {
                    _commandTenpList.Add(command);
                    //仅记录三十条
                    if (_commandTenpList.Count > 30)
                        _commandTenpList.RemoveAt(0);
                }

                _historyCmdIndex = _commandTenpList.Count - 1;

                string res = CheatSystemManager.GetInstance.RunCommand(command);
                //若是密码，这儿的回显也处理一下
                if (_input.contentType == InputField.ContentType.Password)
                    command = new string('*', command.Length);
                if (!string.IsNullOrEmpty(res))
                    _contentText.text = _contentText.text + "\n->" + command + "\n->" + res;

                //while (_contentText.preferredHeight > _contentText.rectTransform.sizeDelta.y)
                //{
                //    _contentText.text = _contentText.text.Remove(0, _contentText.text.Length > 10 ? 10 : _contentText.text.Length);
                //}

                _input.ActivateInputField();

                //_return = false;
            });
        }

        //private bool _return = false;
        private int _historyCmdIndex = -1;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_commandTenpList.Count < 1) return;

                _historyCmdIndex--;
                if (_historyCmdIndex < 0)
                    _historyCmdIndex = 0;
                _input.text = _commandTenpList[_historyCmdIndex];
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_commandTenpList.Count < 1) return;
                _historyCmdIndex++;
                if (_historyCmdIndex >= _commandTenpList.Count)
                    _historyCmdIndex = _commandTenpList.Count - 1;
                _input.text = _commandTenpList[_historyCmdIndex];
            }

            if (Input.GetMouseButtonDown(0))
                CheatSystemManager.GetInstance.RayCheckTarget();
            //if (Input.GetKeyDown(KeyCode.Return))
            //    _return = true;
        }

        public void Active()
        {
            CheatSystemManager.GetInstance.ResetTarget();
            RefreshTips();
            _input.text = "";
            _input.ActivateInputField();
            gameObject.SetActive(!gameObject.activeSelf);

        }

        public void RefreshTips()
        {
            _targetText.text = "当前目标：" + (CheatSystemManager.GetInstance.Target == null ? "无" : CheatSystemManager.GetInstance.Target.Name);
            _tipsText.text = CheatSystemManager.GetInstance.GetCheatCommandTips();
        }

        public void ClearText()
        {
            _contentText.text = "";
        }

        public string GetText()
        {
            return _contentText.text;
        }

        public void Passward(bool p)
        {
            _input.contentType = p ? InputField.ContentType.Password : InputField.ContentType.Standard;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _input.ActivateInputField();
        }
    }
}