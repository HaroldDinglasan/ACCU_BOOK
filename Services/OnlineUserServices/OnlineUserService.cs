using System.Collections.Concurrent;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.OnlineUserServices;

public class OnlineUserService
{
    private readonly string connectionString;

    public OnlineUserService(IConfiguration config)
    {
       
        connectionString = config.GetConnectionString("DefaultConnectionString");
    }

    public async Task UserConnected(string userId)
    {
        using var con = new SqlConnection(connectionString);

        await con.ExecuteAsync(@"
        IF EXISTS (SELECT 1 FROM OnlineUsers WHERE UserId=@userId)
            UPDATE OnlineUsers
            SET ConnectionCount = ConnectionCount + 1,
                LastSeen = GETDATE()
        ELSE
            INSERT INTO OnlineUsers(UserId, LastSeen, ConnectionCount)
            VALUES(@userId, GETDATE(), 1)",
        new
        {
            userId
        });
    }

    public async Task UserDisconnected(string userId)
    {
        using var con = new SqlConnection(connectionString);

        await con.ExecuteAsync(@"
        UPDATE OnlineUsers
        SET ConnectionCount = ConnectionCount - 1,
            LastSeen = GETDATE()
        WHERE UserId = @userId;

        DELETE FROM OnlineUsers WHERE ConnectionCount <= 0",
        new
        {
            userId
        });
    }

    public async Task<List<string>> GetOnlineUsers()
    {
        using var con = new SqlConnection(connectionString);

        return (await con.QueryAsync<string>(
            "SELECT UserId FROM OnlineUsers")).ToList();
    }
}
