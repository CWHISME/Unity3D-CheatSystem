//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：


using UnityEngine;

namespace MC.CheatNs
{
    public enum EnumRootLevel
    {
        [Passward(null, "玩家")]
        Player,
        [Passward("4D3CA37D310F95E4A06AB2A503A55B87", "一级")]
        One,
        [Passward("E89BBA290D125803653FC3CC5FB726AD", "二级")]
        Two,
        [Passward("B47C67EB1EAE84056537D2661EDA4149", "三级")]
        Three,
        [Passward("1BA7A4573ECD2668E1E63B9662560B78", "管理员")]
        Max,
    }
}