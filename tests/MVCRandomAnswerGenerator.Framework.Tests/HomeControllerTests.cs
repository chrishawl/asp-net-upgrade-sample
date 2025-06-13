using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MVCRandomAnswerGenerator.Controllers;
using MVCRandomAnswerGenerator.Models;
using Xunit;

namespace MVCRandomAnswerGenerator.Framework.Tests
{
    public class HomeControllerTests
    {
        private HomeController CreateController()
        {
            // Clear static state before each test to ensure isolation
            HomeController.ClearAllAnswers();
            return new HomeController();
        }

        [Fact]
        public void Index_GET_ReturnsViewWithModel()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<QuestionAndAnswer>>(result.Model);
        }

        [Fact]
        public void Index_GET_ReturnsEmptyListInitially()
        {
            // Arrange
            var controller = CreateController();

            // Act  
            var result = controller.Index() as ViewResult;
            var model = result.Model as List<QuestionAndAnswer>;

            // Assert
            Assert.NotNull(model);
            Assert.Empty(model);
        }

        [Fact]
        public void Index_POST_AddsQuestionToModel()
        {
            // Arrange
            var controller = CreateController();
            const string testQuestion = "Will this test pass?";

            // Act
            var result = controller.Index(testQuestion) as ViewResult;
            var model = result.Model as List<QuestionAndAnswer>;

            // Assert
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal(testQuestion, model.First().Question);
            Assert.NotNull(model.First().Answer);
            Assert.NotEmpty(model.First().Answer);
        }

        [Fact]
        public void Index_POST_ReturnsViewWithUpdatedModel()
        {
            // Arrange
            var controller = CreateController();
            const string testQuestion = "Test question?";

            // Act
            var result = controller.Index(testQuestion) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<QuestionAndAnswer>>(result.Model);
            
            var model = result.Model as List<QuestionAndAnswer>;
            Assert.Single(model);
        }

        [Fact]
        public void Index_POST_MultipleQuestions_AddsToBeginningOfList()
        {
            // Arrange
            var controller = CreateController();
            const string firstQuestion = "First question?";
            const string secondQuestion = "Second question?";

            // Act
            controller.Index(firstQuestion);
            var result = controller.Index(secondQuestion) as ViewResult;
            var model = result.Model as List<QuestionAndAnswer>;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
            // Most recent question should be first
            Assert.Equal(secondQuestion, model.First().Question);
            Assert.Equal(firstQuestion, model.Last().Question);
        }

        [Fact]
        public void Index_POST_WithNullQuestion_HandlesGracefully()
        {
            // Arrange
            var controller = CreateController();
            string nullQuestion = null;

            // Act & Assert
            // This will throw ArgumentNullException from AnswerGenerator.GenerateAnswer()
            Assert.Throws<ArgumentNullException>(() => controller.Index(nullQuestion));
        }

        [Fact]
        public void Index_POST_WithEmptyQuestion_AddsToModel()
        {
            // Arrange
            var controller = CreateController();
            const string emptyQuestion = "";

            // Act
            var result = controller.Index(emptyQuestion) as ViewResult;
            var model = result.Model as List<QuestionAndAnswer>;

            // Assert
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal(emptyQuestion, model.First().Question);
            Assert.NotNull(model.First().Answer);
        }

        [Fact]
        public void Index_POST_GeneratesAnswerUsingAnswerGenerator()
        {
            // Arrange
            var controller = CreateController();
            const string testQuestion = "Test question for answer generation?";

            // Act
            var result = controller.Index(testQuestion) as ViewResult;
            var model = result.Model as List<QuestionAndAnswer>;

            // Get expected answer directly from AnswerGenerator
            var expectedAnswer = AnswerGenerator.GenerateAnswer(testQuestion);

            // Assert
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal(expectedAnswer, model.First().Answer);
        }

        [Fact]
        public void About_ReturnsViewWithCorrectMessage()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("The ASP.NET MVC Random Answer Generator", result.ViewBag.Message);
        }

        [Fact]
        public void About_ReturnsView()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = controller.About();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_POST_PreservesOrderOfMultipleQuestions()
        {
            // Arrange
            var controller = CreateController();
            var questions = new[]
            {
                "Question 1?",
                "Question 2?", 
                "Question 3?",
                "Question 4?",
                "Question 5?"
            };

            // Act
            ViewResult lastResult = null;
            foreach (var question in questions)
            {
                lastResult = controller.Index(question) as ViewResult;
            }

            var model = lastResult.Model as List<QuestionAndAnswer>;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(questions.Length, model.Count);
            
            // Questions should be in reverse order (most recent first)
            for (int i = 0; i < questions.Length; i++)
            {
                Assert.Equal(questions[questions.Length - 1 - i], model[i].Question);
            }
        }

        [Fact]
        public void Index_POST_SameQuestionGeneratesSameAnswer()
        {
            // Arrange
            var controller = CreateController();
            const string question = "Same question for consistency test?";

            // Act
            var result1 = controller.Index(question) as ViewResult;
            var result2 = controller.Index(question) as ViewResult;
            
            var model1 = result1.Model as List<QuestionAndAnswer>;
            var model2 = result2.Model as List<QuestionAndAnswer>;

            // Assert
            Assert.NotNull(model1);
            Assert.NotNull(model2);
            Assert.Equal(2, model2.Count); // Should have both questions
            
            // Both instances of the same question should have the same answer
            var firstAnswer = model2.First().Answer; // Most recent
            var secondAnswer = model2.Last().Answer; // Older one
            Assert.Equal(firstAnswer, secondAnswer);
        }

        [Fact]
        public void Controller_StaticState_PersistsAcrossInstances()
        {
            // Arrange
            HomeController.ClearAllAnswers(); // Start with clean state
            var controller1 = new HomeController(); // Don't use CreateController() for this test
            var controller2 = new HomeController(); // Don't use CreateController() for this test
            const string question = "Cross-instance persistence test?";

            // Act
            controller1.Index(question);
            var result = controller2.Index() as ViewResult;
            var model = result.Model as List<QuestionAndAnswer>;

            // Assert
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal(question, model.First().Question);
        }

        [Fact]
        public void Index_POST_HasValidateAntiForgeryTokenAttribute()
        {
            // Arrange
            var controller = CreateController();
            var method = controller.GetType().GetMethod("Index", new[] { typeof(string) });

            // Act
            var attributes = method.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), false);

            // Assert
            Assert.NotEmpty(attributes);
            Assert.IsType<ValidateAntiForgeryTokenAttribute>(attributes[0]);
        }

        [Fact]
        public void Index_POST_HasHttpPostAttribute()
        {
            // Arrange
            var controller = CreateController();
            var method = controller.GetType().GetMethod("Index", new[] { typeof(string) });

            // Act
            var attributes = method.GetCustomAttributes(typeof(HttpPostAttribute), false);

            // Assert
            Assert.NotEmpty(attributes);
            Assert.IsType<HttpPostAttribute>(attributes[0]);
        }
    }
}