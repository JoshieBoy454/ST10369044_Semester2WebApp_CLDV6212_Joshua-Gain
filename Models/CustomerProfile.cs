using Azure;
using Azure.Data.Tables;

namespace ST10369044Semerster2WebApp.Models
{
    public class CustomerProfile:ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? ConstractFilePath { get; set;}
        public string? ImageFilePath { get; set; }

        public CustomerProfile()
        {
            PartitionKey = "CustomerProfile";
            RowKey = Guid.NewGuid().ToString();

            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            ConstractFilePath = string.Empty;
            ImageFilePath = string.Empty;


        }
    }
}
