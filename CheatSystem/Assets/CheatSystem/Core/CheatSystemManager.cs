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

        public string GetCheatCommandTips()
        {
            string com = "";
            for (int i = 0; i < _cheatItems.Count; i++)
            {
                com += "<color=#00FF00>" + _cheatItems[i].GetType().Name + "</color>  ";
            }
            return "可用命令：" + com + "(当前权限：" + GetRootLevelDetail(CurrenLevel).Name + ")";
        }

        public string RunCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) return "<color=#FF00FF>错误：命令为空！</color>";
            //if (command == "Help")
            //    return GetCommandList();
            command = command.Trim();
            string[] cmds = command.Split(' ');
            CheatItem item = _cheatItems.Find((i) => i.GetType().Name == cmds[0]);

            if (item == null)
                return "<color=#FF00FF>错误：未找到命令 [" + cmds[0] + "]</color>\n有关命令帮助，请输入“Help”查看.";

            if (cmds.Length < 2)
            {
                #region 处理只有一个单位的命令
                MethodInfo met = item.GetType().GetMethod("SingleMethod");
                if (met != null)
                {
                    try
                    {
                        met.Invoke(item, null);
                        return "";
                    }
                    catch (Exception ex)
                    {
                        return "<color=#FF00FF>错误：" + ex.Message + "</color>";
                    }
                }
                #endregion

                return "<color=#FF00FF>错误：该命令需要至少一个参数。</color>\n" + cmds[0] + "可用参数有：\n" + item.GetCommandList();
            }

            string name = cmds[1];

            object[] parms = new object[cmds.Length - 2];

            if (cmds.Length > 2)
                for (int i = 2; i < cmds.Length; i++)
                {
                    parms[i - 2] = cmds[i];
                }

            try
            {
                MethodInfo met = item.GetType().GetMethod(name);
                CommandInfo info = GetCommandInfo(met);
                if (!info.CanExecute)
                {
                    return "<color=#FF00FF>权限不足：该命令要求至少为 " + info.LevelName + " 权限</color>";
                }

                object res = met.Invoke(item, parms);
                string resInfo = res == null ? "成功" : res.ToString();
                return "执行结果：" + resInfo;

            }
            catch (Exception ex)
            {
                return "<color=#FF00FF>错误：" + ex.Message + "</color>";
            }
        }

        public string GetTargetCommandHelp(string cmdName)
        {
            CheatItem item = _cheatItems.Find((i) => i.GetType().Name == cmdName);
            if (item == null)
                return "<color=#FF00FF>错误：未找到指定命令 [" + cmdName + "]</color>";

            CommandInfo expl = (CommandInfo)System.Attribute.GetCustomAttribute(item.GetType(), typeof(CommandInfo));

            return item.GetType().Name + ">>>>>" + expl.Explain + "\n\n" + item.GetCommandList();
        }

        public Passward GetRootLevelDetail(EnumRootLevel level)
        {
            Passward p = System.Attribute.GetCustomAttribute(level.GetType().GetField(level.ToString()), typeof(Passward)) as Passward;
            return p;
        }

        public string GetCommandList()
        {
            string com = "";
            for (int i = 0; i < _cheatItems.Count; i++)
            {
                CommandInfo expl = (CommandInfo)System.Attribute.GetCustomAttribute(_cheatItems[i].GetType(), typeof(CommandInfo));
                com += "\n<color=#00FF00>" + _cheatItems[i].GetType().Name + "</color>";
                if (expl != null)
                    com += ": " + expl.Explain;
            }
            return "帮助：你可以从以下命令中选择执行，命令格式按照 指令->方法与参数顺序进行.\n" + com;
        }

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

        private CommandInfo GetCommandInfo(MethodInfo met)
        {
            return (CommandInfo)System.Attribute.GetCustomAttribute(met, typeof(CommandInfo));
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
    }
}