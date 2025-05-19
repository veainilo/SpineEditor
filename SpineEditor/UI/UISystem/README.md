# GUILayout系统使用指南

本文档提供了GUILayout系统的使用指南，帮助您快速上手并有效地使用这个类似Unity的UI布局系统。

![GUILayout示例](https://i.imgur.com/JQGXxZP.png)

## 目录

- [基本概念](#基本概念)
- [快速开始](#快速开始)
- [基础控件](#基础控件)
- [布局容器](#布局容器)
- [布局选项](#布局选项)
- [辅助类](#辅助类)
- [基类](#基类)
- [最佳实践](#最佳实践)
- [示例](#示例)

## 基本概念

GUILayout系统是一个即时模式GUI系统，类似于Unity的GUILayout。它提供了自动布局功能，不需要手动指定每个控件的位置和大小。

主要特点：
- 自动布局：控件会自动排列，不需要手动指定位置
- 嵌套布局：可以创建复杂的嵌套布局
- 即时模式：每帧重新创建所有控件
- 类似Unity：API和使用方式与Unity的GUILayout非常相似

## 快速开始

### 1. 初始化

首先，需要初始化GUILayout系统：

```csharp
// 创建UI管理器
_uiManager = new UIManager(GraphicsDevice);

// 初始化GUILayout系统
GUILayout.Initialize(_uiManager, _font);
```

### 2. 使用GUILayout

在Draw方法中使用GUILayout：

```csharp
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    // 绘制GUI
    DrawGUI();

    // 绘制UI管理器
    _spriteBatch.Begin();
    _uiManager.Draw(_spriteBatch);
    _spriteBatch.End();

    base.Draw(gameTime);
}

private void DrawGUI()
{
    // 垂直布局
    GUILayout.BeginVertical();

    // 标签
    GUILayout.Label("标题");

    // 按钮
    if (GUILayout.Button("点击我"))
    {
        Console.WriteLine("按钮被点击");
    }

    // 文本框
    _text = GUILayout.TextField(_text);

    GUILayout.EndVertical();
}
```

## 基础控件

### 标签 (Label)

显示文本：

```csharp
GUILayout.Label("这是一个标签");
```

### 按钮 (Button)

创建一个按钮，返回是否被点击：

```csharp
if (GUILayout.Button("点击我"))
{
    Console.WriteLine("按钮被点击");
}
```

### 文本框 (TextField)

创建一个文本框，返回输入的文本：

```csharp
_text = GUILayout.TextField(_text);
```

## 布局容器

### 垂直布局 (BeginVertical/EndVertical)

垂直排列子元素：

```csharp
GUILayout.BeginVertical();
GUILayout.Label("第一行");
GUILayout.Label("第二行");
GUILayout.Label("第三行");
GUILayout.EndVertical();
```

### 水平布局 (BeginHorizontal/EndHorizontal)

水平排列子元素：

```csharp
GUILayout.BeginHorizontal();
GUILayout.Label("左");
GUILayout.Label("中");
GUILayout.Label("右");
GUILayout.EndHorizontal();
```

### 嵌套布局

可以嵌套使用垂直和水平布局：

```csharp
GUILayout.BeginVertical();
    GUILayout.Label("标题");

    GUILayout.BeginHorizontal();
        GUILayout.Label("左侧");
        GUILayout.Label("右侧");
    GUILayout.EndHorizontal();

    GUILayout.Label("底部");
GUILayout.EndVertical();
```

## 布局选项

可以使用布局选项控制控件的大小：

```csharp
// 设置宽度
GUILayout.Button("按钮", GUILayout.Width(100));

// 设置高度
GUILayout.Label("标签", GUILayout.Height(30));

// 同时设置宽度和高度
GUILayout.TextField("文本", GUILayout.Width(200), GUILayout.Height(30));
```

## 辅助类

`GUILayoutHelper` 提供了一些常用的辅助方法：

```csharp
// 绘制标题
GUILayoutHelper.Title("表单示例", 2);

// 绘制分隔线
GUILayoutHelper.Separator();

// 带标签的文本框
_name = GUILayoutHelper.LabelField("姓名", _name);

// 带标签的整数框
_age = GUILayoutHelper.IntField("年龄", _age);

// 带标签的浮点数框
_height = GUILayoutHelper.FloatField("身高", _height, "F2");

// 复选框
_agreeTerms = GUILayoutHelper.Toggle("同意条款", _agreeTerms);

// 向量2输入框
_position = GUILayoutHelper.Vector2Field("位置", _position);

// 选项卡
_selectedTab = GUILayoutHelper.Tabs(_selectedTab, _tabs);
```

## 基类

### GUILayoutWindow

窗口基类，提供了基本的窗口功能：

```csharp
public class MyWindow : GUILayoutWindow
{
    public MyWindow(string title, Rectangle bounds, GraphicsDevice graphicsDevice, SpriteFont font)
        : base(title, bounds, graphicsDevice, font)
    {
    }

    protected override void DrawGUI()
    {
        // 使用GUILayout绘制窗口内容
        GUILayout.BeginVertical();
        GUILayout.Label("这是一个窗口");
        GUILayout.EndVertical();
    }
}
```

### GUILayoutPanel

面板基类，提供了基本的面板功能：

```csharp
public class MyPanel : GUILayoutPanel
{
    public MyPanel(string title, Rectangle bounds, GraphicsDevice graphicsDevice, SpriteFont font)
        : base(title, bounds, graphicsDevice, font)
    {
    }

    protected override void DrawGUI()
    {
        // 使用GUILayout绘制面板内容
        GUILayout.BeginVertical();
        GUILayout.Label("这是一个面板");
        GUILayout.EndVertical();
    }
}
```

## 最佳实践

1. **组织代码**：将GUI代码组织到单独的方法中，使代码更清晰
2. **使用辅助类**：使用GUILayoutHelper减少重复代码
3. **继承基类**：继承GUILayoutWindow或GUILayoutPanel创建自定义窗口或面板
4. **避免深层嵌套**：过多的嵌套会使代码难以阅读和维护
5. **使用布局选项**：使用布局选项控制控件的大小，而不是手动计算
6. **不需要手动调用BeginFrame/EndFrame**：系统会自动处理这些步骤
7. **使用GUILayoutHelper创建常用控件组合**：减少重复代码，提高可读性

## 示例

### 基本表单

```csharp
GUILayout.BeginVertical();

GUILayoutHelper.Title("用户信息", 2);
GUILayoutHelper.Separator();

_name = GUILayoutHelper.LabelField("姓名", _name);
_email = GUILayoutHelper.LabelField("邮箱", _email);
_age = GUILayoutHelper.IntField("年龄", _age);
_agreeTerms = GUILayoutHelper.Toggle("我同意服务条款", _agreeTerms);

if (GUILayout.Button("提交", GUILayout.Width(100)))
{
    if (string.IsNullOrEmpty(_name))
    {
        Console.WriteLine("请输入姓名");
    }
    else if (string.IsNullOrEmpty(_email))
    {
        Console.WriteLine("请输入邮箱");
    }
    else if (!_agreeTerms)
    {
        Console.WriteLine("请同意服务条款");
    }
    else
    {
        Console.WriteLine($"表单提交成功: 姓名={_name}, 邮箱={_email}, 年龄={_age}");
    }
}

GUILayout.EndVertical();
```

### 选项卡界面

```csharp
GUILayout.BeginVertical();

_selectedTab = GUILayoutHelper.Tabs(_selectedTab, new[] { "基本信息", "详细信息", "设置" });

switch (_selectedTab)
{
    case 0:
        DrawBasicInfo();
        break;
    case 1:
        DrawDetailInfo();
        break;
    case 2:
        DrawSettings();
        break;
}

GUILayout.EndVertical();
```

更多示例请参考 `GUILayoutExamplePanel.cs` 文件。

## 与Unity GUILayout的区别

虽然我们的GUILayout系统在API和使用方式上与Unity的GUILayout非常相似，但仍有一些区别：

1. **控件种类**：Unity提供更多种类的控件，如Toggle、Slider、TextArea等
2. **布局选项**：Unity提供更丰富的布局选项，如MinWidth、MaxWidth、ExpandWidth等
3. **样式系统**：Unity有完整的GUIStyle和GUISkin系统，可以详细控制控件的外观
4. **特殊布局容器**：Unity提供BeginArea、BeginScrollView等特殊布局容器

## 扩展

如果需要添加新的控件或功能，可以：

1. 在GUILayout类中添加新的方法
2. 在GUILayoutHelper类中添加新的辅助方法
3. 创建新的继承自GUILayoutWindow或GUILayoutPanel的类
4. 修改现有控件的外观和行为

## 常见问题

### 控件不显示

- 检查是否正确初始化了GUILayout系统
- 检查是否正确调用了_uiManager.Draw()
- 确保每个BeginVertical/BeginHorizontal都有对应的EndVertical/EndHorizontal

### 布局混乱

- 检查是否正确嵌套了布局容器
- 避免过多的嵌套
- 使用布局选项控制控件的大小

### 性能问题

- 减少控件数量
- 避免每帧重新创建大量控件
- 使用GUILayoutPanel或GUILayoutWindow分割UI
