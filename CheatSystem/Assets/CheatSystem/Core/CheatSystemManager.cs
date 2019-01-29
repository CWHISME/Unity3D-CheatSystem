//创建作者：Wangjiaying
//创建日期：2016.12.13
//主要功能：


using System;
using System.Collections.Generic;
using System.Reflection;

namespace MC.CheatNs
{
    public class CheatSystemManager
    {

        //======可自定义设置=============
        //修改返回值，当第一次打开控制台，没有任何目标时，默认会设置的目标
        //一般就是玩家了
        private ICheatDetectable DefaultTarget { get { return null; } }


        private static CheatSystemManager _instance;
        public static CheatSystemManager GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CheatSystemManager();
                }
                return _instance;
            }
        }

        public event Action OnTargetChange;

        private ICheatDetectable _target;
        public ICheatDetectable Target
        {
            get
            {
                if (_target == null)
                    _target = DefaultTarget;
                return _target;
            }
            private set
            {
                if (_target != value)
                {
                    _target = value;
                    if (OnTargetChange != null)
                        OnTargetChange.Invoke();
                }
            }
        }

        public void ResetTarget() { _target = null; }

        private EnumRootLevel _currentLevel;
        public EnumRootLevel CurrenLevel { get { return _currentLevel; } set { _currentLevel = value; } }

        private List<CheatItem> _cheatItems = new List<CheatItem>();

        public CheatSystemManager()
        {
            Init();

#if UNITY_EDITOR
            _currentLevel = EnumRootLevel.Max;
#else
            _currentLevel = EnumRootLevel.Player;
#endif
        }

        /// <summary>
        /// 获取标题提示（信息为 权限+当前所有命令模块）
        /// </summary>
        /// <returns></returns>
        public string GetCheatCommandTips()
        {
            //string com = "";
            //for (int i = 0; i < _cheatItems.Count; i++)
            //{
            //    com += ConstLanguage.Get(ConstLanguage.CommandName, _cheatItems[i].GetType().Name) + " ";
            //}
            return ConstLanguage.Get(ConstLanguage.CommandTips, GetRootLevelDetail(CurrenLevel).Name);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string RunCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) return ConstLanguage.ErrorNullCommand;
            command = command.Trim();
            string[] cmds = command.Split(' ');
            CheatItem item = _cheatItems.Find((i) => i.GetType().Name == cmds[0]);

            if (item == null)
                return ConstLanguage.Get(ConstLanguage.ErrorNotFindCommand, cmds[0]);

            //无参数默认命令
            if (cmds.Length == 1)
            {
                #region 处理只有一个单位的默认命令
                return RunMethod(item, "DefaultSingleMethod", null);
                #endregion
            }

            //有一个参数默认命令
            if (item.GetCommandNameList().FindIndex((cmdName) => cmdName == cmds[1]) == -1)
            {
                object[] vals = new object[cmds.Length - 1];
                Array.Copy(cmds, 1, vals, 0, vals.Length);
                return RunMethod(item, "DefaultOneParamsMethod", vals);
            }

            string name = cmds[1];

            object[] parms = new object[cmds.Length - 2];
            Array.Copy(cmds, 2, parms, 0, parms.Length);

            return RunMethod(item, name, parms);
        }

        /// <summary>
        /// 获取指定可以进行自动补全命令的列表
        /// </summary>
        public List<string> GetCommandCompletionList(string str)
        {
            string[] cmds = str.Split(' ');
            if (cmds.Length < 1)
                return null;
            //模块名
            if (cmds.Length == 1)
            {
                return _cheatItems.FindAll((i) => i.GetType().Name.ToLower().StartsWith(str.Trim().ToLower())).ConvertAll<string>((citem) => citem.GetType().Name);
            }

            CheatItem item = GetParseCheatItem(cmds[0]);
            string name = item.GetType().Name;
            List<string> cmdList = new List<string>();
            cmdList = item.GetCommandNameList().FindAll((cmdName) => cmdName.ToLower().StartsWith(cmds[1].ToLower())).ConvertAll<string>((rawName) => name + rawName);
            return cmdList;
        }

        /// <summary>
        /// 自动补全命令
        /// </summary>
        public string GetAutoCompltion(string str)
        {
            string[] cmds = str.Split(' ');
            if (cmds.Length < 1)
                return "";
            //模块名
            if (cmds.Length == 1)
            {
                CheatItem it = GetParseCheatItem(cmds[0]);
                if (it == null) return str;
                return it.GetType().Name;
            }

            //模块名+方法名
            CheatItem item = GetParseCheatItem(cmds[0]);
            if (item == null) return str;
            cmds[0] = item.GetType().Name;
            cmds[1] = item.GetCommandNameList().Find((name) => name.ToLower().StartsWith(cmds[1].ToLower()));
            if (cmds[1] == null)
                return str;
            return string.Join(" ", cmds);
        }

        /// <summary>
        /// 获取指定命令的帮助信息
        /// </summary>
        public string GetTargetCommandHelp(string cmdName)
        {
            CheatItem item = _cheatItems.Find((i) => i.GetType().Name == cmdName);
            if (item == null)
                return ConstLanguage.Get(ConstLanguage.ErrorNotFindCommand, cmdName);

            CommandInfo expl = (CommandInfo)System.Attribute.GetCustomAttribute(item.GetType(), typeof(CommandInfo));

            return ConstLanguage.Get("{0}>>>>>{1}\n\n{2}", string.IsNullOrEmpty(expl.Name) ? item.GetType().Name : expl.Name, expl.Explain, item.GetCommandDescList());
        }

        /// <summary>
        /// 获取所有命令列表
        /// </summary>
        /// <returns></returns>
        public string GetCommandList()
        {
            string com = "";
            for (int i = 0; i < _cheatItems.Count; i++)
            {
                CommandInfo expl = (CommandInfo)System.Attribute.GetCustomAttribute(_cheatItems[i].GetType(), typeof(CommandInfo));
                com += ConstLanguage.Get(ConstLanguage.CommandName, _cheatItems[i].GetType().Name);
                if (expl != null)
                    com += ": " + expl.Explain + "\n";
            }
            return ConstLanguage.Get(ConstLanguage.CommandHelp, com);
        }

        /// <summary>
        /// RayCast 拾取目标
        /// </summary>
        public void RayCheckTarget()
        {
            UnityEngine.RaycastHit hit;
            if (UnityEngine.Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 100))
            {
                ICheatDetectable target = hit.transform.GetComponent<ICheatDetectable>();
                if (target != null)
                    Target = target;
            }
        }

        /// <summary>
        /// 获取指定权限信息
        /// </summary>
        public static Passward GetRootLevelDetail(EnumRootLevel level)
        {
            Passward p = System.Attribute.GetCustomAttribute(level.GetType().GetField(level.ToString()), typeof(Passward)) as Passward;
            return p;
        }

        private void Init()
        {
            Type cit = typeof(CheatItem);
            Assembly asm = this.GetType().Assembly;
            Type[] types = asm.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsSubclassOf(cit))
                {
                    CheatItem item = asm.CreateInstance(types[i].FullName) as CheatItem;
                    _cheatItems.Add(item);
                }
            }
        }

        /// <summary>
        /// 获取方法上的CommandInfo属性
        /// </summary>
        private CommandInfo GetCommandInfo(MethodInfo met)
        {
            return (CommandInfo)System.Attribute.GetCustomAttribute(met, typeof(CommandInfo));
        }

        private string RunMethod(CheatItem item, string name, object[] parms)
        {
            try
            {
                //获取模块信息
                MethodInfo met = item.GetType().GetMethod(name);
                CommandInfo info = GetCommandInfo(met);
                if (info != null && !info.CanExecute)
                    return ConstLanguage.Get(ConstLanguage.ErrorCommandPolicy, info.RequireLevelName);

                ParameterInfo[] paramsInfo = met.GetParameters();
                if (parms != null)
                {
                    if (parms.Length != paramsInfo.Length)
                        Array.Resize<object>(ref parms, paramsInfo.Length);
                    for (int i = 0; i < paramsInfo.Length; i++)
                    {
                        if (parms[i] != null)
                            parms[i] = Convert.ChangeType(parms[i], paramsInfo[i].ParameterType);
                        else
                            parms[i] = paramsInfo[i].DefaultValue;
                    }
                }

                object res = met.Invoke(item, parms);
                return res == null ? "" : ConstLanguage.Get(ConstLanguage.CommandResult, res.ToString());
            }
            catch (Exception ex)
            {
                string cmdList = item.GetCommandDescList();
                if (string.IsNullOrEmpty(cmdList))
                    return ConstLanguage.Get(ConstLanguage.ErrorCommand, ex.Message);
                return ConstLanguage.Get(ConstLanguage.ErrorCommandParams, ex.Message, cmdList);
            }
        }

        /// <summary>
        /// 获取指定命令的CheatItem（忽略大小写）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private CheatItem GetCheatItem(string str)
        {
            CheatItem item = _cheatItems.Find((i) => i.GetType().Name.ToLower() == (str.Trim().ToLower()));
            return item;
        }

        /// <summary>
        /// 获取指定命令名字开头与之匹配的CheatItem（忽略大小写）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private CheatItem GetParseCheatItem(string str)
        {
            CheatItem item = _cheatItems.Find((i) => i.GetType().Name.ToLower().StartsWith(str.Trim().ToLower()));
            return item;
        }
    }
}