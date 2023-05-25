using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ropufu.Homepage.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace Ropufu.Homepage.Controllers;

using CourseGraph = Graph<Course, Prerequisite>;
using CourseVertex = Vertex<Course, Prerequisite>;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private const string HighlightedVertexColor = "#ffaaaa";

    private readonly DbSet<Course> _courses;
    private readonly DbSet<Prerequisite> _prerequisites;

    public CoursesController(ApplicationDbContext context)
    {
        _courses = context.Courses ?? throw new ArgumentException("Database context malformed.");
        _prerequisites = context.Prerequisites ?? throw new ArgumentException("Database context malformed.");
    }

    private bool TryBuildCourseGraph(out CourseGraph result)
    {
        result = new CourseGraph();

        // Add vertices.
        foreach (Course c in _courses)
            result.AddVertex(c);

        foreach (Prerequisite p in _prerequisites
            .Include(p => p.Course)
            .Include(p => p.RequiredCourse))
        {
            if (p.RequiredCourse is null)
                return false;
            if (p.Course is null)
                return false;

            if (!result.TryFindFirstVertex(c => object.ReferenceEquals(c, p.RequiredCourse), out CourseVertex from))
                return false;
            if (!result.TryFindFirstVertex(c => object.ReferenceEquals(c, p.Course), out CourseVertex to))
                return false;

            result.AddEdge(from, to, p);
        } // foreach(...)
        return true;
    }

    private static CytoscapeGraph ToCytoscape(CourseGraph graph, Course? highlight = null)
    {
        var defaultNode = new CytoscapeNode();
        var minimizer = new NameMinimizer('a', 'z');
        return CytoscapeGraph.FromGraph(graph,
            c => new CytoscapeNode()
            {
                Id = minimizer.Next(),
                Code = $"{c.Label.Prefix.Trim()}-{c.Label.Number}",
                Name = $"{c.Label.Prefix.Trim()}-{c.Label.Number}\n{c.Label.Name}",
                Weight = 0,
                Background = (object.ReferenceEquals(c.Label, highlight) ?
                    CoursesController.HighlightedVertexColor :
                    defaultNode.Background)
            },
            (p, cn) => cn.Weight += 1,
            (p, cn) => { }
        );
    }

    // GET api/<controller>
    [HttpGet]
    [Produces("application/json")]
    public IActionResult Get()
    {
        if (!this.TryBuildCourseGraph(out CourseGraph graph))
            return this.BadRequest($"Something went wrong when building the graph.");

        CytoscapeGraph response = CoursesController.ToCytoscape(graph);
        return new JsonResult(response);
    }

    // GET api/<controller>/math-170
    [HttpGet("{prefix:alpha}-{number:int:min(100):max(999)}")]
    [Produces("application/json")]
    public IActionResult Get(string prefix, int number)
    {
        if (!this.TryBuildCourseGraph(out CourseGraph graph))
            return this.BadRequest($"Something went wrong when building the graph.");

        bool predicate(Course c) =>
            c.Number == number &&
            c.Prefix == prefix;

        if (!graph.TryFindFirstVertex(predicate, out CourseVertex vertex))
            return this.BadRequest($"Course not found.");

        CourseGraph connected = graph.ConnectedComponentWith(vertex);

        CytoscapeGraph response = CoursesController.ToCytoscape(connected);
        return new JsonResult(response);
    }
}
