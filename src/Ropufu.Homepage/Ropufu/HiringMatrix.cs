using System.ComponentModel.DataAnnotations;

namespace Ropufu.Homepage;

/// <summary>
/// A hiring matrix presents the distribution of reviewers amoung applications.
/// It achieves two purposes:
/// -- Randomizes the application review order for each reviewer.
/// -- Handles the case when there are too many applications for each to be
///    reviewed by the entire committee.
/// Rows of the hiring matrix correspond to applications. Columns of the matrix
/// tell which reviewers should handle each application, and in which order.
/// </summary>
[Validatable]
public class HiringMatrix : IValidatable
{
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int CountApplicants { get; set; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int CountReviewers { get; set; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int CountReviewersPerApplication { get; set; } = 1;

    public int ApplicationsPerReviewer =>
        (this.CountApplicants * this.CountReviewersPerApplication) / this.CountReviewers;

    public string? ErrorMessage
    {
        get
        {
            if (this.CountApplicants < 1)
                return "There must be at least one applicant.";
            if (this.CountReviewers < 1)
                return "There must be at least one reviewer.";
            if (this.CountReviewersPerApplication < 1)
                return "Each application must have at least one reviewer.";
            if (this.CountReviewersPerApplication > this.CountReviewers)
                return "An application cannot be reviewed multiple times by the same reviewer.";
            return IValidatable.Success;
        }
    }

    /// <summary>
    /// Checks if the hiring matrix is valid.
    /// </summary>
    public bool IsValid => this.ErrorMessage == IValidatable.Success;

    /// <summary>
    /// Constructs a list of headers for the hiring matrix columns.
    /// </summary>
    /// <returns></returns>
    public string[] BuildHeaders()
    {
        int n = this.CountReviewers;
        var result = new string[2 * n + 1];

        result[0] = "Application #";
        for (var j = 0; j < n; ++j)
        {
            // Indicates if j'th reviewer should skip (0) or not (1) this application.
            result[1 + j] = $"Indicator {j + 1}";
            // Indicates when j'th reviewer should review this application.
            result[1 + n + j] = $"Review Sequence {j + 1}";
        } // for (...)

        return result;
    }

    /// <summary>
    /// Builds the hiring matrix.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">The hiring matrix is in an invalid state.</exception>
    /// <exception cref="NotSupportedException">Requested hiring matrix is currently not supported.</exception>
    /// <exception cref="ApplicationException">Internal error. Something that should not have happened just did.</exception>
    public int[,] BuildMatirx()
    {
        if (!this.IsValid)
            throw new InvalidOperationException();
        if (!this.TryBuildMatrix(out int[,] result))
            throw new NotSupportedException();
        return result;
    }

    /// <summary>
    /// Tries to build the hiring matrix.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool TryBuildMatrix(out int[,] result)
    {
        int m = this.CountApplicants;
        int n = this.CountReviewers;
        result = new int[m, 2 * n + 1];
        var random = new Random();

        if (!this.TryFillIndicators(result, random))
            return false;

        for (var i = 0; i < m; ++i)
            result[i, 0] = i + 1;

        for (var j = 0; j < n; ++j)
        {
            int[] applicantIndex = random.NextPermutation(m, 1);
            for (var i = 0; i < m; ++i)
            {
                result[i, 1 + n + j] = applicantIndex[i];
            } // for (...)
        } // for (...)

        return true;
    }

    /// <summary>
    /// Tries to fill out the indicator portion of the hiring matrix.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    private bool TryFillIndicators(int[,] result, Random random)
    {
        int m = this.CountApplicants;
        int n = this.CountReviewers;
        int k = this.CountReviewersPerApplication;

        if (k == n)
        {
            for (var i = 0; i < m; ++i)
                for (var j = 0; j < n; ++j)
                    result[i, 1 + j] = 1;
            return true;
        } // if (...)
        else if (k == n - 1)
        {
            int[] applicantPermutation = random.NextPermutation(m);
            int[] reviewerPermutation = random.NextPermutation(n);
            for (var i = 0; i < m; ++i)
            {
                for (var j = 0; j < n; ++j)
                {
                    result[applicantPermutation[i], 1 + reviewerPermutation[j]] = (((i + j) % n) == 0 ? 0 : 1);
                } // for (...)
            } // for (...)
            return true;
        } // else if (...)
        return false;
    }
}
