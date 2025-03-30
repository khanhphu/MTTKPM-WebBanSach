namespace WebBanSach.Models.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using WebBanSach.Areas.Admin.Models;
    using WebBanSach.Models.Data.CommentsComposite;
    public partial class BSDBContext : DbContext
    {
        private static BSDBContext _instance;
        private static readonly object _lock = new object();
        public BSDBContext() : base("name=BSBD2")
        {
        }

        public static BSDBContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BSDBContext();
                        }
                    }
                }
                return _instance;
            }
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<ChiTietDDH> ChiTietDDHs { get; set; }
        public virtual DbSet<DonDatHang> DonDatHangs { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<LienHe> LienHes { get; set; }
        public virtual DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public virtual DbSet<Sach> Saches { get; set; }
        public virtual DbSet<TacGia> TacGias { get; set; }
        public virtual DbSet<TheLoai> TheLoais { get; set; }
      //them comments
      public virtual DbSet<Comments> Comments { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .Property(e => e.TaiKhoan)
                .IsUnicode(false);

            modelBuilder.Entity<Admin>()
                .Property(e => e.MatKhau)
                .IsUnicode(false);

            modelBuilder.Entity<DonDatHang>()
                .HasMany(e => e.ChiTietDDHs)
                .WithRequired(e => e.DonDatHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KhachHang>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<KhachHang>()
                .Property(e => e.DienThoai)
                .IsUnicode(false);

            modelBuilder.Entity<KhachHang>()
                .Property(e => e.TaiKhoan)
                .IsUnicode(false);

            modelBuilder.Entity<KhachHang>()
                .Property(e => e.MatKhau)
                .IsUnicode(false);

            modelBuilder.Entity<KhachHang>()
                .HasMany(e => e.DonDatHangs)
                .WithRequired(e => e.KhachHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LienHe>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<LienHe>()
                .Property(e => e.DienThoai)
                .IsUnicode(false);

            modelBuilder.Entity<NhaXuatBan>()
                .Property(e => e.DienThoai)
                .IsUnicode(false);

            modelBuilder.Entity<NhaXuatBan>()
                .HasMany(e => e.Saches)
                .WithRequired(e => e.NhaXuatBan)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Sach>()
                .Property(e => e.GiaBan)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Sach>()
                .Property(e => e.AnhBia)
                .IsUnicode(false);

            modelBuilder.Entity<Sach>()
                .HasMany(e => e.ChiTietDDHs)
                .WithRequired(e => e.Sach)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TacGia>()
                .HasMany(e => e.Saches)
                .WithRequired(e => e.TacGia)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TheLoai>()
                .HasMany(e => e.Saches)
                .WithRequired(e => e.TheLoai)
                .WillCascadeOnDelete(false);
            //comments
            //voi bang Khach hHang
            modelBuilder.Entity<Comments>()
              .HasRequired(c => c.User)
              .WithMany(kh => kh.Comments)
              .HasForeignKey(c => c.UserId)
              .WillCascadeOnDelete(false);
            //voi bang Sach
            modelBuilder.Entity<Comments>()
            .HasRequired(c => c.Book)
            .WithMany(s => s.Comments)
            .HasForeignKey(c => c.BookId)
            .WillCascadeOnDelete(false);
            //voi bang comments (parentId)
            modelBuilder.Entity<Comments>()
            .HasOptional(c => c.Parent)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentId)
            .WillCascadeOnDelete(false);
        }
    }
}
