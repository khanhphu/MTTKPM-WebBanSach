using System;
namespace WebBanSach.Models.Data.CommentsComposite
{
    public interface IComment
    {
        int Id { get; set; }
        string Content { get; set; }
        int UserId { get; set; }
        int BookId { get; set; }
        int Depth { get; set; }
        DateTime CreatedAt { get; set; }
        void Delete(BSDBContext context, string currentUser);
        void AddComment(BSDBContext context, int bookId, string content, string currentUser);
        void AddReply(IComment comment);

    }
}
