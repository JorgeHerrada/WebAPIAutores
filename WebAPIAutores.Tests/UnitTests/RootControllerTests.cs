using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPIAutores.Tests.Mocks;
using WebAppAutores.Controllers.v1;

namespace WebAPIAutores.Tests.UnitTests
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task When_GetMethodIsCalled_IfUserIsAdmin_ThenAListOfFourItemsIsReturned()
        {
            // Arrange
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.DesiredResult = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Act
            var result = await rootController.Get();

            // Assert
            Assert.AreEqual(4, result.Value.Count());
        }

        [TestMethod]
        public async Task When_GetMethodIsCalled_IfUserIsNotAdmin_ThenAListOfTwoItemsIsReturned()
        {
            // Arrange
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.DesiredResult = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Act
            var result = await rootController.Get();

            // Assert
            Assert.AreEqual(2, result.Value.Count());
        }

        [TestMethod]
        public async Task When_GetMethodIsCalled_IfUserIsNotAdmin_ThenAListOfTwoItemsIsReturned_UsingMoqLibrary()
        {
            // Arrange

            // Mocking the IAuthorizationService
            var mockAuth = new Mock<IAuthorizationService>(); // defines its type
            mockAuth.Setup(auth =>  // defines the params when .AuthorizeAsync method is used
                auth.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(), // IsAny -> the value doesn't matter
                    It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed())); // refines what should be returned

            mockAuth.Setup(auth =>
                auth.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockURLHelper = new Mock<IUrlHelper>();
            mockURLHelper.Setup(urlHelper =>
                urlHelper.Link(
                    It.IsAny<string>(),
                    It.IsAny<object>()
                )).Returns(string.Empty);

            var rootController = new RootController(mockAuth.Object);

            rootController.Url = mockURLHelper.Object;

            // Act
            var result = await rootController.Get();

            // Assert
            Assert.AreEqual(2, result.Value.Count());
        }
    }
}
