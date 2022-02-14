using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace Ropufu.Homepage.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HiringController : ControllerBase
{
    public const int MaxApplicants = 1000;
    public const int MaxReviewers = 10;

    // GET api/hiring/12/345/0
    // GET api/hiring/345/12/0
    [HttpGet("{countA:int}/{countB:int}/{offload:int?}")]
    [Produces("text/csv")]
    public IActionResult Get(int countA, int countB, int offload = 0)
    {
        int countApplicants = Math.Max(countA, countB);
        int countReviewers = Math.Min(countA, countB);
        int countReviewersPerApplication = countReviewers + offload;

        if (countApplicants <= 0)
            return this.BadRequest($"There should be at least one applicant.");
        if (countApplicants > HiringController.MaxApplicants)
            return this.BadRequest($"Sorry, cannot handle more than {HiringController.MaxApplicants} applicants.");

        if (countReviewers <= 0)
            return this.BadRequest($"There should be at least one reviewer.");
        if (countReviewers > HiringController.MaxApplicants)
            return this.BadRequest($"Sorry, cannot handle more than {HiringController.MaxReviewers} reviewer.");

        if (offload > 0)
            return this.BadRequest($"Offload should be zero or negative.");
        if (offload < -1)
            return this.BadRequest($"Sorry, cannot handle more than one reviewer per application offload.");

        if (countReviewersPerApplication <= 0)
            return this.BadRequest($"Somebody has to do the reviews.");

        HiringMatrix matrix = new()
        {
            CountApplicants = countApplicants,
            CountReviewers = countReviewers,
            CountReviewersPerApplication = countReviewers + offload
        };

        if (!matrix.IsValid)
            return this.BadRequest($"Something went wrong when initializing the matrix.");

        if (!matrix.TryBuildMatrix(out int[,] result))
            return this.BadRequest($"Something went wrong when building the matrix.");

        string text = StaticHelper.ToCommaSeparatedValues(result, matrix.BuildHeaders());
        return this.File(System.Text.Encoding.ASCII.GetBytes(text), "text/csv", "hiring.csv");
    }
}
