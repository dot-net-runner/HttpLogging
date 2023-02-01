namespace DotNetRunner.HttpLogging.BodyFormat;

public class PatternBodyFormatter
{
    public IgnorePattern Pattern { get; init; }
    public IBodyFormatter RequestBodyFormatter { get; init; }
    public IBodyFormatter ResponseBodyFormatter { get; init; }
    
    public PatternBodyFormatter(IgnorePattern pattern, 
        IBodyFormatter requestBodyFormatter = null,
        IBodyFormatter responseBodyFormatter = null)
    {
        Pattern = pattern;
        RequestBodyFormatter = requestBodyFormatter;
        ResponseBodyFormatter = responseBodyFormatter;
    }
}