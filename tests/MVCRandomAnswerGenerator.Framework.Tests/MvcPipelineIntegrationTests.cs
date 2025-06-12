using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MVCRandomAnswerGenerator.Controllers;
using MVCRandomAnswerGenerator.Models;
using Xunit;

namespace MVCRandomAnswerGenerator.Framework.Tests
{
    /// <summary>
    /// Integration tests for MVC pipeline functionality.
    /// These tests validate the core MVC routing and controller functionality without requiring external mocking frameworks.
    /// </summary>
    public class MvcPipelineIntegrationTests
    {
        private RouteCollection CreateRoutes()
        {
            var routes = new RouteCollection();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            return routes;
        }

        [Fact]
        public void RouteCollection_CanBeCreated_WithDefaultRoute()
        {
            // Arrange & Act
            var routes = CreateRoutes();

            // Assert
            Assert.NotNull(routes);
            Assert.True(routes.Count > 0);
        }

        [Fact]
        public void ControllerFactory_CanCreateHomeController()
        {
            // Arrange
            var factory = new DefaultControllerFactory();

            // Act
            var controller = factory.CreateController(null, "Home");

            // Assert
            Assert.NotNull(controller);
            Assert.IsType<HomeController>(controller);
            
            // Cleanup
            factory.ReleaseController(controller);
        }

        [Fact]
        public void HomeController_Index_GET_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            
            var viewResult = result as ViewResult;
            Assert.IsType<List<QuestionAndAnswer>>(viewResult.Model);
        }

        [Fact]
        public void HomeController_Index_POST_AddsQuestionToModel()
        {
            // Arrange
            var controller = new HomeController();
            const string testQuestion = "Integration test question?";

            // Act
            var result = controller.Index(testQuestion);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<QuestionAndAnswer>;
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal(testQuestion, model.First().Question);
        }

        [Fact]
        public void HomeController_About_ReturnsViewWithMessage()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.About();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            
            var viewResult = result as ViewResult;
            Assert.Equal("The ASP.NET MVC Random Answer Generator", viewResult.ViewBag.Message);
        }

        [Fact]
        public void StaticState_PersistsAcrossControllerInstances()
        {
            // Arrange
            var controller1 = new HomeController();
            var controller2 = new HomeController();
            const string testQuestion = "State persistence test?";

            // Act
            controller1.Index(testQuestion); // Add question using first controller
            var result = controller2.Index(); // Get state using second controller

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<QuestionAndAnswer>;
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal(testQuestion, model.First().Question);
        }

        [Fact]
        public void MultipleQuestions_MaintainLIFOOrder()
        {
            // Arrange
            var controller = new HomeController();
            const string firstQuestion = "First question?";
            const string secondQuestion = "Second question?";

            // Act
            controller.Index(firstQuestion);
            controller.Index(secondQuestion);
            var result = controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<QuestionAndAnswer>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
            Assert.Equal(secondQuestion, model.First().Question); // Latest question first (LIFO)
        }

        [Fact]
        public void AnswerGenerator_ProducesConsistentAnswers()
        {
            // Arrange
            var controller = new HomeController();
            const string testQuestion = "Consistency test question?";

            // Act
            controller.Index(testQuestion);
            var result1 = controller.Index();
            var result2 = controller.Index();

            // Assert
            var viewResult1 = result1 as ViewResult;
            var model1 = viewResult1.Model as List<QuestionAndAnswer>;
            
            var viewResult2 = result2 as ViewResult;
            var model2 = viewResult2.Model as List<QuestionAndAnswer>;
            
            Assert.NotNull(model1);
            Assert.NotNull(model2);
            Assert.Equal(model1.First().Answer, model2.First().Answer); // Same question should give same answer
        }
    }
}