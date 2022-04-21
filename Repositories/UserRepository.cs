using Dapper;
using Twitter_BE.Models;
using Twitter_BE.Utilities;

namespace Twitter_BE.Repositories;

public interface IUserRepository
{
    Task<User> Create(User Item);
    Task Update(User Item);
    Task<User> GetUser(string Email);
    Task<User> GetById(int Id);
}

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(IConfiguration config) : base(config)
    {

    }

    public async Task<User> Create(User Item)
    {
        var query = $@"INSERT INTO ""{TableNames.user}"" (name, email, password)
        VALUES (@Name, @Email,@Password) RETURNING *";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, Item);
    }

    public async Task<User> GetById(int Id)
    {
        var query = @$"SELECT * FROM ""{TableNames.user}"" WHERE id= @userid";
        using (var connection = NewConnection)
            return await connection.QuerySingleOrDefaultAsync<User>(query, new { userid = Id });
    }

    public async Task<User> GetUser(string Email)
    {
        var query = $@"SELECT * FROM ""{TableNames.user}"" WHERE email= @Email ";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new { Email });
    }

    public async Task Update(User Item)
    {
        var query = $@"UPDATE ""{TableNames.user}"" SET name = @Name WHERE id = @Id";
        using (var con = NewConnection)
            await con.ExecuteAsync(query, Item);
    }
}