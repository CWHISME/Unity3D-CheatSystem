//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：


namespace MC.CheatNs
{
    public class CommandInfo : System.Attribute
    {

        private string _exp;
        public string Explain { get { return _exp; } }

        private EnumRootLevel _rootLevel;
        public bool CanExecute
        {
            get
            {
                return (int)CheatSystemManager.GetInstance.CurrenLevel >= (int)_rootLevel;
            }
        }

        public string LevelName { get { return CheatSystemManager.GetInstance.GetRootLevelDetail(_rootLevel).Name; } }

        public CommandInfo(string str, EnumRootLevel level = EnumRootLevel.Player)
        {
            _exp = str;
            _rootLevel = level;
        }
    }
}