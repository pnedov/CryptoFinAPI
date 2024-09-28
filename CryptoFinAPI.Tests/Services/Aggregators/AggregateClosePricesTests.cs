using CryptoFinAPI.Models;
using CryptoFinAPI.Services.Aggregators;

namespace PriceAggregator.Tests.Services.Aggregators
{
    public class AverageClosePriceAggregatorTests
    {
        private readonly AggregateClosePrices _aggregator;

        public AverageClosePriceAggregatorTests()
        {
            _aggregator = new AggregateClosePrices();
        }

        [Fact]
        public async Task AggregateAsync_CalculatesCorrectAverage()
        {
            // Arrange
            var prices = new List<PricePoint>
            {
                new PricePoint { Close = "1.0" },
                new PricePoint { Close = "2.0" },
                new PricePoint { Close = "3.0" }
            };
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _aggregator.AggregateAsync(prices, cancellationToken);

            // Assert
            Assert.Equal(2.0m, Convert.ToDecimal(result));
        }

        [Fact]
        public async Task AggregateAsync_HandlesEmptyList()
        {
            // Arrange
            IList<PricePoint> prices = [];
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _aggregator.AggregateAsync(prices, cancellationToken));
        }

        [Fact]
        public async Task AggregateAsync_HandlesSinglePricePoint()
        {
            // Arrange
            var prices = new List<PricePoint>
            {
                new PricePoint { Close = "1.0" }
            };
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _aggregator.AggregateAsync(prices, cancellationToken);

            // Assert
            Assert.Equal(1.0m, Convert.ToDecimal(result));
        }

        [Fact]
        public async Task AggregateAsync_HandlesCancellationToken()
        {
            // Arrange
            var prices = new List<PricePoint>
            {
                new() { Close = "1.0" },
                new() { Close = "2.0" },
                new() { Close = "3.0" }
            };
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => _aggregator.AggregateAsync(prices, cancellationToken));
        }
    }
}
