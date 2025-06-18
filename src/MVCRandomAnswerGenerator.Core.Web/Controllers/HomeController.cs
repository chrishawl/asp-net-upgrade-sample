using Microsoft.AspNetCore.Mvc;
using MVCRandomAnswerGenerator.Core.Domain;
using MVCRandomAnswerGenerator.Core.Web.Services;
using System.ComponentModel.DataAnnotations;

namespace MVCRandomAnswerGenerator.Core.Web.Controllers;

/// <summary>
/// Home controller for the ASP.NET Core Random Answer Generator application.
/// Provides endpoints for asking questions and getting Magic 8-Ball style answers.
/// </summary>
public class HomeController(
    IAnswerGenerator answerGenerator,
    IQuestionAnswerService questionAnswerService,
    ILogger<HomeController> logger) : Controller
{
    /// <summary>
    /// Displays the main page with all previously asked questions and answers.
    /// </summary>
    /// <returns>The main view with the list of questions and answers.</returns>
    [HttpGet]
    public IActionResult Index()
    {
        logger.LogInformation("Displaying questions and answers page");
        var allAnswers = questionAnswerService.GetAll();
        return View(allAnswers);
    }

    /// <summary>
    /// Processes a new question and generates an answer.
    /// </summary>
    /// <param name="nextQuestion">The question to answer. Cannot be null or empty.</param>
    /// <returns>Redirects to the Index action to display updated results, or returns the view with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index([Required] string nextQuestion)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for question submission");
            var allAnswers = questionAnswerService.GetAll();
            return View(allAnswers);
        }

        try
        {
            logger.LogInformation("Processing question: {Question}", nextQuestion);
            
            var answer = answerGenerator.GenerateAnswer(nextQuestion);
            var questionAndAnswer = new QuestionAndAnswer(nextQuestion, answer, DateTime.UtcNow);
            
            questionAnswerService.Add(questionAndAnswer);
            
            logger.LogInformation("Added new Q&A: {Question} -> {Answer}", nextQuestion, answer);
            
            // Use PRG (Post-Redirect-Get) pattern to prevent duplicate submissions
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing question: {Question}", nextQuestion);
            ModelState.AddModelError("", "An error occurred while processing your question. Please try again.");
            
            var allAnswers = questionAnswerService.GetAll();
            return View(allAnswers);
        }
    }

    /// <summary>
    /// Displays information about the application.
    /// </summary>
    /// <returns>The About view.</returns>
    [HttpGet]
    public IActionResult About()
    {
        logger.LogInformation("Displaying About page");
        ViewData["Message"] = "The ASP.NET Core Random Answer Generator";
        return View();
    }
}