namespace Ropufu.Homepage;

public class Vertex<TVertex, TEdge>
    where TVertex : new()
{
    private readonly List<Edge<TVertex, TEdge>> _incoming = new();
    private readonly List<Edge<TVertex, TEdge>> _outgoing = new();

    public IEnumerable<Edge<TVertex, TEdge>> Incoming => this._incoming.AsReadOnly();
    public IEnumerable<Edge<TVertex, TEdge>> Outgoing => this._outgoing.AsReadOnly();

    public TVertex Label { get; set; }

    public int Order => this._incoming.Count + this._outgoing.Count;

    public Vertex(TVertex label)
    {
        this.Label = label;
    }

    public Edge<TVertex, TEdge> MakeEdgeTo(Vertex<TVertex, TEdge> to, TEdge label)
    {
        Edge<TVertex, TEdge> edge = new(this, to, label);
        this._outgoing.Add(edge);
        to._incoming.Add(edge);
        return edge;
    }
}

public class Edge<TVertex, TEdge>
    where TVertex : new()
{
    private readonly Vertex<TVertex, TEdge> _from;
    private readonly Vertex<TVertex, TEdge> _to;

    public Vertex<TVertex, TEdge> From => this._from;
    public Vertex<TVertex, TEdge> To => this._to;

    public TEdge Label { get; set; }

    public Edge(Vertex<TVertex, TEdge> from, Vertex<TVertex, TEdge> to, TEdge label)
    {
        this._from = from;
        this._to = to;
        this.Label = label;
    }
}

/// <summary>
/// A highly redundant and unoptimized yet fairly straightforward class for storing directed graphs.
/// </summary>
/// <typeparam name="TVertex">Value type for vertices.</typeparam>
/// <typeparam name="TEdge">Value type for edges.</typeparam>
public class Graph<TVertex, TEdge>
    where TVertex : new()
{
    private static readonly Vertex<TVertex, TEdge> s_missingVertex = new(new TVertex());
    private readonly List<Vertex<TVertex, TEdge>> _vertices = new();
    private readonly List<Edge<TVertex, TEdge>> _edges = new();

    public IEnumerable<Vertex<TVertex, TEdge>> Vertices => this._vertices.AsReadOnly();

    public IEnumerable<Edge<TVertex, TEdge>> Edges => this._edges.AsReadOnly();

    public bool TryFindFirstVertex(Func<TVertex, bool> predicate, out Vertex<TVertex, TEdge> vertex)
    {
        vertex = Graph<TVertex, TEdge>.s_missingVertex;
        foreach (Vertex<TVertex, TEdge> item in this._vertices)
            if (predicate(item.Label))
            {
                vertex = item;
                return true;
            } // if (...)
        return false;
    }

    public bool Empty => this._vertices.Count == 0;

    public Vertex<TVertex, TEdge> AddVertex(TVertex label)
    {
        Vertex<TVertex, TEdge> vertex = new(label);
        this._vertices.Add(vertex);
        return vertex;
    }

    public Edge<TVertex, TEdge> AddEdge(Vertex<TVertex, TEdge> from, Vertex<TVertex, TEdge> to, TEdge label)
    {
        Edge<TVertex, TEdge> edge = from.MakeEdgeTo(to, label);
        this._edges.Add(edge);
        return edge;
    }

    public Graph<TVertex, TEdge> ConnectedComponentWith(Vertex<TVertex, TEdge> target)
    {
        int countVertices = this._vertices.Count;
        int countEdges = this._edges.Count;

        Graph<TVertex, TEdge> subGraph = new();
        subGraph._vertices.Capacity = countVertices;
        subGraph._edges.Capacity = countEdges;

        // Make sure to only consider each vertex once.
        List<Vertex<TVertex, TEdge>> visited = new(countVertices);
        List<Vertex<TVertex, TEdge>> discoveredVertices = new(countVertices);
        discoveredVertices.Add(target);
        while (discoveredVertices.Count > 0)
        {
            List<Vertex<TVertex, TEdge>> extendedVertices = new();
            foreach (Vertex<TVertex, TEdge> discovered in discoveredVertices)
            {
                // Skip vertices that have already been visited.
                if (visited.Contains(discovered))
                    continue;

                // Mark the vertex as visited.
                visited.Add(discovered);
                // Add it to the subgraph.
                subGraph._vertices.Add(discovered);
                foreach (Edge<TVertex, TEdge> edge in discovered.Incoming)
                {
                    Vertex<TVertex, TEdge> from = edge.From;
                    // Record the newly discovered vertex.
                    // Note that the "from" vertex will be added only at the next iteration.
                    extendedVertices.Add(from);
                    // Add the edge to the subgraph.
                    subGraph._edges.Add(edge);
                } // foreach (...)
            } // foreach (...)
            discoveredVertices = extendedVertices;
        } // while (...)

        return subGraph;
    }
}
