using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.Data.CommentsComposite
{
    public class SingleComment : IComment
    {
      
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int Depth { get; set; }
        public DateTime CreatedAt { get; set; }
       
        public void AddComment(BSDBContext context, int bookId, string content, string currentUser)
        {
            if (string.IsNullOrEmpty(currentUser))
            {
                throw new UnauthorizedAccessException("Bạn cần đăng nhập để bình luận!");

            }
            var userId = context.KhachHangs.First(s => s.TenKH == currentUser).MaKH;
            var newComment = new Comments
            {
                Content = content,
                UserId = userId,
                BookId = bookId,
                CreatedAt = DateTime.Now,
                Depth = this.Depth + 1,
                ParentId=this.Id, //binh luan nut la ko co binh luan cha
            };
            context.Comments.Add(newComment);

        }

        public void Delete(BSDBContext context, string currentUser)
        {
            var comment = context.Comments.FirstOrDefault(s => s.Id==this.Id);
            if (string.IsNullOrEmpty(currentUser) ||
                context.KhachHangs.First(s => s.MaKH==comment.UserId).TaiKhoan !=currentUser) // lay ra ten KH co maKH trung vs maKH cua cmt
            {
                throw new UnauthorizedAccessException("Ban chi co the xoa binh luan cua chinh minh!");
            }
            context.Comments.Remove(comment);   
        }


      

     

     

      public  void AddReply(IComment comment)
        {
            throw new NotSupportedException("Khong chua phan hoi truc tiep");

        }
    }
}