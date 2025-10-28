using System.ComponentModel.DataAnnotations;

namespace hehehe.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string MaNhapHoc { get; set; } = string.Empty;

        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải ít nhất 6 ký tự.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
