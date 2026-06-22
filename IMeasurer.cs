namespace WordCloudGenerator;

public interface IMeasurer
{
    Rectangle Measure(string text, double fontSize);
}