using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    }
}
