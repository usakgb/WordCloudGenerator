namespace WordCloudGenerator;

public enum OverflowBehavior
{
    /// <summary>
    /// Skip words that do not fit and continue placing smaller or later words.
    /// </summary>
    Skip = 0,

    /// <summary>
    /// Stop arranging when the next word does not fit.
    /// </summary>
    Stop = 1,
}