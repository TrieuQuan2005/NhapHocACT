using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hehehe.Models
{
    public class InitUserForm
    {
        [Key]
        [ForeignKey("UserForm")]
        public string MaNhapHoc { get; set; } = null!; 
        
        public string HoTen { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }
        public string Cccd  { get; set; } = null!;
        public string GioiTinh { get; set; } = null!;

        public string NoiSinh { get; set; } = null!;
        public string DanToc { get; set; } = null!;
        public string NoiThuongTru { get; set; } = null!;
        public string ChoOHienNay { get; set; } = null!;
        public string KhuVuc { get; set; } = null!;
        
        public string NganhDaoTao { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string Email { get; set; } = null!;
        
        public bool IsSentPassNotiEmail { get; set; } = false;
        
        // Navigation property
        public User User { get; set; } = null!;
        public UserForm? UserForm { get; set; }
    }
}
