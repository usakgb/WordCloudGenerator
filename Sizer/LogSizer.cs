using System;
using System.Collections.Generic;
using System.Linq;

namespace WordCloudGenerator.Sizer;

public class LogSizer : IFontSizer
{
    private readonly int _fontDelta;
    private readonly int _minFontSize;
    private readonly double _minLog;
    private readonly double _divisor;

    public LogSizer(WordCloudSettings settings, IEnumerable<WordEntry> entries)
    {
        _fontDelta = settings.MaxFont - settings.MinFont;
        _minFontSize = settings.MinFont;

        var counts = entries.Select(e => e.Count).DefaultIfEmpty(0).ToList();
        var min = counts.Min();
        var max = counts.Max();
        _minLog = Math.Log(Math.Max(min, 1));
        _divisor = Math.Log(Math.Max(max, 1)) - _minLog;
    }

    public double GetFontSize(int count)
    {
        var ratio = _divisor == 0.0 ? 1.0 : (Math.Log(Math.Max(count, 1)) - _minLog) / _divisor;
        return _minFontSize + _fontDelta * ratio;
    }
}