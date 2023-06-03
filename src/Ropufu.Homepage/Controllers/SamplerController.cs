using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace Ropufu.Homepage.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SamplerController : ControllerBase
{
    public const int MaxPopulation = 1000;

    // Selects a SRS of size \c sampleSize from the population of \c populationSize elements.
    //
    // Algorithm outline.
    // Random:                                    *
    // Previous step: 1 -- 2 -- ... -- (m - 1) -- m -- (m + 1) -- ... -- (N - 1) -- N
    // After removal: 1 -- 2 -- ... -- (m - 1) ------- (m + 1) -- ... -- (N - 1) -- N
    // Next step:     1 -- 2 -- ... -- (m - 1) -- N -- (m + 1) -- ... -- (N - 1)
    private static int[] Choose(int populationSize, int sampleSize)
    {
        var result = new int[sampleSize];
        Dictionary<int, int> sparsePermutation = new(capacity: sampleSize);
        Random r = new();

        for (int k = 0; k < sampleSize; ++k, --populationSize)
        {
            int chosenIndex = r.Next(1, populationSize);
            // Get the original population index for chosen item.
            result[k] = sparsePermutation.TryGetValue(chosenIndex, out int shuffledIndex)
                ? shuffledIndex
                : chosenIndex;
            // Move the last item in the population to replace the removed item.
            sparsePermutation[chosenIndex] = sparsePermutation.TryGetValue(populationSize, out int tailIndex)
                ? tailIndex
                : populationSize;
        } // for (...)

        return result;
    }

    // GET api/sampler/12/choose/7
    [HttpGet("{populationSize:int}/choose/{sampleSize:int}")]
    [Produces("text/plain")]
    public IActionResult Get(int populationSize, int sampleSize)
    {
        if (populationSize <= 0)
            return this.BadRequest($"There should be at least one item in the population.");
        if (populationSize > SamplerController.MaxPopulation)
            return this.BadRequest($"Sorry, cannot handle population of more than {SamplerController.MaxPopulation}.");

        if (sampleSize <= 0)
            return this.BadRequest($"There should be at least one item to select.");
        if (sampleSize > populationSize)
            return this.BadRequest($"Sample size cannot exceed population size.");

        return this.Ok(JsonSerializer.Serialize(
            SamplerController.Choose(populationSize, sampleSize)
        ));
    }
}
