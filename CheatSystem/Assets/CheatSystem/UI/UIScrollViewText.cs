//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：
using UnityEngine;
using UnityEngine.UI;

namespace MC.UI
{
    /// <summary>
    /// 仅用于Text的ScrollView 扩展
    /// </summary>
    public class UIScrollViewText : MonoBehaviour
    {

        [SerializeField]
        private Text _content;
        [SerializeField]
        private Scrollbar _slider;
        [SerializeField]
        private RectTransform _scrollContentRect;
        [SerializeField]
        //最大支持多少高度，超出高度将会自动删除之前的内容
        private float _supportHeight = 500;

        [SerializeField]
        private bool _useAnimation = true;
        [SerializeField]
        private bool _jumpToLast = true;//显示完毕，是够跳至在最后

        public string text
        {
            get
            {
                return _content.text;
            }

            set
            {

                if (!_useAnimation || string.IsNullOrEmpty(value))
                {
                    _content.text = value;

                    CheckCanDeleteText();
                    ResizeScrollHeight();
                    if (_jumpToLast)
                        _slider.value = 0;
                    else _slider.value = 1;
                }
                else
                {
                    _content.text = value;
                    CheckCanDeleteText();
                    ResizeScrollHeight();
                    if (_jumpToLast)
                        _slider.value = 0;
                    else _slider.value = 1;
                }
            }
        }

        public void AddText(string txt)
        {
            text += txt;
        }

        //检查高度是否超出限制，否则删除最早的文字
        private void CheckCanDeleteText()
        {
            int limitIndex = 10;
            while (_content.preferredHeight > _supportHeight)
            {
                int index = _content.text.IndexOf('\n');
                if (index != -1)
                    _content.text = _content.text.Remove(0, index + 1);
                limitIndex--;
                if (limitIndex < 0) break;
            }
        }

        private void ResizeScrollHeight()
        {
            _scrollContentRect.sizeDelta = new Vector2(_scrollContentRect.sizeDelta.x, _content.preferredHeight);
            _content.rectTransform.sizeDelta = new Vector2(_content.rectTransform.sizeDelta.x, _content.preferredHeight);
        }

        public void AddText(string txt, float closeTime)
        {
            AddText(txt);
            Show(closeTime);
        }

        public void ShowText(string txt, float closeTime)
        {
            text = txt;
            Show(closeTime);
        }

        public void Show(float closeTime)
        {
            gameObject.SetActive(true);
        }

        public void Close(float closeTime)
        {
            gameObject.SetActive(false);
        }


    }
}