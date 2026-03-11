namespace lab31v10.Interfaces
{
    public interface IInventoryRepository
    {
        int GetStock(string productName);
        void UpdateStock(string productName, int quantity);
    }
}
