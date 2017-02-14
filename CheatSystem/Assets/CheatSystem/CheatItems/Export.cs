//创建作者：Wangjiaying
//创建日期：2016.12.14
//主要功能：

using System.IO;
using UnityEngine;

namespace MC.CheatNs
{
    [CommandInfo("导出")]
    public class Export : CheatItem
    {

        [CommandInfo("导出当前控制台文本 （参数：导出路径）")]
        public string ExportTextWithPath(object path)
        {
            File.WriteAllText(path.ToString(), UICheatSystem.GetInstance.GetText());

            return "当前控制台文本已导出至：" + path;
        }

        [CommandInfo("导出当前控制台文本（默认路径）")]
        public string ExportText()
        {
            return ExportTextWithPath(Application.dataPath + "/../ExportConsole.txt");
        }

    }
}