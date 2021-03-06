# Unity3D-CheatSystem
单机游戏都有的东西，类似于控制台模式的测试系统。

![](https://github.com/CWHISME/Unity3D-CheatSystem/raw/master/Image/Snipaste_2019-01-29_19-06-35.png)

## 使用

### 打开

在任何地方调用UICheatSystem.GetInstance.Active();即可。
示例场景默认提供CheatSystem脚本，按下”~“键打开。

输入Help可查看相关帮助。
按下Tab键进行自动补全，键盘方向键可进行历史命令的快捷输入。

* 翻页功能

由于整个系统显示仅使用一个Text，Text所支持的文本显示具有上限（虽然很大，但是一直添加也会到头），因此特别增加了翻页功能。
在UIScrollViewText中，可设置_supportTextLineSize大小，代表每一页所具有的行数，超出该设置行数会自动进行分页。
右下角可查看当前所用总页面及当前所在页面，上下拖动至Scroll终点，自动切换页面。
在Prefab上的行数设置为：
```
[CheatSystem]->Background->Content->ScrollViewText->SupportTextLineSize
```
（注：该行数不计算非“\r\n”分割值，即直接“\n”分割的行数不算进去。若需要命令返回字符串不计入统计，参看命令Tools->SystemInfo处理。）

### 增加新命令
继承于CheatItem即可
如：
``` cs
[CommandInfo("导出")]
public class Export : CheatItem
{

}
```

CommandInfo用于命令的显示提示文本，分为两者：

* 1

CommandInfo(string str, EnumRootLevel level = EnumRootLevel.Player)
其中第二位为使用者权限，默认权限为“玩家”，即所有打开控制台者皆可使用。

* 2

CommandInfo(string name, string str, EnumRootLevel level = EnumRootLevel.Player)
第一位为名字，后续参数与第一项相同，若采用该重载，名字会使用这里赋值的。
否则名字显示为方法本身名字(针对于SingleMethod方法，SingleMethod作用后续解释)。

继承于CheatItem类中，所有添加了CommandInfo的公共方法，均可在控制台进行使用，使用方法为“类名 方法名 参数”。
若方法有默认参数，使用时可不传对应默认参数。
方法的返回值若不为void，则该返回值会用于控制台命令执行完毕后，显示为提示信息。

两个默认方法作用：
`DefaultSingleMethod`：
无参数默认方法，若继承于CheatItem的类中具有该方法，直接输入类名，即会调用。如Clear或者Root功能，因为实现了DefaultSingleMethod，因此可以直接通过类名调用器功能。
`DefaultOneParamsMethod`：
作用与DefaultSingleMethod相同，但是可以接收一个参数。

如：
``` cs
[CommandInfo("导出当前控制台文本（默认路径）")]
public string ExportText()
{
    return ExportTextWithPath(Application.dataPath + "/../ExportConsole.txt");
}
```
### 增加权限或修改密码
所有权限及密码位于EnumRootLevel类中，作为枚举存在，枚举之上放置Passward属性即可。
同时，密码在此保存为MD5格式，新增或修改密码也注意，必须先转化为MD5格式再粘贴过来。

### 代码使用

直接代码使用也是可以的，如果愿意，可以将功能写入CheatSystem中，执行RunCommand运行命令。
如：
``` cs
string rerult = CheatSystemManager.GetInstance.RunCommand(command);
Debug.Log(rerult);
```
这样的好处是，灵活，通过外部配置之类的控制部分功能执行。当然因为是反射，性能肯定会有那么点损失的。

### 注
Class不一定需要放在CheatItems文件夹下，只要类继承了CheatItem即可。
