using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hehehe.Models
{
    public class User_UploadFile
    {
        [Key]
        [Column(Order = 0)]
        public string MaNhapHoc { get; set; } = string.Empty;

        [Key]
        [Column(Order = 1)]
        public string FileType { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string FileTail { get; set; } = string.Empty;

        [ForeignKey("MaNhapHoc")]
        public User? User { get; set; }
    }
}