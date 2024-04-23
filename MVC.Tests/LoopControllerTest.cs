// namespace MVC.Tests
// {
//     public class LoopControllerTests
//     {
//         // Arrange
//         [Fact]
//         public async Task TestDeleteReturnsRedirectToActionIndex()
//         {
//             // Arrange
//             var loopId = 1; // Sample loop ID
//             var loopServiceMock = new Mock<ILoopServiceInterface>();
//             loopServiceMock.Setup(service => service.DeleteLoop(loopId)).Returns(Task.CompletedTask);
//             var loggerMock = new Mock<ILogger<LoopController>>();
//             var loopController = new LoopController(loopServiceMock.Object, loggerMock.Object);
//
//             // Act
//             var result = await loopController.Delete(loopId) as RedirectToActionResult;
//
//             // Assert
//             Assert.NotNull(result);
//             Assert.Equal("Index", result.ActionName); // Verify that it redirects to the "Index" action
//             loopServiceMock.Verify(service => service.DeleteLoop(loopId), Times.Once); // Verify that DeleteLoop method was called with the correct loop ID
//         }
//     }
// }