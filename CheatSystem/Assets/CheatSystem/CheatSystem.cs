//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：开启控制台管理主入口，可在任何地放进行调用，以开启。
using System;
using UnityEngine;

namespace MC.CheatNs
{
    public class CheatSystem : MonoBehaviour
    {

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                UICheatSystem.GetInstance.Active();
            }
        }

    }
}