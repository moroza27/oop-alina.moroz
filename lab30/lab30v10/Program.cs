namespace lab30v10
{
    public class DiscountCalculator
    {
        public decimal CalculateDiscount(decimal price, decimal discountPercent)
        {
            if (price < 0)
                throw new ArgumentException("Price cannot be negative");

            if (discountPercent < 0 || discountPercent > 100)
                throw new ArgumentException("Discount percent must be between 0 and 100");

            decimal discountAmount = price * discountPercent / 100;
            return price - discountAmount;
        }

        public decimal ApplyCoupon(decimal price, string couponCode)
        {
            if (price < 0)
                throw new ArgumentException("Price cannot be negative");

            if (string.IsNullOrEmpty(couponCode))
                return price;

            switch (couponCode)
            {
                case "SAVE10":
                    return price * 0.9m;

                case "SAVE20":
                    return price * 0.8m;

                case "HALF":
                    return price * 0.5m;

                default:
                    return price;
            }
        }
        
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            
        }
    }
}