namespace DotNetRunner.HttpLogging.BodyFormat;

public interface IBodyFormatter
{
    public bool TryFormate(string httpBody, out string result);
}