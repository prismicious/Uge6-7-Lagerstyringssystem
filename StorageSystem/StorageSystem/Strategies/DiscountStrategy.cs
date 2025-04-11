using StorageSystem.Models;

namespace StorageSystem.Strategies
{
    public interface IDiscountStrategy
    {
        abstract decimal Calculate(Product product, int quantity);
    }

    public class DefaultDiscountStrategy : IDiscountStrategy
    {
        public decimal Calculate(Product product, int quantity)
        {
            return 0.0m;
        }
    }


    public class FlatDiscountStrategy(decimal discount) : IDiscountStrategy
    {
        public decimal Calculate(Product product, int quantity)
        {
            return discount;
        }
    }

    public class ValueStrategy(decimal targetValue, decimal discount) : IDiscountStrategy
    {
        public decimal Calculate(Product product, int quantity)
        {
            bool hitTarget = (quantity * product.Price) >= targetValue;
            return hitTarget ? discount : 0.0m;
        }
    }

    public class BulkStrategy(int targetQuantity, decimal discount) : IDiscountStrategy
    {
        public decimal Calculate(Product product, int quantity)
        {
            bool hitTarget = quantity >= targetQuantity;
            return hitTarget ? discount : 0.0m;
        }
    }
}
