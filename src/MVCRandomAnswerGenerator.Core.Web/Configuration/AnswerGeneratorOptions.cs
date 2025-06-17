using System.ComponentModel.DataAnnotations;

namespace MVCRandomAnswerGenerator.Core.Web.Configuration;

/// <summary>
/// Configuration options for the Answer Generator feature.
/// </summary>
public sealed class AnswerGeneratorOptions
{
    /// <summary>
    /// The configuration section name for the AnswerGenerator options.
    /// </summary>
    public const string SectionName = "AnswerGenerator";

    /// <summary>
    /// Gets or sets the maximum number of questions to store in memory.
    /// </summary>
    [Range(1, 1000)]
    public int MaxStoredQuestions { get; set; } = 100;

    /// <summary>
    /// Gets or sets whether to enable question caching.
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the application title displayed in the UI.
    /// </summary>
    [Required]
    public string ApplicationTitle { get; set; } = "ASP.NET Core Random Answer Generator";

    /// <summary>
    /// Gets or sets the application description.
    /// </summary>
    [Required]
    public string ApplicationDescription { get; set; } = "A Magic 8-Ball style answer generator built with ASP.NET Core 8";
}