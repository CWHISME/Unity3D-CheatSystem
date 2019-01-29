//创建作者：Wangjiaying
//创建日期：2016.12.14
//主要功能：


namespace MC.CheatNs
{
    [CommandInfo("帮助命令，主要用于命令帮助等信息 + 命令名 可显示指定命令详情")]
    public class Help : CheatItem
    {
        //[CommandInfo("帮助", "显示所有可用命令列表及解释")]
        public string DefaultSingleMethod()
        {
            return CheatSystemManager.GetInstance.GetCommandList();
        }

        //[CommandInfo("帮助", "显示指定命令的详细信息")]
        public string DefaultOneParamsMethod(string cmdName)
        {
            return CheatSystemManager.GetInstance.GetTargetCommandHelp(cmdName.ToString());
        }
    }
}