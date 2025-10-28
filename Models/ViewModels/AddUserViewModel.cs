using System.ComponentModel.DataAnnotations;

namespace hehehe.Models
{
    public class AddUserViewModel
    {
        public string MaNhapHoc { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool IsLocked { get; set; } = false;

        public bool IsAdmin { get; set; } = false;
        
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
    }
}