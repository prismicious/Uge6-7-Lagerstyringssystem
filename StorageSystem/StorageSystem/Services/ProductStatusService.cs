using Microsoft.EntityFrameworkCore;
using StorageSystem;
using StorageSystem.Exceptions;
using StorageSystem.Helpers;
using StorageSystem.Models;

namespace StorageSystem.Services
{
    public static class ProductStatusService
    {

        // Get the product status for a product
        public static ProductStatus? Get(Product product)
        {
            return GetProductStatus(product.ID);
        }

        // Get the product status for a product
        public static ProductStatus? GetProductStatus(int productID)
        {
            using var ctx = new StorageContext();
            return ctx.ProductStatuses
                .Where(ps => ps.ProductID == productID)
                .SingleOrDefault();
        }

        // Get all product statuses for a warehouse
        public static List<ProductStatus>? Get()
        {
            using var ctx = new StorageContext();
            return [.. ctx.ProductStatuses];
        }

        // Create new product status for a product
        public static ProductStatus Create(Product product, int initialQuantity)
        {
            if (initialQuantity < 0)
                throw new ArgumentOutOfRangeException("Quantity can not be less than zero");

            if (null != Get(product))
                throw new InvalidOperationException("Product status already exists");

            using var ctx = new StorageContext();
            var ps = new ProductStatus { ProductID = product.ID, Quantity = initialQuantity };
            ctx.ProductStatuses.Add(ps);
            ctx.SaveChanges();
            return ps;
        }

        // Updates a product status for a product
        public static bool Update(ProductStatus ps)
        {
            using var ctx = new StorageContext();

            if (ps.ID == 0 || !ctx.ProductStatuses.Contains(ps))
                return false;

            ctx.ProductStatuses.Update(ps);
            return 0 != ctx.SaveChanges();
        }

        // Reserves a quantity of a product
        public static bool Reserve(int productID, int quantity)
        {
            using var ctx = new StorageContext();
         
            // Find the ProductStatus object associated with the product
            ProductStatus? productStatus = ctx.ProductStatuses
                .Where(ps => ps.ProductID == productID)
                .SingleOrDefault();
            if (null == productStatus)
                return false;

            // Ensure there is enough stock to reserve
            if (productStatus.Quantity < quantity)
                return false;
            productStatus.Quantity -= quantity;
            productStatus.Reserved += quantity;

            return Update(productStatus);
        }

        // Undoes a reservation of a product
        public static bool UndoReserve(int productID, int quantity)
        {
            using var ctx = new StorageContext();

            // Find the ProductStatus object associated with the product
            ProductStatus? productStatus = ctx.ProductStatuses
                .Where(ps => ps.ProductID == productID)
                .SingleOrDefault();
            if (null == productStatus)
                return false;
         
            // Ensure there is enough stock reserved to undo
            if (productStatus.Reserved < quantity)
                return false;
            productStatus.Quantity += quantity;
            productStatus.Reserved -= quantity;
            return Update(productStatus);
        }

        // Get the total quantity of all products in stock
        public static int GetTotalQuantity()
        {
            using var ctx = new StorageContext();
            return ctx.ProductStatuses.Sum(ps => ps.Quantity);
        }

        // Update a products quantity.
        // Returns true if the quantity changed the stock.
        public static bool Update(Product p, int quantity)
        {
            using var ctx = new StorageContext();

            // Find the ProductStatus object associated with the warehouse and product
            ProductStatus? productStatus = ctx.ProductStatuses
                .Where(ps => ps.ProductID == p.ID)
                .SingleOrDefault();

            if (null == productStatus)
            {
                // Make sure we don't create a product status with negative quantity
                if (quantity < 0)
                    return false;

                Create(p, quantity);
                return true;
            }
            else
            {
                // Ensure there is enough stock to
                if (quantity < 0 && quantity < -productStatus.Quantity)
                    return false;

                productStatus.Quantity += quantity;
                return Update(productStatus);
            }
        }

        public static void OrderMoreStock(Product product, int quantity)
        {
            Update(product, quantity);
        }

        public static int GetTotalReserved()
        {
            using var ctx = new StorageContext();
            return ctx.ProductStatuses.Sum(ps => ps.Reserved);
        }
    }
}
