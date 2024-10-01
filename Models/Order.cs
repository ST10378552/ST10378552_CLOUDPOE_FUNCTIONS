using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCRetailApp_Functions.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "Orders";
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
