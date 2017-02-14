//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：


using System.Reflection;

namespace MC.CheatNs
{
    public class CheatItem
    {

        private string _commandList;

        public CheatItem()
        {


            MethodInfo[] mths = this.GetType().GetMethods();
            for (int i = 0; i < mths.Length; i++)
            {
                CommandInfo exp = (CommandInfo)System.Attribute.GetCustomAttribute(mths[i], typeof(CommandInfo));
                if (exp != null)
                    _commandList += "<color=#00FF00>" + mths[i].Name + "</color> : " + exp.Explain + " \n";
            }
        }

        public string GetCommandList()
        {
            return _commandList;
        }
    }
}