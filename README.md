# AvaloniaHotMarkdown

AvaloniaHotMarkdown is an Obsidian-inspired live-preview Markdown editor for Avalonia.

See AvaloniaHotMarkdown.Demo for a ready-to-use example.

[![Nuget.AvaloniaHotMarkdown](https://img.shields.io/nuget/v/AvaloniaHotMarkdown?label=Nuget&style=flat-square)](https://www.nuget.org/packages/AvaloniaHotMarkdown/)

![FIANL](https://github.com/user-attachments/assets/7732a4f6-86f7-48f6-8eb2-a4b88a04fdf2)

## Current supported blocks:
- Headings
- Unordered lists
- Ordered lists
- Strikethrough, bold, underline and italic texts
- Highlights
- Checkboxes
- Pipe Tables
- Thematic breaks
- Links
- Images
- `Code inlines`
- Subscript
- Superscript
- Blockquotes

If you need any feature, please open an issue or submit a PR. I will try to add it as soon as possible.

## Install the Library

Open your terminal in the project root directory. Run this command to install the package:

```bash
dotnet add package AvaloniaHotMarkdown
```

Alternatively, open the NuGet Package Manager in your IDE. Search for AvaloniaHotMarkdown and select install.

## Configure the XAML

Open your Avalonia XAML file. Add the namespace declaration to the root element:

```xml
xmlns:HotMarkdown="clr-namespace:AvaloniaHotMarkdown;assembly=AvaloniaHotMarkdown"
```

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:vm="using:AvaloniaHotMarkdown.Demo.ViewModels"
             mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
             x:Class="AvaloniaHotMarkdown.Demo.Views.MainView"

             xmlns:HotMarkdown="clr-namespace:AvaloniaHotMarkdown;assembly=AvaloniaHotMarkdown"
        
             x:DataType="vm:MainViewModel">
```

## Add the Control

Place the editor control inside your layout view.

```xml
<HotMarkdown:HotMarkdownEditor Padding="10"/>
```
