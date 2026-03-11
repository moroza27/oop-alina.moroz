using lab31v10.Interfaces;

namespace lab31v10.Services
{
    public class InventoryService
    {
        private readonly IInventoryRepository repository;
        private readonly IAlertService alertService;

        public InventoryService(IInventoryRepository repository, IAlertService alertService)
        {
            this.repository = repository;
            this.alertService = alertService;
        }

        public bool RemoveProduct(string productName, int quantity)
        {
            int stock = repository.GetStock(productName);

            if (stock < quantity)
            {
                alertService.SendAlert("Not enough stock");
                return false;
            }

            repository.UpdateStock(productName, stock - quantity);

            if (stock - quantity < 5)
            {
                alertService.SendAlert("Low stock warning");
            }

            return true;
        }
    }
}
