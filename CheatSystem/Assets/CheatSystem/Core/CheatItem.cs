//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：


using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MC.CheatNs
{
    public class CheatItem
    {

        private List<string> _commandList;
        private string _commandDescList;

        public CheatItem()
        {
            _commandList = new List<string>();
            StringBuilder builder = new StringBuilder();
            MethodInfo[] mths = this.GetType().GetMethods();
            for (int i = 0; i < mths.Length; i++)
            {
                CommandInfo exp = (CommandInfo)System.Attribute.GetCustomAttribute(mths[i], typeof(CommandInfo));
                if (exp != null)
                {
                    _commandList.Add(mths[i].Name);
                    builder.Append("[" + exp.RequireLevelName + "]");
                    builder.Append(string.Format(ConstLanguage.CommandName, mths[i].Name));
                    builder.Append("(");
                    ParameterInfo[] paramsInfo = mths[i].GetParameters();
                    foreach (var item in paramsInfo)
                    {
                        builder.Append(item.ParameterType + " " + item.Name);
                        if (!string.IsNullOrEmpty(item.DefaultValue.ToString()))
                        {
                            builder.Append("=");
                            builder.Append(item.DefaultValue.ToString());
                        }
                        builder.Append(",");
                    }
                    if (paramsInfo.Length > 0)
                        builder.Remove(builder.Length - 1, 1);
                    builder.Append(")");
                    builder.Append(":");
                    builder.AppendLine(exp.Explain);
                }
            }
            _commandDescList = builder.ToString();
        }

        /// <summary>
        /// 该模块所有可执行方法及其描述(帮助)
        /// </summary>
        /// <returns></returns>
        public string GetCommandDescList()
        {
            return _commandDescList;
        }

        /// <summary>
        /// 该模块所有可执行方法名
        /// </summary>
        /// <returns></returns>
        public List<string> GetCommandNameList()
        {
            return _commandList;
        }
    }
}