//创建作者：Wangjiaying
//创建日期：2019.1.28
//主要功能：


using UnityEngine;

namespace MC.CheatNs
{
    public class ConstLanguage
    {
        public const string TargetInfo = "目标：{0}";
        public const string CommandName = "<color=#00FF00>{0}</color>";
        public const string CommandTips = "当前权限：{0}";
        public const string CommandResult = "<color=#FFFF00FF>执行结果：Success！</color>\n{0}";
        public const string CommandHelp = "帮助：你可以从以下命令中选择执行，命令格式按照 指令->方法与参数顺序进行.\n{0}";
        public const string CommandParams = "<color=#FF8E00FF>{0}</color>";

        public const string ErrorCommand = "<color=#FF00FF>错误：{0}</color>";
        public const string ErrorNullCommand = "<color=#FF00FF>错误：命令为空！</color>";
        public const string ErrorNotFindCommand = "<color=#FF00FF>错误：未找到命令 [{0}]</color>\n有关命令帮助，请输入“Help”查看.";
        public const string ErrorCommandParams = "<color=#FF00FF>错误：{0}</color>\n可用参数有：\n{1}";
        public const string ErrorCommandPolicy = "<color=#FF00FF>权限不足：该命令要求至少为 <color=yellow>{0} 权限</color>";

        public static string Get(string item, params object[] pms)
        {
            return string.Format(item, pms);
        }
    }
}