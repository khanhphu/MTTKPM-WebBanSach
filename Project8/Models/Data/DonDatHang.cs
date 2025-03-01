namespace WebBanSach.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DonDatHang")]
    public partial class DonDatHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonDatHang()
        {
            ChiTietDDHs = new HashSet<ChiTietDDH>();
        }

        [Key]
        [Display(Name = "Mã đơn hàng")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDDH { get; set; }

        [Column(TypeName = "smalldatetime")]
        [Display(Name ="Ngày đặt")]
        public DateTime NgayDat { get; set; }

        [Column(TypeName = "smalldatetime")]
        [Display(Name = "Ngày giao")]
        public DateTime NgayGiao { get; set; }

        [Display(Name = "Tình trạng")]
        public bool TinhTrang { get; set; }

        public int MaKH { get; set; }
        public int? ThanhToan { get; set; }
        public int? Tracking { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDDH> ChiTietDDHs { get; set; }

        public virtual KhachHang KhachHang { get; set; }
    }
}
