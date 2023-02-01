using System.Text.Json.Nodes;

namespace DotNetRunner.HttpLogging.BodyFormat.Json;

public class JsonPropertyIgnoreFormatter : IBodyFormatter
{
    private readonly PropertyTree _tree;

    public JsonPropertyIgnoreFormatter(params string[] propNames)
    {
        _tree = new PropertyTree(propNames);
    }

    public bool TryFormate(string httpBody, out string result)
    {
        JsonObject obj = null;
        try
        {
            obj = JsonObject.Parse(httpBody).AsObject();
        }
        catch
        {
            result = null;
            return false;
        }

        RemoveProperties(obj, _tree.RootNodes);

        result = obj.ToJsonString();
        return true;
    }

    private void RemoveProperties(JsonObject obj, IReadOnlyCollection<PropertyTreeNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (obj.ContainsKey(node.Key))
            {
                if (!node.IsEndNode)
                {
                    RemoveProperties(obj[node.Key].AsObject(), node.Children);
                }
                
                obj.Remove(node.Key);
            }
        }
    }
}