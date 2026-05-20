using Xunit;
using IndependentWork24;

namespace IndependentWork24.Tests;

public class UnitTest1
{
    public class StructuralPatternsIntegrationTests
    {
        // Позитивні тести
        // Composite правильно рахує суму
        [Fact]
        public void Composite_CalculatesCorrectTotal_ForNestedBoxes()
        {
            var rootBox = new Box();
            var smallBox = new Box();
            
            smallBox.Add(new Product("Телефон", 1000m));
            smallBox.Add(new Product("Чохол", 50m));
            
            rootBox.Add(smallBox);
            rootBox.Add(new Product("Ноутбук", 3000m));

            decimal total = rootBox.CalculateTotal();

            Assert.Equal(4050m, total);
        }

        // Decorator застосовується до Composite
        [Fact]
        public void Decorator_AppliesDiscount_ToEntireBox()
        {
            var box = new Box();
            box.Add(new Product("Мишка", 100m));
            box.Add(new Product("Килимок", 50m));
            
            IOrderComponent discountedBox = new DiscountDecorator(box, 10m);

            decimal total = discountedBox.CalculateTotal();

            Assert.Equal(135m, total);
        }

        // Proxy кешує результат
        [Fact]
        public void Proxy_CachesResult_ImprovesPerformanceOnSecondCall()
        {
            var heavyBox = new Box();
            heavyBox.Add(new Product("Товар 1", 100m));
            
            var proxy = new SecureCachedOrderProxy(heavyBox, "Manager");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            decimal firstCall = proxy.CalculateTotal();
            watch.Stop();
            long firstCallTime = watch.ElapsedMilliseconds;

            watch.Restart();
            decimal secondCall = proxy.CalculateTotal();
            watch.Stop();
            long secondCallTime = watch.ElapsedMilliseconds;

            Assert.Equal(100m, firstCall);
            Assert.Equal(100m, secondCall);
            Assert.True(secondCallTime < firstCallTime); 
        }

        // Негативний/Граничний тест: Proxy блокує доступ без прав
        [Fact]
        public void Proxy_ThrowsUnauthorizedAccessException_ForInvalidRole()
        {
            var product = new Product("Секретний товар", 9999m);
            var proxy = new SecureCachedOrderProxy(product, "Guest");

            var exception = Assert.Throws<UnauthorizedAccessException>(() => { proxy.CalculateTotal(); });
            Assert.Equal("У вас немає прав для перегляду вартості цього замовлення.", exception.Message);
        }
    }
}
