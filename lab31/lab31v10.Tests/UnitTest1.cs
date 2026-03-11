using Xunit;
using Moq;
using lab31v10.Interfaces;
using lab31v10.Services;

namespace lab31v10.Tests
{
    public class InventoryServiceTests
    {
        [Fact]
        public void RemoveProduct_ShouldReturnTrue_WhenStockEnough()
        {
            var repoMock = new Mock<IInventoryRepository>();
            var alertMock = new Mock<IAlertService>();

            repoMock.Setup(r => r.GetStock("Laptop")).Returns(10);

            var service = new InventoryService(repoMock.Object, alertMock.Object);

            var result = service.RemoveProduct("Laptop", 3);

            Assert.True(result);
        }

        [Fact]
        public void RemoveProduct_ShouldUpdateStock()
        {
            var repoMock = new Mock<IInventoryRepository>();
            var alertMock = new Mock<IAlertService>();

            repoMock.Setup(r => r.GetStock("Laptop")).Returns(10);

            var service = new InventoryService(repoMock.Object, alertMock.Object);

            service.RemoveProduct("Laptop", 3);

            repoMock.Verify(r => r.UpdateStock("Laptop", 7), Times.Once);
        }

        [Fact]
        public void RemoveProduct_ShouldSendAlert_WhenStockNotEnough()
        {
            var repoMock = new Mock<IInventoryRepository>();
            var alertMock = new Mock<IAlertService>();

            repoMock.Setup(r => r.GetStock("Laptop")).Returns(2);

            var service = new InventoryService(repoMock.Object, alertMock.Object);

            var result = service.RemoveProduct("Laptop", 5);

            Assert.False(result);

            alertMock.Verify(a => a.SendAlert("Not enough stock"), Times.Once);
        }

        [Fact]
        public void RemoveProduct_ShouldNotUpdateStock_WhenStockNotEnough()
        {
            var repoMock = new Mock<IInventoryRepository>();
            var alertMock = new Mock<IAlertService>();

            repoMock.Setup(r => r.GetStock("Laptop")).Returns(1);

            var service = new InventoryService(repoMock.Object, alertMock.Object);

            service.RemoveProduct("Laptop", 5);

            repoMock.Verify(r => r.UpdateStock(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void RemoveProduct_ShouldSendLowStockAlert()
        {
            var repoMock = new Mock<IInventoryRepository>();
            var alertMock = new Mock<IAlertService>();

            repoMock.Setup(r => r.GetStock("Laptop")).Returns(6);

            var service = new InventoryService(repoMock.Object, alertMock.Object);

            service.RemoveProduct("Laptop", 2);

            alertMock.Verify(a => a.SendAlert("Low stock warning"), Times.Once);
        }

        [Fact]
        public void RemoveProduct_ShouldCallGetStock()
        {
            var repoMock = new Mock<IInventoryRepository>();
            var alertMock = new Mock<IAlertService>();

            repoMock.Setup(r => r.GetStock("Laptop")).Returns(10);

            var service = new InventoryService(repoMock.Object, alertMock.Object);

            service.RemoveProduct("Laptop", 3);

            repoMock.Verify(r => r.GetStock("Laptop"), Times.Once);
        }

        [Fact]
        public void RemoveProduct_ShouldReturnFalse_WhenStockTooSmall()
        {
            var repoMock = new Mock<IInventoryRepository>();
            var alertMock = new Mock<IAlertService>();

            repoMock.Setup(r => r.GetStock("Laptop")).Returns(0);

            var service = new InventoryService(repoMock.Object, alertMock.Object);

            var result = service.RemoveProduct("Laptop", 1);

            Assert.False(result);
        }

        [Fact]
        public void RemoveProduct_ShouldReturnTrue_WhenStockIsLarge()
        {
            var repoMock = new Mock<IInventoryRepository>();
            var alertMock = new Mock<IAlertService>();

            repoMock.Setup(r => r.GetStock("Laptop")).Returns(20);

            var service = new InventoryService(repoMock.Object, alertMock.Object);

            var result = service.RemoveProduct("Laptop", 5);

            Assert.True(result);
        }
    }
}
