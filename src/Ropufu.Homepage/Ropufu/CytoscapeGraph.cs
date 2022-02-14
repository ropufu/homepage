using System.Text.Json.Serialization;

namespace Ropufu.Homepage;

public class CytoscapeDataElement<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    public static implicit operator CytoscapeDataElement<T>(T value)
    {
        return new CytoscapeDataElement<T> { Data = value };
    }
}

public class CytoscapeNode
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("weight")]
    public int Weight { get; set; } = 1;

    [JsonPropertyName("background")]
    public string Background { get; set; } = "white";
}

public class CytoscapeEdge
{
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("target")]
    public string? Target { get; set; }
}

public class CytoscapeGraph
{
    [JsonPropertyName("nodes")]
    public List<CytoscapeDataElement<CytoscapeNode>> Nodes { get; } = new();

    [JsonPropertyName("edges")]
    public List<CytoscapeDataElement<CytoscapeEdge>> Edges { get; } = new();

    public static CytoscapeGraph FromGraph<TVertex, TEdge>(Graph<TVertex, TEdge> source,
        Func<Vertex<TVertex, TEdge>, CytoscapeNode> vertexConverter,
        Action<Edge<TVertex, TEdge>, CytoscapeNode> fromVertexModifier,
        Action<Edge<TVertex, TEdge>, CytoscapeNode> toVertexModifier)
            where TVertex : new()
    {
        Dictionary<Vertex<TVertex, TEdge>, CytoscapeNode> vertexMap = new();
        CytoscapeGraph cytoscape = new();

        foreach (Vertex<TVertex, TEdge> vertex in source.Vertices)
        {
            CytoscapeNode cytoscapeVertex = vertexConverter(vertex);
            cytoscape.Nodes.Add(cytoscapeVertex);
            vertexMap.Add(vertex, cytoscapeVertex);
        } // foreach (...)

        foreach (Edge<TVertex, TEdge> edge in source.Edges)
        {
            CytoscapeNode cytoscapeFrom = vertexMap[edge.From];
            CytoscapeNode cytoscapeTo = vertexMap[edge.To];

            fromVertexModifier(edge, cytoscapeFrom);
            toVertexModifier(edge, cytoscapeTo);

            cytoscape.Edges.Add(new CytoscapeEdge
            {
                Source = cytoscapeFrom.Id,
                Target = cytoscapeTo.Id
            });
        } // foreach (...)

        return cytoscape;
    }
}
