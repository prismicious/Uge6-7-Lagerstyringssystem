using Microsoft.EntityFrameworkCore;
using StorageSystem;
using StorageSystem.Models;

namespace StorageSystem.Services
{
    public static class WarehouseService
    {
        public static Warehouse Create(string location)
        {
            using var ctx = new StorageContext();

            // Create the warehouse
            var warehouse = new Warehouse { Location = location };
            ctx.Warehouses.Add(warehouse);
            ctx.SaveChanges();

            // Create customer for the new warehouse
            var wh_customer = new Customer { Address = location, Name = $"warehouse:{warehouse.ID}", Email = "internal@warehouse.storage", Type = 2 };
            ctx.Customers.Add(wh_customer);
            ctx.SaveChanges();

            return warehouse;
        }

        public static bool Remove(Warehouse wh)
        {
            using var ctx = new StorageContext();
            ctx.Warehouses.Remove(wh);
            return 1 == ctx.SaveChanges();
        }

        // Get the customer associated with the warehouse
        public static Customer GetAssociatedCustomer(Warehouse wh)
        {
            using var ctx = new StorageContext();
            return ctx.Customers
                .Where(c => c.Name == $"warehouse:{wh.ID}")
                .Single();
        }

        // Get the product status for a product
        public static ProductStatus? GetProductStatus(Warehouse wh, Product product)
        {
            return GetProductStatus(wh, product.ID);
        }

        // Get the product status for a product
        public static ProductStatus? GetProductStatus(Warehouse wh, int productID)
        {
            using var ctx = new StorageContext();
            return ctx.ProductStatuses
                .Include(ps => ps.Warehouse)
                .Where(ps => ps.ProductID == productID && ps.Warehouse == wh)
                .SingleOrDefault();
        }

        // Create new product status for a product
        public static ProductStatus? CreateProductStatus(Warehouse wh, Product p, int initialQuantity = 0)
        {
            using var ctx = new StorageContext();
            var ps = new ProductStatus { ProductID = p.ID, WarehouseID = wh.ID, Quantity = initialQuantity };
            ctx.ProductStatuses.Add(ps);
            if (1 != ctx.SaveChanges())
                return null;
            else
                return ps;
        }

        // Updates a product status for a product
        public static bool UpdateProductStatus(ProductStatus ps)
        {
            using var ctx = new StorageContext();

            if (ps.ID == 0 || !ctx.ProductStatuses.Contains(ps))
                return false;

            ctx.ProductStatuses.Update(ps);
            return 1 == ctx.SaveChanges();
        }

        // Update a products quantity in a warehouse.
        // Returns true if the quantity changed the warehouse stock.
        public static bool UpdateProductQuantity(Warehouse wh, Product p, int quantity)
        {
            using var ctx = new StorageContext();

            // Find the ProductStatus object associated with the warehouse and product
            ProductStatus? productStatus = ctx.ProductStatuses
                .Include(ps => ps.Warehouse)
                .Where(ps => ps.ProductID == p.ID && ps.Warehouse.ID == wh.ID)
                .SingleOrDefault();

            if (null == productStatus)
            {
                // Make sure we don't create a product status with negative quantity
                if (quantity < 0)
                    return false;

                productStatus = CreateProductStatus(wh, p, quantity);
                return productStatus != null;
            }
            else
            {
                // Ensure there is enough stock to
                if (quantity < 0 && quantity < -productStatus.Quantity)
                    return false;

                productStatus.Quantity += quantity;
                return UpdateProductStatus(productStatus);
            }
        }

        // Moves product from one warehouse to another
        public static bool MoveProduct(Warehouse from, Warehouse to, Product p, int quantity)
        {
            ProductStatus? statusFrom = GetProductStatus(from, p);

            // If there is no product status, there is no product at location
            if (statusFrom == null)
                return false;

            // Make sure there is enough product to move
            if (statusFrom.Quantity < quantity)
                return false;

            // Create order for the move
            var customerTo = GetAssociatedCustomer(to);
            var ol = OrderListService.Create(customerTo);
            var order = OrderService.Create(ol, p, quantity, 0, 0); // special price for friends

            // TODO create the transaction
            //var transaction = TransactionService.Create(ol);

            return true;
        }
    }
}
