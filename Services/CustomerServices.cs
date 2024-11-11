using Azure.Data.Tables;
using ST10369044Semerster2WebApp.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ST10369044Semerster2WebApp.Services
{
    public class CustomerServices
    {
        private readonly IConfiguration _configuration;

        public CustomerServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InsertCustomerAsync(CustomerProfile profile)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");
            var query = @"INSERT INTO CustomerProfile (FirstName, LastName, Email, PhoneNumber)
                VALUES (@FirstName, @LastName, @Email, @PhoneNumber)";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", profile.FirstName);
                command.Parameters.AddWithValue("@LastName", profile.LastName);
                command.Parameters.AddWithValue("@Email", profile.Email);
                command.Parameters.AddWithValue("@PhoneNumber", profile.PhoneNumber);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}