using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data.CommentsComposite;

namespace WebBanSach.Models.Data.CommentsComposite
{

    public class CompositeComments : IComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int Depth { get; set; }
        public DateTime CreatedAt { get; set; }


        //rieng cua compositecmt
        public List<IComment> _replies = new List<IComment>();

        public void AddComment(BSDBContext context, int bookId, string content, string currentUser)
        {
            if (string.IsNullOrEmpty(currentUser))
                throw new UnauthorizedAccessException("Bạn cần đăng nhập để bình luận.");

            var userId = context.KhachHangs.First(kh => kh.TenKH == currentUser).MaKH;


            var newComment = new Comments
            {
                Content = content,
                UserId = userId,
                BookId = bookId,
                CreatedAt = DateTime.Now,
                Depth = this.Depth + 1,
                ParentId = this.Id
            };
            context.Comments.Add(newComment);

        }

        public void AddReply(IComment comment)
        {
            _replies.Add(comment);
        }

        public void Delete(BSDBContext context, string currentUser)
        {
            var comment = context.Comments.FirstOrDefault(c => c.Id == Id);
            if (comment == null || context.KhachHangs.First(kh => kh.MaKH == comment.UserId).TaiKhoan != currentUser)
                throw new UnauthorizedAccessException("Bạn chỉ có thể xóa bình luận của chính mình.");

            foreach (var reply in _replies)
            {
                reply.Delete(context, context.KhachHangs.First(kh => kh.MaKH == reply.UserId).TenKH);
            }
            context.Comments.Remove(comment);
        }


    }
}