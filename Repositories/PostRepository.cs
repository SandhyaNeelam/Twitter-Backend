using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Twitter_BE.Models;
using Twitter_BE.Utilities;

namespace Twitter_BE.Repositories;

public interface IPostRepository
{
    Task<Post> Create(Post Item);
    Task Update(Post Item);
    Task Delete(int Id);
    Task<List<Post>> GetAll(int Limit, int PageNumber);
    Task<Post> GetById(int PostId);
    Task<List<Post>> GetByUserId(int User_Id);

}

public class PostRepository : BaseRepository, IPostRepository
{
    private readonly IMemoryCache _memoryCache;
    public PostRepository(IConfiguration config, IMemoryCache memoryCache) : base(config)
    {
        _memoryCache = memoryCache;

    }



    public async Task<Post> Create(Post Item)
    {
        var query = $@"INSERT INTO {TableNames.post} (title, user_id)
	    VALUES(@Title, @UserId) RETURNING *";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Post>(query, Item);
    }

    public async Task Delete(int Id)
    {
        var query = $@"DELETE FROM {TableNames.post} WHERE id = @Id";
        using (var con = NewConnection)
            await con.ExecuteAsync(query, new { Id });
    }


    public async Task<List<Post>> GetAll(int Limit, int PageNumber)
    {
        var postCache = _memoryCache.Get<List<Post>>(key: $"post {Limit} : {PageNumber}");
        if (postCache is null)
        {

            var query = $@"SELECT * FROM {TableNames.post} ORDER BY created_at DESC LIMIT @Limit OFFSET @PageNumber";
            using (var con = NewConnection)
                postCache = (await con.QueryAsync<Post>(query, new { @PageNumber = (PageNumber - 1) * Limit, Limit })).AsList();
            _memoryCache.Set(key: "post", postCache, TimeSpan.FromMinutes(value: 1));
        }
        return postCache;
    }


    public async Task<Post> GetById(int PostId)
    {
        var query = $@"SELECT * FROM {TableNames.post} WHERE id = @PostId";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Post>(query, new { PostId });
    }

    public async Task<List<Post>> GetByUserId(int User_Id)
    {
        var query = $@"SELECT * FROM {TableNames.post} WHERE user_id = @UserId";
        using (var con = NewConnection)
            return (await con.QueryAsync<Post>(query, new { UserId = User_Id })).AsList();
    }

    public async Task Update(Post Item)
    {
        var query = $@"Update {TableNames.post} SET title =@Title, updated_at = now()   WHERE id = @Id";
        using (var con = NewConnection)
            await con.ExecuteAsync(query, Item);
    }
}