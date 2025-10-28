// Models/YeuCauDinhChinh.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace hehehe.Models
{
    public class YeuCauDinhChinh
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MaNhapHoc { get; set; } = null!;

        [Required]
        public string TruongCanDinhChinh { get; set; } = null!;

        [Required]
        public string GiaTriMoi { get; set; } = null!;

        public string FileMinhChung { get; set; }

        public DateTime NgayGui { get; set; } = DateTime.Now;

        public bool DaDuyet { get; set; } = false;
        public bool BiTuChoi { get; set; } = false;

        public string? GhiChuAdmin { get; set; } 
    }
}