# Unity3D-CheatSystem
单机游戏都有的东西，类似于控制台模式的测试系统。

![](https://github.com/CWHISME/Unity3D-CheatSystem/raw/master/Image/Snipaste_2019-01-29_19-06-35.png)

##使用

### 打开

在任何地方调用UICheatSystem.GetInstance.Active();即可。
示例场景默认提供CheatSystem脚本，按下”~“键打开。

输入Help可查看相关帮助。
按下Tab键进行自动补全，键盘方向键可进行历史命令的快捷输入。

### 增加新命令
继承于CheatItem即可
如：

[CommandInfo("导出")]
public class Export : CheatItem
{

}
CommandInfo用于命令的显示提示文本，分为两者：
#### 1
CommandInfo(string str, EnumRootLevel level = EnumRootLevel.Player)
其中第二位为使用者权限，默认权限为“玩家”，即所有打开控制台者皆可使用。
#### 2
CommandInfo(string name, string str, EnumRootLevel level = EnumRootLevel.Player)
第一位为名字，后续参数与第一项相同，若采用该重载，名字会使用这里赋值的。
否则名字显示为方法本身名字(针对于SingleMethod方法，SingleMethod作用后续解释)。

继承于CheatItem类中，所有添加了CommandInfo的公共方法，均可在控制台进行使用，使用方法为“类名 方法名 参数”。
若方法有默认参数，使用时可不传对应默认参数。
方法的返回值若不为void，则该返回值会用于控制台命令执行完毕后，显示为提示信息。

DefaultSingleMethod作用：
无参数默认方法，直接使用类名运行的功能，如Clear功能，因为实现了DefaultSingleMethod，因此可以直接通过类名调用。
DefaultOneParamsMethod：
作用与DefaultSingleMethod相同，但是可以接收一个参数。

如：
[CommandInfo("导出当前控制台文本（默认路径）")]
public string ExportText()
{
    return ExportTextWithPath(Application.dataPath + "/../ExportConsole.txt");
}


### 注
Class不一定需要放在CheatItems文件夹下，只要类继承了CheatItem即可。
