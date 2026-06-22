namespace WordCloudGenerator;

public class WordEntry
{
    public string Word { get; }
    public int Count { get; }

    public WordEntry(string word, int count)
    {
        Word = word;
        Count = count;
    }
}