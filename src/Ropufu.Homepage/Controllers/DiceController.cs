using Microsoft.AspNetCore.Mvc;
using System.Globalization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace Ropufu.Homepage.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiceController : ControllerBase
{
    public const int MaxDice = 100;
    public const int MaxSides = 120;
    public const int RngLimit = 1000;

    private static string Roll(int countDice, int countSides, int dropLow, int dropHigh, int repeatCount)
    {
        var result = new string[repeatCount];
        Random r = new();

        if (dropLow == 0 && dropHigh == 0)
        {
            for (var k = 0; k < repeatCount; ++k)
            {
                var outcome = 0;
                for (var i = 0; i < countDice; ++i)
                    outcome += r.Next(1, countSides + 1);
                result[k] = outcome.ToString(CultureInfo.InvariantCulture);
            } // for (...)
        } // if (...)
        else
        {
            var diceValues = new int[countDice];

            for (var k = 0; k < repeatCount; ++k)
            {
                for (var i = 0; i < countDice; ++i)
                    diceValues[i] = r.Next(1, countSides + 1);
                Array.Sort(diceValues);

                var outcome = 0;
                for (var i = dropLow; i < countDice - dropHigh; ++i)
                    outcome += diceValues[i];
                result[k] = outcome.ToString(CultureInfo.InvariantCulture);
            } // for (...)
        } // else
        return string.Join(Environment.NewLine, result);
    }

    // GET api/dice/d6
    [HttpGet("d{countSides:int}")]
    [Produces("text/plain")]
    public IActionResult GetSingeDie(int countSides) =>
        this.Get(1, countSides, dropLow: 0, dropHigh: 0, repeatCount: 1);

    // GET api/dice/4d6
    [HttpGet("{countDice:int}d{countSides:int}")]
    [Produces("text/plain")]
    public IActionResult GetMultipleDice(int countDice, int countSides) =>
        this.Get(countDice, countSides, dropLow: 0, dropHigh: 0, repeatCount: 1);

    // GET api/dice/5d12/100
    [HttpGet("{countDice:int}d{countSides:int}/{repeatCount:int}")]
    [Produces("text/plain")]
    public IActionResult GetRepeatedKeepAll(int countDice, int countSides, int repeatCount) =>
        this.Get(countDice, countSides, dropLow: 0, dropHigh: 0, repeatCount);

    // GET api/dice/5d12/2L/100
    [HttpGet("{countDice:int}d{countSides:int}/{dropLow:int}L/{repeatCount:int}")]
    [Produces("text/plain")]
    public IActionResult GetRepeatedDropLow(int countDice, int countSides, int dropLow, int repeatCount) =>
        this.Get(countDice, countSides, dropLow, dropHigh: 0, repeatCount);

    // GET api/dice/5d12/2H/100
    [HttpGet("{countDice:int}d{countSides:int}/{dropHigh:int}H/{repeatCount:int}")]
    [Produces("text/plain")]
    public IActionResult GetRepeatedDropHigh(int countDice, int countSides, int dropHigh, int repeatCount) =>
        this.Get(countDice, countSides, dropLow: 0, dropHigh, repeatCount);

    // GET api/dice/6d20/1L2H/100
    [HttpGet("{countDice:int}d{countSides:int}/{dropLow:int}L{dropHigh:int}H/{repeatCount:int}")]
    [HttpGet("{countDice:int}d{countSides:int}/{dropHigh:int}H{dropLow:int}L/{repeatCount:int}")]
    [Produces("text/plain")]
    public IActionResult Get(int countDice, int countSides, int dropLow, int dropHigh, int repeatCount)
    {
        if (countDice <= 0)
            return this.BadRequest($"There should be at least one die.");
        if (countDice > DiceController.MaxDice)
            return this.BadRequest($"Sorry, cannot handle more than {DiceController.MaxDice} dice.");

        if (countSides <= 0)
            return this.BadRequest($"There should be at least one side on a die.");
        if (countSides > DiceController.MaxSides)
            return this.BadRequest($"Sorry, cannot handle anything greater than d{DiceController.MaxSides}.");

        if (dropLow < 0)
            return this.BadRequest($"Number of low dice discarded cannot be negative.");
        if (dropHigh < 0)
            return this.BadRequest($"Number of high dice discarded cannot be negative.");
        if (dropLow + dropHigh >= countDice)
            return this.BadRequest($"You cannot discard all your dice.");

        if (repeatCount <= 0)
            return this.BadRequest($"There should be at least one replication of the experiment.");
        if (countDice * repeatCount > DiceController.RngLimit)
            return this.BadRequest($"Sorry, cannot repeat this experiment more than {(DiceController.RngLimit / countDice)} times.");

        return this.Ok(DiceController.Roll(countDice, countSides, dropLow, dropHigh, repeatCount));
    }
}
