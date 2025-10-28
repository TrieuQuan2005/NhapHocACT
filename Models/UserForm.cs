using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hehehe.Models
{
    public class UserForm
    {
        [Key]
        [ForeignKey("User")]
        public string MaNhapHoc { get; set; } = null!; 

        public string HoTen { get; set; } = null!;
        public string Cccd  { get; set; } = null!;

        [DataType(DataType.Date)] public DateTime NgaySinh { get; set; } 

        public string GioiTinh { get; set; } = null!;

        public string NoiSinh { get; set; } = null!;
        public string DanToc { get; set; } = null!;
        public string NoiThuongTru { get; set; } = null!;
        public string ChoOHienNay { get; set; } = null!;
        

        public bool DoiTuongUuTien { get; set; }
        public string KhuVuc { get; set; }
        public int NamNhapHoc { get; set; }
        public int NamTotNghiepTHPT { get; set; }
        public int? NamNhapNgu { get; set; }
        public int? NamXuatNgu { get; set; }

        [DataType(DataType.Date)] public DateTime NgayVaoDoan { get; set; }

        [DataType(DataType.Date)] public DateTime? NgayVaoDang { get; set; }

        public string NganhDaoTao { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string HoTenBo { get; set; } = null!;
        public string NamSinhBo { get; set; }
        public string NgheNghiepBo { get; set; } = null!;
        public string SoDienThoaiBo { get; set; } = null!;

        public string HoTenMe { get; set; } = null!;
        public string NamSinhMe { get; set; }
        public string NgheNghiepMe { get; set; } = null!;
        public string SoDienThoaiMe { get; set; } = null!;

        public string BaoTinChoAi { get; set; } = null!;
        public string DiaChiLienHe { get; set; } = null!;
        
        public bool IsLocked { get; set; } = false;
        public bool isSentEmail { get; set; } = false;
        // Navigation property
        public User? User { get; set; }
        public InitUserForm? InitUserForm { get; set; }

    }
}
