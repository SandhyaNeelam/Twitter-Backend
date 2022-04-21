using Dapper;
using Twitter_BE.Models;
using Twitter_BE.Utilities;

namespace Twitter_BE.Repositories;

public interface ICommentRepository
{
    Task<Comment> Create(Comment Item);
    Task<List<Comment>> GetAll(int PostId);
    Task Delete(int Id);
    Task<Comment> GetById(int CommentId);
}

public class CommentRepository : BaseRepository, ICommentRepository
{
    public CommentRepository(IConfiguration config) : base(config)
    {

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

    public async Task<List<Comment>> GetAll(int PostId)
    {
        var query = $@"SELECT * FROM {TableNames.comment} WHERE post_id = @PostId ORDER BY created_at DESC";
        using (var con = NewConnection)
            return (await con.QueryAsync<Comment>(query, new { PostId })).AsList();
    }

    public async Task<Comment> GetById(int CommentId)
    {
        var query = $@"SELECT * FROM {TableNames.comment} WHERE id = @CommentId";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Comment>(query, new { CommentId });
    }
}