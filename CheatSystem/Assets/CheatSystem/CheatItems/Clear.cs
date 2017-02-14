//创建作者：Wangjiaying
//创建日期：2016.12.14
//主要功能：


namespace MC.CheatNs
{
    [CommandInfo("清除控制台当前所有文本信息")]
    public class Clear : CheatItem
    {

        public void SingleMethod()
        {
            UICheatSystem.GetInstance.ClearText();
        }

    }
}