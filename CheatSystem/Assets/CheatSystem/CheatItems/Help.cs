//创建作者：Wangjiaying
//创建日期：2016.12.14
//主要功能：


namespace MC.CheatNs
{
    [CommandInfo("帮助命令，主要用于命令帮助等信息")]
    public class Help : CheatItem
    {
        [CommandInfo("显示指定命令的详细信息")]
        public string Detail(object cmdName)
        {
            return "\n" + CheatSystemManager.GetInstance.GetTargetCommandHelp(cmdName.ToString());
        }

        [CommandInfo("显示所有可用命令列表及解释")]
        public string ShowAllCommand()
        {
            return "\n" + CheatSystemManager.GetInstance.GetCommandList();
        }

    }
}