//创建作者：Wangjiaying
//创建日期：2016.12.14
//主要功能：

namespace MC.CheatNs
{
    [CommandInfo("权限命令，主要用于控制台权限变更等")]
    public class Root : CheatItem
    {
        [CommandInfo("获取权限")]
        public string Get(object passward)
        {
            int max = (int)EnumRootLevel.Max;
            for (int i = max; i > 0; i--)
            {
                Passward p = CheatSystemManager.GetInstance.GetRootLevelDetail((EnumRootLevel)i);
                if (p != null)
                {
                    if (p.CheckPassward(passward.ToString()))
                    {
                        CheatSystemManager.GetInstance.CurrenLevel = (EnumRootLevel)i;
                        UICheatSystem.GetInstance.RefreshTips();
                        return "权限变更为 <color=#00FF00>" + p.Name + "</color>";
                    }
                }
            }

            return "密码错误，权限变更失败！";
        }

    }
}