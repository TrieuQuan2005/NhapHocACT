using System.ComponentModel.DataAnnotations;

namespace hehehe.Models
{
    public class UserStdInfo
    {
        [Key]
        public string MaNhapHoc { get; set; } = string.Empty;

        public string MaSinhVien { get; set; } = string.Empty;
        public string QlPassword { get; set; } = string.Empty;
        public string MsAccount { get; set; } = string.Empty;
        public string MsPassword { get; set; } = string.Empty;

        public User? User { get; set; }
    }
}