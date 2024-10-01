using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCRetailApp_Functions.Models
{
    public class CustomerProfile : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public CustomerProfile()
        {
            PartitionKey = Guid.NewGuid().ToString();
            RowKey = Guid.NewGuid().ToString();
            ETag = new ETag("*");
            FirstName = string.Empty;
            Surname = string.Empty;
            Email = string.Empty;
        }
    }
}
