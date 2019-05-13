//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：


namespace MC.CheatNs
{
    public class CommandInfo : System.Attribute
    {

        private string _name;
        private string _exp;

        public string Name { get { return _name; } }
        public string Explain { get { return _exp; } }

        private EnumRootLevel _rootLevel;
        public EnumRootLevel RootLevel { get { return _rootLevel; } }

        /// <summary>
        /// 判断运行权限是否足够
        /// </summary>
        public bool CanExecute { get { return (int)CheatSystemManager.GetInstance.CurrenLevel >= (int)_rootLevel; } }

        /// <summary>
        /// 获取运行权限所需等级描述
        /// </summary>
        public string RequireLevelName { get { return CheatSystemManager.GetRootLevelDetail(_rootLevel).Name; } }



        public CommandInfo(string str, EnumRootLevel level = EnumRootLevel.Player)
        {
            _exp = str;
            _rootLevel = level;
        }

        public CommandInfo(string name, string str, EnumRootLevel level = EnumRootLevel.Player)
        {
            _exp = str;
            _name = name;
            _rootLevel = level;
        }
    }
}