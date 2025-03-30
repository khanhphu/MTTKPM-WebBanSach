namespace WebBanSach.Models.Data.CommentsComposite
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    [Table("Comments")]
    public partial class Comments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comments()
        {
            Replies = new HashSet<Comments>();
        }

        [Key]
        [Display(Name = "Mã bình luận")]
        public int Id { get; set; }

        [StringLength(200)]
        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Không được để trống nội dung bình luận")]
        public string Content { get; set; }

        [Display(Name = "Mã khách hàng")]
        public int UserId { get; set; }

        [Display(Name = "Mã sách")]
        public int BookId { get; set; }

        [Display(Name = "Độ sâu")]
        public int Depth { get; set; }

        [Column(TypeName = "datetime")]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Mã bình luận cha")]
        public int? ParentId { get; set; }

        [ForeignKey("UserId")]
        [Display(Name = "Khách hàng")]
        public virtual KhachHang User { get; set; }

        [ForeignKey("BookId")]
        [Display(Name = "Sách")]
        public virtual Sach Book { get; set; }

        [ForeignKey("ParentId")]
        [Display(Name = "Bình luận cha")]
        public virtual Comments Parent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comments> Replies { get; set; }
    }
}