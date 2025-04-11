﻿using Microsoft.EntityFrameworkCore;
using StorageSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Services
{
    public static class OrderListService
    {
        public static List<OrderList> Get()
        {
            using (var ctx = new StorageContext())
            {
                return ctx.OrderLists
                    .Include(ol => ol.Orders)
                    .ToList();
            }
        }

        public static OrderList Get(int ID)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.OrderLists
                    .Where(ol => ol.ID == ID)
                    .Include(ol => ol.Orders)
                    .SingleOrDefault();
            }
        }

        // Creates an order list for a customer
        public static OrderList Create(int customerID)
        {
            using (var ctx = new StorageContext())
            {
                var orderList = new OrderList { CustomerID = customerID/*, Customer = customer*/ };
                ctx.OrderLists.Add(orderList);
                ctx.SaveChanges();
                return orderList;
            }
        }

        // Create an orderlist for moving product between warehouses
        public static OrderList CreateForWarehouseMove(Warehouse destination)
        {
            using (var ctx = new StorageContext())
            {
                // Get the warehouse customer
                var wh_cust = WarehouseService.GetAssociatedCustomer(destination);

                // Create the order list
                var orderList = new OrderList { CustomerID = wh_cust.ID };
                ctx.OrderLists.Add(orderList);
                ctx.SaveChanges();
                return orderList;
            }
        }

        public static OrderList AddOrder(OrderList orderList, Order order)
        {
            using (var ctx = new StorageContext())
            {
                orderList.Orders.Add(order);
                ctx.OrderLists.Update(orderList);
                ctx.SaveChanges();
                return orderList;
            }
        }


        public static OrderList AddOrders(OrderList orderList, List<Order> orders)
        {
            using (var ctx = new StorageContext())
            {
                orderList.Orders.Concat(orders);
                ctx.OrderLists.Update(orderList);
                ctx.SaveChanges();
                return orderList;
            }
        }

        // Removes an order from the order list and the database. Returns the updated order list.
        public static OrderList RemoveOrder(OrderList orderList, Order order)
        {
            using (var ctx = new StorageContext())
            {
                orderList.Orders.Remove(order);
                ctx.Orders.Remove(order);
                ctx.OrderLists.Update(orderList);
                ctx.SaveChanges();
                return orderList;
            }
        }

        // Calculate the total price of the order list
        public static decimal CalculateTotalPrice(OrderList orderList)
        {
            decimal totalPrice = 0;
            foreach (var order in orderList.Orders)
            {
                totalPrice += order.Price * order.Quantity * (1 - order.Discount);
            }
            return totalPrice;
        }
    }
}
