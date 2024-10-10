
using Microsoft.AspNetCore.Mvc;
using Moq;
using CryptoFinAPI.Controllers;
using CryptoFinAPI.Models;
using CryptoFinAPI.Services;
using Microsoft.Extensions.Logging;

namespace CryptoFinAPI.UnitTests.Controllers
{
    public class PricesControllerTests
    {
        private readonly Mock<IPriceService> _priceServiceMock;
        private readonly Mock<ILogger<PricesController>> _loggerMock;
        private readonly PricesController _controller;

        public PricesControllerTests()
        {
            _priceServiceMock = new Mock<IPriceService>();
            _loggerMock = new Mock<ILogger<PricesController>>();
            _controller = new PricesController(_priceServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetPriceAtTimeAsync_ReturnsOk_WhenPriceIsFound()
        {
            // Arrange
            var request = new GetPriceAtTimeRequest { CurrencyPair = "btcusd", Limit = "1", Start = "1723150800", Step = "3600" };
            var pricePoint = new PricePoint { CurrencyPair = "btcusd", Close = "62445" };
            _priceServiceMock.Setup(s => s.GetPriceAtTimeAsync(request.CurrencyPair, request.Start, request.Step, request.Limit, It.IsAny<CancellationToken>()))
                             .ReturnsAsync(Convert.ToDecimal(pricePoint.Close));

            // Act
            var result = await _controller.GetPriceAtTimeAsync(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            string closeParam = string.Format("{0}", okResult.Value); 
            Assert.Equal(pricePoint.Close, closeParam);
        }

        [Fact]
        public async Task GetPriceAtTimeAsync_ReturnsUnprocessableEntity_WhenPriceIsEmpty()
        {
            // Arrange
            var request = new GetPriceAtTimeRequest { CurrencyPair = "btcusd", Limit = "1", Start = "1723150800", Step = "3600" };
            _priceServiceMock.Setup(s => s.GetPriceAtTimeAsync(request.CurrencyPair, request.Start, request.Step, request.Limit, It.IsAny<CancellationToken>())) 
                             .ReturnsAsync(0);

            // Act
            var result = await _controller.GetPriceAtTimeAsync(request, CancellationToken.None);

            // Assert
            var unprocessableEntityResult = Assert.IsType<UnprocessableEntityObjectResult>(result.Result);
            Assert.Equal("The price could not be calculated due to insufficient data.", unprocessableEntityResult.Value);
        }

        [Fact]
        public async Task GetPriceAtTimeAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new GetPriceAtTimeRequest { CurrencyPair = "btcusd", Limit = "1", Start = "1723150800", Step = "3600" };
            _priceServiceMock.Setup(s => s.GetPriceAtTimeAsync(request.CurrencyPair, request.Start, request.Step, request.Limit, It.IsAny<CancellationToken>()))
                             .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetPriceAtTimeAsync(request, CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An unexpected error occurred.", statusCodeResult.Value);
        }




    }
}
