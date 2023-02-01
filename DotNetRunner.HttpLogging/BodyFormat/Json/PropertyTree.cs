namespace DotNetRunner.HttpLogging.BodyFormat.Json;

internal class PropertyTree
{
    public IReadOnlyCollection<PropertyTreeNode> RootNodes { get; }

    public PropertyTree(IEnumerable<string> properies)
    {
        var parsedProps = properies
            .Select(p => new Stack<string>(p.Split(".").Reverse()))
            .ToArray();
        RootNodes = CreateNodes(parsedProps);
    }

    private IReadOnlyCollection<PropertyTreeNode> CreateNodes(Stack<string>[] props)
    {
        if (!props.Any())
        {
            return null;
        }

        var nodes = new List<PropertyTreeNode>();
        var groupedProps = props
            .Where(x => x.Any())
            .GroupBy(x => x.Pop())
            .Select(x => (x.Key, PropStack: x.ToArray()));

        foreach (var prop in groupedProps)
        {
            nodes.Add(new PropertyTreeNode()
            {
                Key = prop.Key,
                Children = CreateNodes(prop.PropStack)
            });
        }

        return nodes.ToArray();
    }
}

internal class PropertyTreeNode
{
    public bool IsEndNode => Children?.Any() != true;
    public string Key { get; init; }
    public IReadOnlyCollection<PropertyTreeNode> Children { get; init; }
}