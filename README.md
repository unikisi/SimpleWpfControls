# SimpleWpfControls

一个轻量的 WPF 自定义控件库。目前主要提供一个带输入过滤与校验能力的输入框控件：`ValidatedTextBox`。

## 功能介绍

`ValidatedTextBox` 是一个继承自 `TextBox` 的控件，面向 MVVM 使用场景，内置输入格式限制与常见校验，并将校验状态/错误文案以可绑定的属性形式暴露出来。

- 输入模式：文本 / 整数 / 小数（`Mode`）
- 输入过滤：在输入阶段拦截非法字符；小数模式支持限制小数位数（`DecimalPlaces`）
- 校验能力：
  - 必填（`IsRequired`）
  - 文本长度（`MinimumLength` / `MaximumLength`）
  - 数值范围（`Minimum` / `Maximum`，整数/小数模式）
- 校验触发：文本变化或失焦触发（`ValidationTrigger`）
- 校验结果可绑定：
  - `HasValidationError`（只读）
  - `ValidationMessage`（只读）
- 错误提示：校验失败时自动弹出 `ToolTip` 显示提示文案
- 单位展示：右侧可显示单位文本（`Units`）
- 提示文案可扩展：支持按文化/语言返回提示或全局替换默认提示（`IValidationMessageProvider` / `ValidatedTextBox.DefaultMessageProvider`）

## 运行环境

- `.NET`: `net8.0-windows`
- UI: WPF

## 快速开始

### 1) 引用

将本项目作为源码引入，或编译后在你的 WPF 项目中引用生成的程序集 `SimpleWpfControls.dll`。

### 2) 合并样式资源（推荐）

控件的默认模板/样式定义在资源字典 `SimpleWpfControlStyle.xaml` 中。使用时建议在你的 `App.xaml`（或任意资源字典）中合并：

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/SimpleWpfControls;component/SimpleWpfControlStyle.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## 使用示例

在 XAML 中引入命名空间后即可使用：

```xml
<Window
    xmlns:swc="clr-namespace:SimpleWpfControls;assembly=SimpleWpfControls">

    <StackPanel Margin="16" Width="260">

        <!-- 文本模式：必填 + 长度范围 -->
        <swc:ValidatedTextBox
            Margin="0,0,0,12"
            IsRequired="True"
            MinimumLength="2"
            MaximumLength="10"
            ValidationTrigger="OnLostFocus"
            Text="{Binding UserName, UpdateSourceTrigger=PropertyChanged}" />

        <!-- 整数模式：范围 + 单位 -->
        <swc:ValidatedTextBox
            Margin="0,0,0,12"
            Mode="Integer"
            Minimum="0"
            Maximum="120"
            Units="岁"
            ValidationTrigger="OnTextChanged"
            Text="{Binding AgeText, UpdateSourceTrigger=PropertyChanged}" />

        <!-- 小数模式：小数位数 + 是否允许负数 -->
        <swc:ValidatedTextBox
            Mode="Decimal"
            DecimalPlaces="2"
            AllowNegative="False"
            Units="kg"
            Text="{Binding WeightText, UpdateSourceTrigger=PropertyChanged}" />

    </StackPanel>
</Window>
```

### 绑定校验结果（MVVM）

可以绑定 `HasValidationError` / `ValidationMessage` 来驱动 UI（例如禁用按钮、显示汇总提示等）：

```xml
<swc:ValidatedTextBox x:Name="AmountBox"
                      Mode="Decimal"
                      Minimum="0"
                      ValidationTrigger="OnTextChanged"
                      Text="{Binding AmountText, UpdateSourceTrigger=PropertyChanged}" />

<TextBlock Margin="0,8,0,0"
           Foreground="Tomato"
           Text="{Binding ElementName=AmountBox, Path=ValidationMessage}" />
```

## 提示文案与多语言

默认提示由 `DefaultValidationMessageProvider` 提供，会根据 `CurrentUICulture`（`zh*` 视为中文）返回中/英文提示。

你也可以全局替换默认文案提供者：

```csharp
using SimpleWpfControls;

ValidatedTextBox.DefaultMessageProvider = new MyMessageProvider();
```

或在单个控件上为某一类校验指定自定义提示（例如 `RequiredMessage`、`MinimumValueMessage` 等）。

## 可用属性（摘要）

- 输入/过滤：`Mode`、`AllowNegative`、`DecimalPlaces`
- 校验触发：`ValidationTrigger`
- 通用校验：`IsRequired`、`MinimumLength`、`MaximumLength`
- 数值校验：`Minimum`、`Maximum`
- UI：`Units`
- 校验结果（只读）：`HasValidationError`、`ValidationMessage`
- 自定义提示：`RequiredMessage`、`MinimumLengthMessage`、`MaximumLengthMessage`、`LengthRangeMessage`、`MinimumValueMessage`、`MaximumValueMessage`

  <br />

