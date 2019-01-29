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

        [CommandInfo("导出控制台文本 （参数：导出路径）")]
        public string ExportTextWithPath(string path, bool justCurrentPage = true, bool openFilePath = false)
        {
            File.WriteAllText(path, UICheatSystem.GetInstance.GetText(justCurrentPage));
            if (openFilePath)
                System.Diagnostics.Process.Start(Path.GetDirectoryName(path));
            return "当前历史文本已导出至：" + path;
        }

        [CommandInfo("导出控制台文本（默认路径）")]
        public string ExportText(bool justCurrentPage = true, bool openFilePath = false)
        {
            return ExportTextWithPath(Application.dataPath + "/../ExportConsole.txt", justCurrentPage, openFilePath);
        }

    }
}