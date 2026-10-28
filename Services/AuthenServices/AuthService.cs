using Dapper;
using Microsoft.Data.SqlClient;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;



//DEPRICATED - NOT IN USE
namespace Accounting_V4.Services.AuthenServices
{
    public class AuthService
    {
        private readonly string _connectionString;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;


        public AuthService(IConfiguration config, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _connectionString = config.GetConnectionString("DefaultConnectionString");
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            // Add null/empty validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                SELECT COUNT(*) 
                FROM UserAccount 
                WHERE Username = @Username AND Password = @Password AND IsActive = 1";

                var count = await connection.ExecuteScalarAsync<int>(sql, new
                {
                    Username = username,
                    Password = password
                });

                return count > 0;
            }
            catch (Exception ex)
            {
                // Log the exception (you should inject ILogger<AuthService>)
                // _logger.LogError(ex, "Error validating user {Username}", authModel.Username);
                return false;
            }
        }



        public async Task LogoutAsync()
        {
            try
            {
                // Remove the authentication token from local storage
                await _localStorage.RemoveItemAsync("authToken");
          
            }
            catch (Exception ex)
            {
                // Handle any errors during logout
                Console.WriteLine($"Error during logout: {ex.Message}");
            }
        }
    }
}
