using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MVCRandomAnswerGenerator.Controllers;
using MVCRandomAnswerGenerator.Models;
using Xunit;

namespace MVCRandomAnswerGenerator.Framework.Tests
{
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

        private HttpContextBase CreateHttpContext(string url, string httpMethod = "GET")
        {
            var context = new Moq.Mock<HttpContextBase>();
            var request = new Moq.Mock<HttpRequestBase>();
            var response = new Moq.Mock<HttpResponseBase>();
            var session = new Moq.Mock<HttpSessionStateBase>();
            var server = new Moq.Mock<HttpServerUtilityBase>();

            request.Setup(x => x.AppRelativeCurrentExecutionFilePath).Returns(url);
            request.Setup(x => x.HttpMethod).Returns(httpMethod);
            request.Setup(x => x.PathInfo).Returns("");
            request.Setup(x => x.Form).Returns(new System.Collections.Specialized.NameValueCollection());
            request.Setup(x => x.QueryString).Returns(new System.Collections.Specialized.NameValueCollection());
            request.Setup(x => x.Files).Returns(new Moq.Mock<HttpFileCollectionBase>().Object);

            context.Setup(x => x.Request).Returns(request.Object);
            context.Setup(x => x.Response).Returns(response.Object);
            context.Setup(x => x.Session).Returns(session.Object);
            context.Setup(x => x.Server).Returns(server.Object);

            return context.Object;
        }

        [Fact]
        public void RouteData_HomeIndex_MapsCorrectly()
        {
            // Arrange
            var routes = CreateRoutes();
            var context = CreateHttpContext("~/");

            // Act
            var routeData = routes.GetRouteData(context);

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("Home", routeData.Values["controller"]);
            Assert.Equal("Index", routeData.Values["action"]);
        }

        [Fact]
        public void RouteData_HomeAbout_MapsCorrectly()
        {
            // Arrange
            var routes = CreateRoutes();
            var context = CreateHttpContext("~/Home/About");

            // Act
            var routeData = routes.GetRouteData(context);

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("Home", routeData.Values["controller"]);
            Assert.Equal("About", routeData.Values["action"]);
        }

        [Fact]
        public void ControllerFactory_CanCreateHomeController()
        {
            // Arrange
            var factory = new DefaultControllerFactory();
            var context = CreateHttpContext("~/");
            var routeData = new RouteData();
            routeData.Values["controller"] = "Home";

            // Act
            var controller = factory.CreateController(new RequestContext(context, routeData), "Home");

            // Assert
            Assert.NotNull(controller);
            Assert.IsType<HomeController>(controller);
        }

        [Fact]
        public void ControllerExecution_Index_GET_ExecutesSuccessfully()
        {
            // Arrange
            var controller = new HomeController();
            var context = CreateHttpContext("~/", "GET");
            var routeData = new RouteData();
            routeData.Values["controller"] = "Home";
            routeData.Values["action"] = "Index";

            var controllerContext = new ControllerContext(context, routeData, controller);
            controller.ControllerContext = controllerContext;

            // Act
            var result = controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            
            var viewResult = result as ViewResult;
            Assert.IsType<List<QuestionAndAnswer>>(viewResult.Model);
        }

        [Fact]
        public void ControllerExecution_Index_POST_ExecutesSuccessfully()
        {
            // Arrange
            var controller = new HomeController();
            var context = CreateHttpContext("~/", "POST");
            var routeData = new RouteData();
            routeData.Values["controller"] = "Home";
            routeData.Values["action"] = "Index";

            var controllerContext = new ControllerContext(context, routeData, controller);
            controller.ControllerContext = controllerContext;

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
        public void ControllerExecution_About_ExecutesSuccessfully()
        {
            // Arrange
            var controller = new HomeController();
            var context = CreateHttpContext("~/Home/About", "GET");
            var routeData = new RouteData();
            routeData.Values["controller"] = "Home";
            routeData.Values["action"] = "About";

            var controllerContext = new ControllerContext(context, routeData, controller);
            controller.ControllerContext = controllerContext;

            // Act
            var result = controller.About();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            
            var viewResult = result as ViewResult;
            Assert.Equal("The ASP.NET MVC Random Answer Generator", viewResult.ViewBag.Message);
        }

        [Fact]
        public void MvcPipeline_EndToEnd_HomeIndex_WorksCorrectly()
        {
            // Arrange
            var routes = CreateRoutes();
            var context = CreateHttpContext("~/");
            var routeData = routes.GetRouteData(context);
            
            var factory = new DefaultControllerFactory();
            var controller = factory.CreateController(new RequestContext(context, routeData), "Home") as HomeController;
            
            var controllerContext = new ControllerContext(context, routeData, controller);
            controller.ControllerContext = controllerContext;

            // Act
            var result = controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            
            var viewResult = result as ViewResult;
            Assert.IsType<List<QuestionAndAnswer>>(viewResult.Model);
        }

        [Fact]
        public void MvcPipeline_EndToEnd_PostQuestion_WorksCorrectly()
        {
            // Arrange
            var routes = CreateRoutes();
            var context = CreateHttpContext("~/", "POST");
            var routeData = routes.GetRouteData(context);
            
            var factory = new DefaultControllerFactory();
            var controller = factory.CreateController(new RequestContext(context, routeData), "Home") as HomeController;
            
            var controllerContext = new ControllerContext(context, routeData, controller);
            controller.ControllerContext = controllerContext;

            const string testQuestion = "End-to-end test question?";

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
            Assert.NotNull(model.First().Answer);
            Assert.NotEmpty(model.First().Answer);
        }

        [Fact]
        public void MvcPipeline_StatePersistence_AcrossRequests()
        {
            // Arrange - First request
            var controller1 = new HomeController();
            var context1 = CreateHttpContext("~/", "POST");
            var routeData1 = new RouteData();
            routeData1.Values["controller"] = "Home";
            routeData1.Values["action"] = "Index";
            var controllerContext1 = new ControllerContext(context1, routeData1, controller1);
            controller1.ControllerContext = controllerContext1;

            // Arrange - Second request
            var controller2 = new HomeController();
            var context2 = CreateHttpContext("~/", "GET");
            var routeData2 = new RouteData();
            routeData2.Values["controller"] = "Home";
            routeData2.Values["action"] = "Index";
            var controllerContext2 = new ControllerContext(context2, routeData2, controller2);
            controller2.ControllerContext = controllerContext2;

            const string testQuestion = "State persistence test?";

            // Act
            controller1.Index(testQuestion); // Add question
            var result = controller2.Index(); // Get current state

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<QuestionAndAnswer>;
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal(testQuestion, model.First().Question);
        }
    }
}