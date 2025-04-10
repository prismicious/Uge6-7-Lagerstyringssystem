using Microsoft.EntityFrameworkCore;
using StorageSystem;
using StorageSystem.Exceptions;
using StorageSystem.Helpers;
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
            var wh_customer = new Customer { Address = location, Name = $"warehouse:{warehouse.ID}", Email = "internal@warehouse.storage", Type = CustomerType.Warehouse };
            ctx.Customers.Add(wh_customer);
            ctx.SaveChanges();

            return warehouse;
        }

        public static bool Remove(Warehouse wh)
        {
            using var ctx = new StorageContext();
            ctx.Warehouses.Remove(wh);
            return 0 != ctx.SaveChanges();
        }

        // Get the customer associated with the warehouse
        public static Customer GetAssociatedCustomer(Warehouse wh)
        {
            using var ctx = new StorageContext();
            return ctx.Customers
                .Where(c => c.Type == CustomerType.Warehouse && c.Name == $"warehouse:{wh.ID}")
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
        // Get all product statuses for a warehouse
        public static List<ProductStatus>? GetAllWarehouseProductStatus(Warehouse wh)
        {
            using var ctx = new StorageContext();
            return ctx.ProductStatuses
                .Where(ps => ps.WarehouseID == wh.ID).ToList();
        }

        // Create new product status for a product
        public static ProductStatus? CreateProductStatus(Warehouse wh, Product p, int initialQuantity = 0)
        {
            using var ctx = new StorageContext();
            var ps = new ProductStatus { ProductID = p.ID, WarehouseID = wh.ID, Quantity = initialQuantity };
            ctx.ProductStatuses.Add(ps);
            if (0 == ctx.SaveChanges())
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
            return 0 != ctx.SaveChanges();
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
        public static Transaction MoveProduct(Warehouse from, Warehouse to, OrderList orderList)
        {
            if (orderList.Orders.Count == 0)
                throw new ArgumentException("Order list is empty");

            // Verify the customer/warehouse
            var customerTo = GetAssociatedCustomer(to);
            if (orderList.CustomerID != customerTo.ID)
                throw new CustomerIDMismatchException($"Expected {customerTo.ID}, got {orderList.CustomerID}");

            // Make sure there is enough product to move
            foreach (Order o in orderList.Orders)
            {
                var p = ProductService.Get(o.ProductID);
                if (p == null)
                    throw new InvalidDataException("Unknown product id");

                o.Product = p;
                ProductStatus? statusFrom = GetProductStatus(from, o.Product);
                if (statusFrom == null || statusFrom.Quantity < o.Quantity)
                    throw new InsufficientStockException($"Product {o.ProductID}: {o.Product.Name}");
            }

            // Adjust product quantity in each warehouse
            foreach (Order o in orderList.Orders)
            {
                WarehouseService.UpdateProductQuantity(from, o.Product, -o.Quantity);
                WarehouseService.UpdateProductQuantity(to,   o.Product, +o.Quantity);
            }

            // Create the transaction
            return TransactionService.CreateWarehouseTransfer(orderList, from);
        }
    }
}
