﻿//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：
using System;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MC.CheatNs
{
    /// <summary>
    /// 仅用于Text的ScrollView 扩展
    /// </summary>
    public class UIScrollViewText : MonoBehaviour, UnityEngine.EventSystems.IEndDragHandler
    {

        [SerializeField]
        private Text _content;
        //[SerializeField]
        //private Scrollbar _slider;
        [SerializeField]
        private RectTransform _scrollContentRect;
        [SerializeField]
        //最大支持多少行，超出行数会自动分页，上下拖动至终点，自动切换页面
        private int _supportTextLineSize = 50;
        [SerializeField]
        //是否允许翻页
        private bool _allowPage = true;

        private RectTransform _rectTranform;
        private ScrollRect _scrollRect;
        /// <summary>
        /// 所有显示过的文字缓存
        /// </summary>
        private StringBuilder _strBuilder;
        /// <summary>
        /// 翻页下标，用于处理超出字数显示上限的翻页情况
        /// </summary>
        private int _pageIndex = 0;
        /// <summary>
        /// 页码数量标示
        /// </summary>
        private int _pageCount = 0;

        /// <summary>
        /// 翻页事件(当前/总共)
        /// </summary>
        public Action<int, int> OnPageChange;

        private void Awake()
        {
            _rectTranform = transform as RectTransform;
            _scrollRect = GetComponent<ScrollRect>();
        }

        public string text
        {
            get
            {
                return _content.text;
            }

            private set
            {
                //_strBuilder = new StringBuilder(value);
                _content.text = value;
                //CheckCanDeleteText();
                ResizeScrollHeight();
                JumpContentToEnd();
            }
        }

        /// <summary>
        /// 当前添加进入的所有文本内容
        /// </summary>
        public string TotalText { get { return _strBuilder.ToString(); } }

        /// <summary>
        /// 增加文本
        /// </summary>
        /// <param name="str"></param>
        public void Add(string str)
        {
            _strBuilder.Append(str);
            RefreshDisplayText();
        }

        /// <summary>
        /// 提一行，增加文本
        /// </summary>
        /// <param name="str"></param>
        public void AddLine(string str)
        {
            _strBuilder.AppendLine(str);
            RefreshDisplayText();
        }

        /// <summary>
        /// 翻页 +
        /// </summary>
        public void PagePlus()
        {
            int page = Mathf.Clamp(_pageIndex + 1, 0, _pageCount);
            if (page == _pageIndex) return;
            RefreshDisplayText(page);
        }

        /// <summary>
        /// 翻页 -
        /// </summary>
        public void PageReduce()
        {
            int page = Mathf.Clamp(_pageIndex - 1, 0, _pageIndex);
            if (page == _pageIndex) return;
            RefreshDisplayText(page);
            JumpContentToStart();
        }

        public void Clear()
        {
            _strBuilder = new StringBuilder();
            text = "";
            RefreshDisplayText();
        }

        /// <summary>
        /// 处理显示文本
        /// </summary>
        private void RefreshDisplayText(int pageIndex = 0)
        {
            //分页处理
            string[] contentArray = _strBuilder.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None);
            int pageCount = contentArray.Length / _supportTextLineSize;
            if (pageIndex <= pageCount)
            {
                bool contentMoreThanSize = contentArray.Length > _supportTextLineSize;
                int totalLength = contentArray.Length;
                int startIndex, length;
                startIndex = contentMoreThanSize ? totalLength - _supportTextLineSize * (pageIndex + 1) : 0;
                length = contentMoreThanSize ? _supportTextLineSize : contentArray.Length;
                if (startIndex < 0)
                {
                    //length = _supportTextSize + startIndex;
                    startIndex = 0;
                }
                if ((_pageIndex != pageIndex || _pageCount != pageCount) && OnPageChange != null)
                    OnPageChange.Invoke(pageIndex + 1, pageCount + 1);
                _pageIndex = pageIndex;
                _pageCount = pageCount;
                //处理字体颜色问题
                //当前已经修改为行数判断，这个暂且就没用了
                //string txt = _strBuilder.ToString(startIndex, length);
                //txt = TrimColor(txt, _pageIndex > 0);

                string[] displayArray = new string[length];
                Array.Copy(contentArray, startIndex, displayArray, 0, length);
                text = string.Join("\r\n", displayArray);
            }
            else
            {
                AddLine("System Error！");
            }

            _scrollRect.StopMovement();
        }

        /// <summary>
        /// 处理字体颜色问题，避免截取到Color，将其分割
        /// </summary>
        private string TrimColor(string txt, bool trimEnd = false)
        {
            int colorStartIndex = txt.IndexOf("<color=");
            int colorEndIndex = txt.IndexOf("</color>");
            if (colorEndIndex < colorStartIndex)
                txt = txt.Substring(colorStartIndex, txt.Length - colorStartIndex);

            if (trimEnd)
            {
                colorStartIndex = txt.LastIndexOf("<color=");
                colorEndIndex = txt.LastIndexOf("</color>");
                if (colorEndIndex < colorStartIndex)
                    txt = txt.Substring(0, colorStartIndex);
            }
            return txt;
        }

        //检查高度是否超出限制，否则删除最早的文字
        //private void CheckCanDeleteText()
        //{
        //    int limitIndex = 10;
        //    while (_content.preferredHeight > _supportHeight)
        //    {
        //        int index = _content.text.IndexOf('\n');
        //        if (index != -1)
        //            _content.text = _content.text.Remove(0, index + 1);
        //        limitIndex--;
        //        if (limitIndex < 0) break;
        //    }
        //}

        /// <summary>
        /// 重设内容大小
        /// </summary>
        private void ResizeScrollHeight()
        {
            _scrollContentRect.sizeDelta = new Vector2(_scrollContentRect.sizeDelta.x, Mathf.CeilToInt(_content.preferredHeight + 2));
            _content.rectTransform.sizeDelta = new Vector2(_content.rectTransform.sizeDelta.x, _content.preferredHeight);
        }

        /// <summary>
        /// 内容跳转至最后
        /// </summary>
        private void JumpContentToEnd()
        {
            _scrollContentRect.anchoredPosition = new Vector2(0, Mathf.Clamp(_scrollContentRect.sizeDelta.y - _rectTranform.sizeDelta.y, 0, int.MaxValue));
        }

        /// <summary>
        /// 内容跳转至最开始
        /// </summary>
        private void JumpContentToStart()
        {
            _scrollContentRect.anchoredPosition = Vector2.zero;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (_allowPage)
            {
                if (_scrollRect.verticalNormalizedPosition > 1.03f)
                    PagePlus();
                else if (_scrollRect.verticalNormalizedPosition < -0.03f)
                    PageReduce();
            }
        }
    }
}