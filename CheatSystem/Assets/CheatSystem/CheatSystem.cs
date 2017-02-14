//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：
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