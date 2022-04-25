using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Twitter_BE.Models;
using Twitter_BE.Utilities;

namespace Twitter_BE.Repositories;

public interface ICommentRepository
{
    Task<Comment> Create(Comment Item);
    Task<List<Comment>> GetAll(int PostId, int Limit, int PageNumber);
    Task Delete(int Id);
    Task<Comment> GetById(int CommentId);
}

public class CommentRepository : BaseRepository, ICommentRepository
{
    private readonly IMemoryCache _memoryCache;
    public CommentRepository(IConfiguration config, IMemoryCache memoryCache) : base(config)
    {
        _memoryCache = memoryCache;

    }


    public async Task<Comment> Create(Comment Item)
    {
        var query = $@"INSERT INTO {TableNames.comment} (text, user_id, post_id)
       VALUES(@Text, @UserId, @PostId) RETURNING *";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Comment>(query, Item);
    }

    public async Task Delete(int Id)
    {
        var query = $@"DELETE FROM {TableNames.comment} WHERE id = @Id";
        using (var con = NewConnection)
            await con.ExecuteAsync(query, new { Id });
    }

    public async Task<List<Comment>> GetAll(int PostId, int Limit, int PageNumber)
    {
        var commentCache = _memoryCache.Get<List<Comment>>(key: $"comment {Limit} : {PageNumber}");
        if (commentCache is null)
        {
            var query = $@"SELECT * FROM {TableNames.comment} WHERE post_id = @PostId ORDER BY created_at DESC LIMIT @Limit OFFSET @PageNumber";
            using (var con = NewConnection)
                commentCache = (await con.QueryAsync<Comment>(query, new { PostId, @PageNumber = (PageNumber - 1) * Limit, Limit })).AsList();
            _memoryCache.Set(key: "comment", commentCache, TimeSpan.FromMinutes(value: 1));

        }
        return commentCache;
    }

    public async Task<Comment> GetById(int CommentId)
    {
        var query = $@"SELECT * FROM {TableNames.comment} WHERE id = @CommentId";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Comment>(query, new { CommentId });
    }
}