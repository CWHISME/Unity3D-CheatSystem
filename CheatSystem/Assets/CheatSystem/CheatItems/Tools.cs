//创建作者：Wangjiaying
//创建日期：2016.12.14
//主要功能：

using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MC.CheatNs
{
    [CommandInfo("工具命令，系统、工具相关接口，如硬件信息等")]
    public class Tools : CheatItem
    {
        [CommandInfo("显示系统信息")]
        public string SystemInfo()
        {
            StringBuilder builder = new StringBuilder("\n系统信息：");

            Type type = typeof(UnityEngine.SystemInfo);
            PropertyInfo[] infos = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
            for (int i = 0; i < infos.Length; i++)
            {
                builder.AppendLine(infos[i].Name + " : " + infos[i].GetValue(null, null));
            }

            builder.AppendLine("报告生成时间：" + System.DateTime.Now.ToString());
            return builder.ToString();
        }

        [CommandInfo("生成一张当前屏幕所有信息截图（如控制台、UI等都会算进去）")]
        public string CaptureScreenshot()
        {
            string path = GetCaptureScreenshotPath();
            UnityEngine.Application.CaptureScreenshot(path);
            return "截图已保存至 <color=yellow>" + path + "</color>";
        }

        private string GetCaptureScreenshotPath()
        {
            string dir = Application.dataPath + "/../CaptureScreenshot/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return Path.GetFullPath(dir + DateTime.Now.ToString("yyyy年MM月dd日 H时m分s秒") + ".PNG");
        }

    }
}