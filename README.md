# WordCloudGenerator

A lightweight, cross-platform .NET library for arranging [word clouds](https://en.wikipedia.org/wiki/Tag_cloud) (tag clouds). It computes word positions and font sizes using a spiral layout and quadtree collision detection, leaving rendering to your application.

Based on [KnowledgePicker.WordCloud](https://github.com/knowledgepicker/word-cloud), with a simplified layout-only API.

[![NuGet](https://img.shields.io/nuget/v/WordCloudGenerator.svg)](https://www.nuget.org/packages/WordCloudGenerator/)

## Features

- **Spiral layout** — Wordle-style placement from the center outward
- **Logarithmic font sizing** — `LogSizer` scales fonts by word frequency
- **Quadtree collision detection** — fast overlap checks while placing words
- **Cross-platform** — .NET Standard 2.1 with no `System.Drawing` or other platform-specific dependencies
- **Pluggable measurement** — bring your own text measurer for WPF, WinUI, SkiaSharp, SVG, etc.
- **Layout only** — no rendering engine bundled; use the output with any UI framework

## Installation

### NuGet (recommended)

```bash
dotnet add package WordCloudGenerator
```

Or in Visual Studio: **Project → Manage NuGet Packages → Browse → WordCloudGenerator**

Package: [nuget.org/packages/WordCloudGenerator](https://www.nuget.org/packages/WordCloudGenerator)

```xml
<PackageReference Include="WordCloudGenerator" Version="2.0.0" />
```

## Quick start

```csharp
using WordCloudGenerator;

var entries = new List<WordEntry>
{
    new("design", 42),
    new("code", 28),
    new("test", 15),
    new("build", 9),
};

var settings = new WordCloudSettings(
    width: 600,
    height: 400,
    minFont: 10,
    maxFont: 36,
    measure: (text, fontSize) =>
    {
        // Replace with real text measurement from your UI framework.
        var width = (int)Math.Ceiling(text.Length * fontSize * 0.6);
        var height = (int)Math.Ceiling(fontSize);
        return new Rectangle(0, 0, width, height);
    });

var cloud = new WordCloud(settings);
var result = cloud.Generate(entries);

foreach (var item in result.Placed)
{
    Console.WriteLine($"{item.Entry.Word}: ({item.Location.X}, {item.Location.Y}), font {item.FontSize:0.#}");
}

if (result.Skipped.Count > 0)
{
    Console.WriteLine($"{result.Skipped.Count} words did not fit.");
}
```

Each `LayoutItem` contains:

| Member | Description |
|--------|-------------|
| `Entry` | The `WordEntry` (`Word`, `Count`) |
| `Location` | Top-left position on the canvas |
| `Measured` | Measured text bounds used for placement |
| `FontSize` | Computed font size |
| `Rectangle` | Bounding box (`Location` + `Measured.Size`) |

## Custom text measurement

Provide an `IMeasurer` implementation when your renderer has a dedicated measurement API:

```csharp
public sealed class MyMeasurer : IMeasurer
{
    public Rectangle Measure(string text, double fontSize)
    {
        // Measure with your UI framework and return width/height.
        return new Rectangle(0, 0, width, height);
    }
}

var settings = new WordCloudSettings(600, 400, 10, 36, new MyMeasurer());
```

Accurate measurement is important. Placement quality depends on how closely the measurer matches the text you render.

### WinUI example

Measure with a WinUI `TextBlock`, then place controls on a `Canvas`:

```csharp
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using WordCloudGenerator;
using Windows.Foundation.Size;

var entries = new List<WordEntry>
{
    new("design", 42),
    new("code", 28),
    new("test", 15),
};

var settings = new WordCloudSettings(600, 400, 10, 35, (text, fontSize) =>
{
    var textBlock = new TextBlock { Text = text, FontSize = fontSize };
    textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
    return new Rectangle(0, 0, (int)textBlock.DesiredSize.Width, (int)textBlock.DesiredSize.Height);
});

var result = new WordCloud(settings).Generate(entries);

wordCloudCanvas.Children.Clear();
foreach (var word in result.Placed)
{
    var element = new TextBlock
    {
        Text = word.Entry.Word,
        FontSize = word.FontSize,
    };

    Canvas.SetLeft(element, word.Location.X);
    Canvas.SetTop(element, word.Location.Y);
    wordCloudCanvas.Children.Add(element);
}
```

Use the same font family, weight, and style in the measurer and in the rendered control so layout matches what you draw.

## Overflow behavior

When the canvas runs out of space, `Generate` returns a `WordCloudResult` with:

| Member | Description |
|--------|-------------|
| `Placed` | Words arranged inside the canvas |
| `Skipped` | Words that were not placed |

Control this with `WordCloudSettings.OverflowBehavior`:

```csharp
settings.OverflowBehavior = OverflowBehavior.Skip; // default
```

| Value | Behavior |
|-------|----------|
| `Skip` | Skip words that do not fit and keep trying smaller or later words |
| `Stop` | Stop arranging when the next word does not fit |

Only in-bounds words are returned in `Placed`.

## Custom layout or font sizing

`Generate` accepts optional layout and sizer implementations:

```csharp
using WordCloudGenerator.Layout;
using WordCloudGenerator.Sizer;

var layout = new SpiralLayout(settings);
var sizer = new LogSizer(settings, entries);

var result = cloud.Generate(entries, layout, sizer);
var items = result.Placed;
```

Implement `ILayout` or `IFontSizer` to experiment with alternative placement or sizing strategies.

## Building and testing

```bash
dotnet build WordCloudGenerator.csproj
dotnet test WordCloudGenerator.Tests/WordCloudGenerator.Tests.csproj
```

## Algorithm

The placement algorithm follows the same lineage as KnowledgePicker.WordCloud:

1. Sort words by frequency and assign font sizes with `LogSizer`
2. Measure each word's bounding box
3. Search along a spiral from the canvas center for a non-overlapping position
4. Track occupied space in a quadtree

Originally inspired by [SourceCodeCloud](https://archive.codeplex.com/?p=sourcecodecloud) and [Wordle](https://web.archive.org/web/20201206102909/http://www.wordle.net/).

## License

This project is licensed under the [MIT License](LICENSE).

It is derived from [KnowledgePicker.WordCloud](https://github.com/knowledgepicker/word-cloud) by Jan Joneš, also licensed under the MIT License. The original copyright notice is included in [LICENSE](LICENSE).

## Acknowledgements

- [KnowledgePicker.WordCloud](https://github.com/knowledgepicker/word-cloud) — upstream implementation and algorithm
- [KnowledgePicker](https://knowledgepicker.com) — maintenance of the original library
- [SourceCodeCloud](https://archive.codeplex.com/?p=sourcecodecloud) — original algorithm port