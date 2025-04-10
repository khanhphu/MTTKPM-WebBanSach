namespace WebBanSach.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using WebBanSach.Models.Data.CommentsComposite;

    [Table("Sach")]
    public partial class Sach
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sach()
        {
            ChiTietDDHs = new HashSet<ChiTietDDH>();
        }

        [Key]
        [Display(Name = "Mã sách")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaSach { get; set; }

        [Display(Name = "Mã loại")]
        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        public int MaLoai { get; set; }

        [Display(Name = "Mã NXB")]
        [Required(ErrorMessage = "Vui lòng chọn nhà xuất bản")]
        public int MaNXB { get; set; }

        [Display(Name = "Mã tác giả")]
        [Required(ErrorMessage = "Vui lòng chọn tác giả")]
        public int MaTG { get; set; }

        [StringLength(250)]
        [Display(Name = "Tên sách")]
        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        public string TenSach { get; set; }

        [Column(TypeName = "money")]
        [Display(Name = "Giá bán")]
        public decimal? GiaBan { get; set; }

        // Thêm thuộc tính lưu giá gốc, không mapping vào DB
        [NotMapped]
        public decimal? GiaBanGoc { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string Mota { get; set; }

        [StringLength(50)]
        [Display(Name = "Người dịch")]
        public string NguoiDich { get; set; }

        [StringLength(50)]
        [Display(Name = "Ảnh bìa")]
        public string AnhBia { get; set; }

        [Column(TypeName = "smalldatetime")]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }

        [Display(Name = "Số lượng tồn")]
        public int? SoLuongTon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDDH> ChiTietDDHs { get; set; }
        //them vao cho phan Comment
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comments> Comments { get; set; }

        public virtual NhaXuatBan NhaXuatBan { get; set; }

        public virtual TheLoai TheLoai { get; set; }

        public virtual TacGia TacGia { get; set; }

        // Interface Builder
        public interface ISachBuilder
        {
            ISachBuilder SetMaLoai(int maLoai);
            ISachBuilder SetMaNXB(int maNXB);
            ISachBuilder SetMaTG(int maTG);
            ISachBuilder SetTenSach(string tenSach);
            ISachBuilder SetGiaBan(decimal? giaBan);
            ISachBuilder SetGiaBanGoc(decimal? giaBanGoc); // Thêm phương thức cho GiaBanGoc
            ISachBuilder SetMota(string mota);
            ISachBuilder SetNguoiDich(string nguoiDich);
            ISachBuilder SetAnhBia(string anhBia);
            ISachBuilder SetNgayCapNhat(DateTime? ngayCapNhat);
            ISachBuilder SetSoLuongTon(int? soLuongTon);
            Sach Build();
        }

        // Class Builder
        public class SachBuilder : ISachBuilder
        {
            private Sach sach;

            public SachBuilder()
            {
                sach = new Sach();
            }

            public ISachBuilder SetMaLoai(int maLoai)
            {
                sach.MaLoai = maLoai;
                return this;
            }

            public ISachBuilder SetMaNXB(int maNXB)
            {
                sach.MaNXB = maNXB;
                return this;
            }

            public ISachBuilder SetMaTG(int maTG)
            {
                sach.MaTG = maTG;
                return this;
            }

            public ISachBuilder SetTenSach(string tenSach)
            {
                sach.TenSach = tenSach;
                return this;
            }

            public ISachBuilder SetGiaBan(decimal? giaBan)
            {
                sach.GiaBan = giaBan;
                return this;
            }

            public ISachBuilder SetGiaBanGoc(decimal? giaBanGoc) 
            {
                sach.GiaBanGoc = giaBanGoc;
                return this;
            }

            public ISachBuilder SetMota(string mota)
            {
                sach.Mota = mota;
                return this;
            }

            public ISachBuilder SetNguoiDich(string nguoiDich)
            {
                sach.NguoiDich = nguoiDich;
                return this;
            }

            public ISachBuilder SetAnhBia(string anhBia)
            {
                sach.AnhBia = anhBia;
                return this;
            }

            public ISachBuilder SetNgayCapNhat(DateTime? ngayCapNhat)
            {
                sach.NgayCapNhat = ngayCapNhat;
                return this;
            }

            public ISachBuilder SetSoLuongTon(int? soLuongTon)
            {
                sach.SoLuongTon = soLuongTon;
                return this;
            }

            public Sach Build()
            {
                return sach;
            }
        }
    }
}
