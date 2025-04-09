﻿using StorageSystem.Helpers;

namespace StorageSystem.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }

        public int OrderListID { get; set; }
        public OrderList OrderList { get; set; }

        // Optional receipt reference navigation
        public Receipt? Receipt { get; set; }

        // Optional foreign key and reference navigation
        public int? WarehouseID { get; set; }
        //public Warehouse? Warehouse { get; set; }
    }
}