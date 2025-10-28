namespace hehehe.Models.ViewModels
{
    public class HocPhiViewModel
    {
        public decimal HocPhi { get; set; } = 8400000;
        public decimal BHYT  { get; set; } = 737100;
        public decimal ThuVien { get; set; } = 500000;
        public decimal TheSinhVien { get; set; } = 100000;
        public decimal KhamSucKhoe { get; set; } = 265000;

        public decimal TongTien =>
            HocPhi + BHYT + ThuVien + KhamSucKhoe  + TheSinhVien;
    }
}

