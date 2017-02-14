//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：


using UnityEngine;

namespace MC.CheatNs
{
    public interface ICheatDetectable
    {
        string Name { get; }
        GameObject gameObject { get; }
    }
}