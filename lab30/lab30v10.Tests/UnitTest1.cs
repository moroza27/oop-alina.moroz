using Xunit;
using lab30v10;

namespace lab30v10.Tests
{
    public class DiscountCalculatorTests
    {
        private readonly DiscountCalculator calculator = new DiscountCalculator();

        [Fact]
        public void CalculateDiscount_ValidDiscount_ReturnsCorrectPrice()
        {
            var result = calculator.CalculateDiscount(100, 10);

            Assert.Equal(90, result);
        }

        [Theory]
        [InlineData(100, 20, 80)]
        [InlineData(200, 50, 100)]
        [InlineData(100, 0, 100)]
        public void CalculateDiscount_MultipleCases(decimal price, decimal percent, decimal expected)
        {
            var result = calculator.CalculateDiscount(price, percent);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateDiscount_NegativePrice_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                calculator.CalculateDiscount(-100, 10));
        }

        [Fact]
        public void CalculateDiscount_InvalidPercent_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                calculator.CalculateDiscount(100, 200));
        }

        // ----------- ApplyCoupon -----------

        [Fact]
        public void ApplyCoupon_Save10_ReturnsDiscountedPrice()
        {
            var result = calculator.ApplyCoupon(100, "SAVE10");

            Assert.Equal(90, result);
        }

        [Fact]
        public void ApplyCoupon_Save20_ReturnsDiscountedPrice()
        {
            var result = calculator.ApplyCoupon(100, "SAVE20");

            Assert.Equal(80, result);
        }

        [Fact]
        public void ApplyCoupon_Half_ReturnsHalfPrice()
        {
            var result = calculator.ApplyCoupon(100, "HALF");

            Assert.Equal(50, result);
        }

        [Fact]
        public void ApplyCoupon_InvalidCoupon_ReturnsOriginalPrice()
        {
            var result = calculator.ApplyCoupon(100, "UNKNOWN");

            Assert.Equal(100, result);
        }

        [Fact]
        public void ApplyCoupon_EmptyCoupon_ReturnsOriginalPrice()
        {
            var result = calculator.ApplyCoupon(100, "");

            Assert.Equal(100, result);
        }

        [Fact]
        public void ApplyCoupon_NegativePrice_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                calculator.ApplyCoupon(-100, "SAVE10"));
        }
    }
}