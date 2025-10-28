using Microsoft.AspNetCore.Mvc;
using hehehe.Data;
using hehehe.Models.ViewModels; 
using System.Net;
using hehehe.Services;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using hehehe.Models;
using hehehe.models.viewmodels;
using System.IO.Compression;

namespace hehehe.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly EmailService _email;
        private readonly IWebHostEnvironment _env;

        public AdminController(IWebHostEnvironment env,ApplicationDbContext db, EmailService email)
        {
            _env = env;
            _db = db;
            _email = email;
        }

        private bool IsAdmin()
        {
            var maNhapHoc = HttpContext.Session.GetString("MaNhapHoc");
            if (string.IsNullOrEmpty(maNhapHoc)) return false;

            var user = _db.Users.Find(maNhapHoc);
            return user != null && user.IsAdmin;
        }

        [HttpGet]
        public IActionResult ThongKe()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            string MaNhapHoc = HttpContext.Session.GetString("MaNhapHoc").ToUpper();

            var totalUser = _db.Users.Count() - 5;
            var totalForm = _db.UserForms.Count();

            var atpbUserList = _db.InitUserForm.Where(f => f.MaNhapHoc.ToUpper().Contains("ATPB")).ToList();
            var atpnUserList = _db.InitUserForm.Where(f => f.MaNhapHoc.ToUpper().Contains("ATPN")).ToList();
            var ctUserList = _db.InitUserForm.Where(f => f.MaNhapHoc.ToUpper().Contains("CNTT")).ToList();
            var dtUserList = _db.InitUserForm.Where(f => f.MaNhapHoc.ToUpper().Contains("DTVT")).ToList();

            var atpbFormList = _db.UserForms.Where(f => f.MaNhapHoc.ToUpper().Contains("ATPB")).ToList();
            var atpnFormList = _db.UserForms.Where(f => f.MaNhapHoc.ToUpper().Contains("ATPN")).ToList();
            var ctFormList = _db.UserForms.Where(f => f.MaNhapHoc.ToUpper().Contains("CNTT")).ToList();
            var dtFormList = _db.UserForms.Where(f => f.MaNhapHoc.ToUpper().Contains("DTVT")).ToList();
            if (MaNhapHoc == "ADMINISTRATOR")
            {
                var stats = new
                {
                    TotalUser = totalUser,
                    TotalForm = totalForm,
                    ATPB = new
                    {
                        total = atpbUserList.Count, current = atpbFormList.Count,
                        percent = atpbFormList.Count * 100 / Math.Max(1, atpbUserList.Count), list = atpbFormList,
                        locked = atpbFormList.Count(f => f.IsLocked)
                    },
                    ATPN = new
                    {
                        total = atpnUserList.Count, current = atpnFormList.Count,
                        percent = atpnFormList.Count * 100 / Math.Max(1, atpnUserList.Count), list = atpnFormList,
                        locked =  atpnFormList.Count(f => f.IsLocked)
                    },
                    CT = new
                    {
                        total = ctUserList.Count, current = ctFormList.Count,
                        percent = ctFormList.Count * 100 / Math.Max(1, ctUserList.Count), list = ctFormList,
                        locked =  ctFormList.Count(f => f.IsLocked)
                    },
                    DT = new
                    {
                        total = dtUserList.Count, current = dtFormList.Count,
                        percent = dtFormList.Count * 100 / Math.Max(1, dtUserList.Count), list = dtFormList,
                        locked =  dtFormList.Count(f => f.IsLocked)
                    }
                };
                ViewBag.Stats = stats;
            }

            if (MaNhapHoc == "ADMINATPB")
            {
                var stats = new
                {
                    ATPB = new
                    {
                        total = atpbUserList.Count, current = atpbFormList.Count,
                        percent = atpbFormList.Count * 100 / Math.Max(1, atpbUserList.Count), list = atpbFormList,
                        locked =  atpbFormList.Count(f => f.IsLocked)
                    },
                };
                ViewBag.Stats = stats;
            }

            if (MaNhapHoc == "ADMINATPN")
            {
                var stats = new
                {
                    ATPN = new
                    {
                        total = atpnUserList.Count, current = atpnFormList.Count,
                        percent = atpnFormList.Count * 100 / Math.Max(1, atpnUserList.Count), list = atpnFormList,
                        locked =  atpnFormList.Count(f => f.IsLocked)
                    }
                };
                ViewBag.Stats = stats;
            }

            if (MaNhapHoc == "ADMINCNTT")
            {
                var stats = new
                {
                    CT = new
                    {
                        total = ctUserList.Count, current = ctFormList.Count,
                        percent = ctFormList.Count * 100 / Math.Max(1, ctUserList.Count), list = ctFormList,
                        locked =  ctFormList.Count(f => f.IsLocked)
                    },
                };
                ViewBag.Stats = stats;
            }

            if (MaNhapHoc == "ADMINDTVT")
            {
                var stats = new
                {
                    DT = new
                    {
                        total = dtUserList.Count, current = dtFormList.Count,
                        percent = dtFormList.Count * 100 / Math.Max(1, dtUserList.Count), list = dtFormList,
                        locked =  dtFormList.Count(f => f.IsLocked)
                    }
                };
                ViewBag.Stats = stats;
            }

            ViewBag.MaNhapHoc = MaNhapHoc;
            return View();
        }

        [HttpGet]
        public IActionResult SubmitedList(string nganh = "", string maNhapHoc = "", string lockStatus = "")
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            string MaDangNhap = HttpContext.Session.GetString("MaNhapHoc").ToUpper();
            ViewBag.MaDangNhap = MaDangNhap;
            var formList = _db.UserForms.ToList();

            if (!string.IsNullOrEmpty(lockStatus))
            {
                if (lockStatus == "locked") formList = formList.Where(f => f.IsLocked).ToList();
                else if (lockStatus == "unlocked") formList = formList.Where(f => f.IsLocked == false).ToList();
            }

            if (MaDangNhap == "ADMINISTRATOR")
            {
                if (!string.IsNullOrEmpty(nganh))
                    formList = formList.Where(f => f.MaNhapHoc.StartsWith(nganh.ToUpper())).ToList();
                if (!string.IsNullOrEmpty(maNhapHoc))
                    formList = formList.Where(f => f.MaNhapHoc.ToUpper() == maNhapHoc).ToList();

                ViewBag.MaNhapHocFilter = maNhapHoc.ToUpper();
                ViewBag.nganh = nganh;
            }
            else if (MaDangNhap == "ADMINATPB")
                formList = formList.Where(f => f.MaNhapHoc.ToUpper().StartsWith("ATPB")).ToList();
            else if (MaDangNhap == "ADMINATPN")
                formList = formList.Where(f => f.MaNhapHoc.ToUpper().StartsWith("ATPN")).ToList();
            else if (MaDangNhap == "ADMINCNTT")
                formList = formList.Where(f => f.MaNhapHoc.ToUpper().StartsWith("CNTT")).ToList();
            else if (MaDangNhap == "ADMINDTVT")
                formList = formList.Where(f => f.MaNhapHoc.ToUpper().StartsWith("DTVT")).ToList();

            ViewBag.LockStatus = lockStatus;
            ViewBag.List = formList;
            return View();
        }

        [HttpPost]
        public IActionResult ToggleLock(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return Json(new { success = false, message = "Mã người dùng không hợp lệ." });

            var user = _db.Users.Find(id);
            var userForm = _db.UserForms.FirstOrDefault(f => f.MaNhapHoc == user.MaNhapHoc);
            if (user == null) return Json(new { success = false, message = "Không tìm thấy người dùng." });

            try
            {
                user.IsLocked = !user.IsLocked;
                userForm.IsLocked = user.IsLocked;
                _db.SaveChanges();
                return Json(new
                {
                    success = true,
                    locked = user.IsLocked,
                    message = user.IsLocked
                        ? "Đã khóa form của người dùng."
                        : "Đã mở khóa form của người dùng."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        public IActionResult XemChiTiet(string id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var user = _db.Users.FirstOrDefault(u => u.MaNhapHoc == id && !u.IsAdmin);
            if (user == null) return NotFound();

            var form = _db.UserForms.FirstOrDefault(f => f.MaNhapHoc == user.MaNhapHoc);
            if (form == null) return NotFound();

            var fileList = _db.UserUploadFiles
                .Where(f => f.MaNhapHoc == id)
                .ToList();

            ViewBag.FileList = fileList;

            return View("XemChiTiet", form);
        }

        [HttpGet]
        public IActionResult DuyetDinhChinh()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var MaNhapHoc = HttpContext.Session.GetString("MaNhapHoc");

            var dsHopLe = _db.UserForms.Select(u => u.MaNhapHoc).ToList();

            var danhSach = _db.YeuCauDinhChinh
                .Where(yc => dsHopLe.Contains(yc.MaNhapHoc)) // lọc theo UserForm
                .OrderByDescending(x => x.NgayGui)
                .ToList();

            if (MaNhapHoc.ToUpper() == "ADMINATPB")
                danhSach = danhSach.Where(f => f.MaNhapHoc.ToUpper().StartsWith("ATPB")).ToList();
            if (MaNhapHoc.ToUpper() == "ADMINATPN")
                danhSach = danhSach.Where(f => f.MaNhapHoc.ToUpper().StartsWith("ATPN")).ToList();
            if (MaNhapHoc.ToUpper() == "ADMINCNTT")
                danhSach = danhSach.Where(f => f.MaNhapHoc.ToUpper().StartsWith("CNTT")).ToList();
            if (MaNhapHoc.ToUpper() == "ADMINDTVT")
                danhSach = danhSach.Where(f => f.MaNhapHoc.ToUpper().StartsWith("DTVT")).ToList();

            ViewBag.DanhSachDinhChinh = danhSach;
            return View("DuyetDinhChinh");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DuyetYeuCau(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var yc = _db.YeuCauDinhChinh.FirstOrDefault(x => x.Id == id);
            if (yc == null) return Json(new { success = false, message = "Không tìm thấy yêu cầu." });
            if (yc.TruongCanDinhChinh == "IsLocked")
                return Json(new { success = false, message = "Không thể duyệt trường này." });


            var form = _db.UserForms.FirstOrDefault(f => f.MaNhapHoc == yc.MaNhapHoc);
            if (form == null) return Json(new { success = false, message = "Không tìm thấy form." });


            var init = _db.InitUserForm.FirstOrDefault(f => f.MaNhapHoc == yc.MaNhapHoc);
            var propInit = init.GetType().GetProperty(yc.TruongCanDinhChinh);
            var propForm = form.GetType().GetProperty(yc.TruongCanDinhChinh);

            try
            {
                if (propForm != null && propForm.CanWrite && propInit != null && propInit.CanWrite)
                {
                    object? newValue = Convert.ChangeType(yc.GiaTriMoi, propForm.PropertyType);
                    propForm.SetValue(form, newValue);
                    propInit.SetValue(init, newValue);
                }

                yc.DaDuyet = true;
                yc.BiTuChoi = !yc.DaDuyet;
                _db.SaveChanges();

                return Json(new { success = true, status = "approved", message = "\u2714\ufe0f Đã duyệt" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TuChoiYeuCau(int id, string GhiChuAdmin)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Bạn không có quyền." });


            var yc = _db.YeuCauDinhChinh.FirstOrDefault(x => x.Id == id);
            if (yc == null) return Json(new { success = false, message = "Không tìm thấy yêu cầu." });

            yc.BiTuChoi = true;
            yc.DaDuyet = false;
            yc.GhiChuAdmin = GhiChuAdmin;
            _db.SaveChanges();

            return Json(new { success = true, status = "rejected", message = "❌ Bị từ chối" });

        }

        [HttpGet]
        public IActionResult ExportExcelForm()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var MaDangNhap = HttpContext.Session.GetString("MaNhapHoc").ToUpper();
            ViewBag.MaDangNhap = MaDangNhap;

            var nganhList = new List<string>() { "ATPB", "ATPN", "CNTT", "DTVT", "All" };
            var model = new ExportOptionsViewModel
            {
                AvailableNganhs = nganhList
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportToExcel(string selectedNganh)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var data = _db.UserForms.ToList();
            if (selectedNganh != "All")
                data = data.Where(u => u.MaNhapHoc.Substring(0, 4).ToUpper() == selectedNganh).ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh sách sinh viên");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "STT";
                worksheet.Cell(currentRow, 2).Value = "Mã trúng tuyển";
                worksheet.Cell(currentRow, 3).Value = "Họ tên";
                worksheet.Cell(currentRow, 4).Value = "CCCD";
                worksheet.Cell(currentRow, 5).Value = "Ngày sinh";
                worksheet.Cell(currentRow, 6).Value = "Giới tính";
                worksheet.Cell(currentRow, 7).Value = "Nơi sinh";
                worksheet.Cell(currentRow, 8).Value = "Dân tộc";
                worksheet.Cell(currentRow, 9).Value = "Nơi thường trú";
                worksheet.Cell(currentRow, 10).Value = "Chỗ ở hiện nay";
                worksheet.Cell(currentRow, 11).Value = "Đối tượng ưu tiên";
                worksheet.Cell(currentRow, 12).Value = "Khu vực";
                worksheet.Cell(currentRow, 13).Value = "Năm nhập học";
                worksheet.Cell(currentRow, 14).Value = "Năm tốt nghiệp THPT";
                worksheet.Cell(currentRow, 15).Value = "Năm nhập ngũ";
                worksheet.Cell(currentRow, 16).Value = "Năm xuất ngũ";
                worksheet.Cell(currentRow, 17).Value = "Ngày vào đoàn";
                worksheet.Cell(currentRow, 18).Value = "ngày vào Đảng";
                worksheet.Cell(currentRow, 19).Value = "Ngành đào tạo";
                worksheet.Cell(currentRow, 20).Value = "Số điện thoại";
                worksheet.Cell(currentRow, 21).Value = "Email";
                worksheet.Cell(currentRow, 22).Value = "Họ tên bố";
                worksheet.Cell(currentRow, 23).Value = "Năm sinh bố";
                worksheet.Cell(currentRow, 24).Value = "Nghề nghiệp bố";
                worksheet.Cell(currentRow, 25).Value = "SDT bố";
                worksheet.Cell(currentRow, 26).Value = "Họ tên mẹ";
                worksheet.Cell(currentRow, 27).Value = "Năm sinh mẹ";
                worksheet.Cell(currentRow, 28).Value = "Nghề nghiệp mẹ";
                worksheet.Cell(currentRow, 29).Value = "SDT mẹ";
                worksheet.Cell(currentRow, 30).Value = "Báo tin cho ai";
                worksheet.Cell(currentRow, 31).Value = "Địa chỉ liên hệ";

                // Dữ liệu
                foreach (var user in data)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = currentRow - 1;
                    worksheet.Cell(currentRow, 2).Value = user.MaNhapHoc;
                    worksheet.Cell(currentRow, 3).Value = user.HoTen;
                    worksheet.Cell(currentRow, 4).Value = user.Cccd;
                    worksheet.Cell(currentRow, 5).Value = user.NgaySinh.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 6).Value = user.GioiTinh;
                    worksheet.Cell(currentRow, 7).Value = user.NoiSinh;
                    worksheet.Cell(currentRow, 8).Value = user.DanToc;
                    worksheet.Cell(currentRow, 9).Value = user.NoiThuongTru;
                    worksheet.Cell(currentRow, 10).Value = user.ChoOHienNay;
                    worksheet.Cell(currentRow, 11).Value = user.DoiTuongUuTien ? "Có" : "Không";
                    worksheet.Cell(currentRow, 12).Value = user.KhuVuc;
                    worksheet.Cell(currentRow, 13).Value = user.NamNhapHoc;
                    worksheet.Cell(currentRow, 14).Value = user.NamTotNghiepTHPT;
                    worksheet.Cell(currentRow, 15).Value = user.NamNhapNgu;
                    worksheet.Cell(currentRow, 16).Value = user.NamXuatNgu;
                    worksheet.Cell(currentRow, 17).Value = user.NgayVaoDoan;
                    worksheet.Cell(currentRow, 18).Value = user.NgayVaoDang;
                    worksheet.Cell(currentRow, 19).Value = user.NganhDaoTao;
                    worksheet.Cell(currentRow, 20).Value = user.SoDienThoai;
                    worksheet.Cell(currentRow, 21).Value = user.Email;
                    worksheet.Cell(currentRow, 22).Value = user.HoTenBo;
                    worksheet.Cell(currentRow, 23).Value = user.NamSinhBo;
                    worksheet.Cell(currentRow, 24).Value = user.NgheNghiepBo;
                    worksheet.Cell(currentRow, 25).Value = user.SoDienThoaiBo;
                    worksheet.Cell(currentRow, 26).Value = user.HoTenMe;
                    worksheet.Cell(currentRow, 27).Value = user.NamSinhMe;
                    worksheet.Cell(currentRow, 28).Value = user.NgheNghiepMe;
                    worksheet.Cell(currentRow, 29).Value = user.SoDienThoaiMe;
                    worksheet.Cell(currentRow, 30).Value = user.BaoTinChoAi;
                    worksheet.Cell(currentRow, 31).Value = user.DiaChiLienHe;
                }

                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"DanhSach_{selectedNganh}_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
        }

        [HttpGet]
        public IActionResult Quanlysinhvien()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View();
        }

        [HttpPost]
        public IActionResult Quanlysinhvien(string maNhapHoc)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            if (string.IsNullOrWhiteSpace(maNhapHoc))
            {
                ViewBag.Error = "Vui lòng nhập mã nhập học.";
                return View();
            }

            var inituserform = _db.InitUserForm.FirstOrDefault(u => u.MaNhapHoc == maNhapHoc);
            var user = _db.Users.FirstOrDefault(u => u.MaNhapHoc == maNhapHoc);

            if (inituserform == null || user == null)
            {
                ViewBag.Error = "Không tìm thấy người dùng.";
                return View();
            }

            Quanlysinhvien Sinhvien = new Quanlysinhvien();
            Sinhvien.MaNhapHoc = maNhapHoc;
            Sinhvien.Cccd = inituserform.Cccd;
            Sinhvien.Email = inituserform.Email;
            Sinhvien.SoDienThoai = inituserform.SoDienThoai;
            Sinhvien.NganhDaoTao = inituserform.NganhDaoTao;
            Sinhvien.Password = user.Password;
            return View(Sinhvien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetMatKhau(string maNhapHoc)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var inituser = _db.InitUserForm.FirstOrDefault(u => u.MaNhapHoc == maNhapHoc);
            var user = _db.Users.FirstOrDefault(u => u.MaNhapHoc == maNhapHoc);

            if (user == null || inituser == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng để reset mật khẩu.";
                return RedirectToAction("Quanlysinhvien");
            }

            user.Password = inituser.Cccd;
            _db.SaveChanges();

            TempData["Success"] = $"Mật khẩu của {maNhapHoc} đã được reset về số CCCD.";
            return RedirectToAction("Quanlysinhvien");
        }
        
        [HttpGet]
        public IActionResult SinhVienTrungTuyen()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            
            var initUserForms = _db.InitUserForm.ToList();
            var user = _db.Users.ToList();  
            ViewBag.InitUserForms = initUserForms;
            ViewBag.User = user;
            return View();
        }
        
        public void SendPassNotiEmail(string maNhapHoc)
        {
            var initUser = _db.InitUserForm.FirstOrDefault(u => u.MaNhapHoc == maNhapHoc);
            var user = _db.Users.FirstOrDefault(u => u.MaNhapHoc == maNhapHoc);
            if (initUser != null && !initUser.IsSentPassNotiEmail)
            {
                var Subject = "Thông báo nhập học trực tuyến";
                var Body = 
                    $"<div>Xin chào {initUser.HoTen}</div>" + 
                    "<div>Học viện Kỹ thuật mật mã chúc mừng bạn đã xác nhận nhập học thành công. Chào đón bạn trở thành thành viên của Học viện.</div>" +
                    "<div>Chúc bạn có những năm tháng học tập tích cực, thành công rực rỡ tại ngôi nhà chung này!</div>" + 
                    $"<div>Giấy báo trúng tuyển kiêm giấy báo nhập học: Bản điện tử đã được gửi đến bạn qua Email: {initUser.Email}; Bản giấy được gửi tới bạn qua đường bưu điện.</div>" + 
                    "<div>Nhập học trực tuyến tại: <a href = \"https://nhaphoc.actvn.edu.vn\" >Nhập học trực tuyến KMA</a></div>" + 
                    $"<div>Mã trúng tuyển: {initUser.MaNhapHoc}</div>" +
                    $"<div>Password: {user.Password} <small>(vui lòng đổi mật khẩu sau lần đăng nhập đầu tiên)</small></div>";
                   
                var rootPath = _env.WebRootPath;
                
                var giayBaoTrungTuyen = Path.Combine(rootPath, "Attachments", "GiayBaoTungTuyen", $"{initUser.MaNhapHoc} GiayBaoTrungTuyen.pdf");
                var PhieuSinhVien = Path.Combine(rootPath, "Attachments", "Phiếu sinh viên 2025.pdf");
                var BHYT_ATTT = Path.Combine(rootPath, "Attachments", "Mẫu khai BHYT sinh viên ATTT.pdf");
                var BHYT_MIC = Path.Combine(rootPath, "Attachments", "Tờ khai yêu cầu bảo hiểm MIC 2025.pdf");
                var HuongDan = Path.Combine(rootPath, "Attachments", "HƯỚNG DẪN NHẬP HỌC (Bản chính thức).pdf");

                var Attachments = new string[5]
                {
                    giayBaoTrungTuyen,
                    PhieuSinhVien,
                    BHYT_MIC,
                    BHYT_ATTT,
                    HuongDan,
                };

                try
                {
                    _email.SendEmail(initUser.Email, Subject, Body, Attachments);
                    TempData["Success"] = $"Đã gửi Email tới {initUser.HoTen} ({initUser.MaNhapHoc})";
                    initUser.IsSentPassNotiEmail = true;
                    _db.SaveChanges();

                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Không thể gửi Email tới {initUser.HoTen} ({initUser.MaNhapHoc}) do " + ex.Message;
                }
                
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendPassNotiEmailSiglely(string maNhapHoc)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            
            var initUser = _db.InitUserForm.FirstOrDefault(u => u.MaNhapHoc == maNhapHoc);
            SendPassNotiEmail(initUser.MaNhapHoc);
            return RedirectToAction("SinhVienTrungTuyen");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendPassNotiEmailAutomatically()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var logFile = Path.Combine(_env.WebRootPath,"logs", $"SendPassEmailLog.txt");
            
            using (var writer = new StreamWriter(logFile, false))
            {
                var initUsers = _db.InitUserForm.ToList();

                foreach (var initUser in initUsers)
                {
                    try
                    {
                        SendPassNotiEmail(initUser.MaNhapHoc); 
                        writer.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✅ Đã gửi mail cho {initUser.HoTen} ({initUser.MaNhapHoc})");
                    }
                    catch (Exception ex)
                    {
                        writer.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Lỗi gửi mail cho {initUser.HoTen} ({initUser.MaNhapHoc}): {ex.Message}");
                    }
                }
            }

            TempData["LogFile"] = $"/logs/{Path.GetFileName(logFile)}";
            
            return RedirectToAction("SinhVienTrungTuyen");
        }
        
        
        [HttpGet]
        public IActionResult SendSucessEmail()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var isConfirmedUserForm = _db.UserForms.Where(f => f.IsLocked == true).ToList();
            var maDangNhap = HttpContext.Session.GetString("MaNhapHoc");
            switch (maDangNhap.ToUpper())
            {
                case "ADMINATPB":
                {
                    isConfirmedUserForm = isConfirmedUserForm.Where(f => f.MaNhapHoc.StartsWith("ATPB")).ToList();
                    break;
                }
                case "ADMINATPN":
                {
                    isConfirmedUserForm = isConfirmedUserForm.Where(f => f.MaNhapHoc.StartsWith("ATPN")).ToList();
                    break;
                }
                case "ADMINCNTT":
                {
                    isConfirmedUserForm = isConfirmedUserForm.Where(f => f.MaNhapHoc.StartsWith("CNTT")).ToList();
                    break;
                }
                case "ADMINDTVT":
                {
                    isConfirmedUserForm = isConfirmedUserForm.Where(f => f.MaNhapHoc.StartsWith("DTVT")).ToList();
                    break;
                }
            }
            ViewBag.IsConfirmedUserForm = isConfirmedUserForm;
            return View();
        }

        [HttpPost]
        public IActionResult SendSucessEmail(string maNhapHoc)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var isConfirmedUserForm = _db.UserForms.FirstOrDefault(f => f.MaNhapHoc == maNhapHoc);
            if (isConfirmedUserForm == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sinh viên." });
            }

            var subject = "Thông báo về trạng thái hồ sơ nhập học";
            var body =
                $"<div>Chào bạn <strong>{isConfirmedUserForm.HoTen}</strong>,</div>" +
                $"<div>Hội đồng tuyển sinh Học viện Kỹ thuật Mật mã đã xác nhận hồ sơ nhập học của bạn.</div>" +
                $"<div><strong>Chúc mừng bạn đã trở thành tân sinh viên của Học viện Kỹ thuật Mật mã 🎉</strong></div>" +
                $"<div>Trân trọng thông báo!</div>";

            try
            {
                _email.SendEmail(isConfirmedUserForm.Email, subject, body, null);
                isConfirmedUserForm.isSentEmail = true;
                _db.SaveChanges();

                return Json(new { success = true, status = "sent", message = "Gửi Email thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi gửi email: " + ex.Message });
            }
        }
        
        [HttpGet]
        public IActionResult InvalidUserForms()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var submittedIds = _db.UserForms.Select(f => f.MaNhapHoc).ToList();
            var invalidUsers = _db.InitUserForm.Where(f => !submittedIds.Contains(f.MaNhapHoc)).ToList();

            var maDangNhap = HttpContext.Session.GetString("MaNhapHoc");
            if (maDangNhap != null)
            {
                if(maDangNhap.ToUpper() == "ADMINATPB") invalidUsers = invalidUsers.Where(f => f.MaNhapHoc.StartsWith("ATPB")).ToList();
                else if(maDangNhap.ToUpper() == "ADMINATPN") invalidUsers = invalidUsers.Where(f => f.MaNhapHoc.StartsWith("ATPN")).ToList();
                else if(maDangNhap.ToUpper() == "ADMINCNTT") invalidUsers = invalidUsers.Where(f => f.MaNhapHoc.StartsWith("CNTT")).ToList();
                else if(maDangNhap.ToUpper() == "ADMINDTVT") invalidUsers = invalidUsers.Where(f => f.MaNhapHoc.StartsWith("DTVT")).ToList();
            }
            
            ViewBag.InvalidUserForms = invalidUsers;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendAlertEmail(string maNhapHoc)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var initUser = _db.InitUserForm.FirstOrDefault(f => f.MaNhapHoc == maNhapHoc);

            if (initUser == null)
            {
                TempData["Error"] = "Không tìm thấy sinh viên!";
                return RedirectToAction("InvalidUserForms", "Admin");
            }

            var subject = "Nhắc nhở hoàn thiện hồ sơ nhập học";
            var body = $@"
        <p>Hội đồng tuyển sinh Học viện Kỹ thuật mật mã thông báo!</p>
        <p>Thí sinh chưa hoàn thiện hồ sơ nhập học trực tuyến.</p>
        <p>Đề nghị thí sinh hoàn thiện hồ sơ nhập học trực tuyến trước 24h00 này 08/9/2025.</p>
        <p>Trân trọng.</p>";

            try
            {
                _email.SendEmail(initUser.Email, subject, body, null);
                TempData["Success"] = $"Đã gửi email nhắc nhở tới {initUser.HoTen} ({initUser.Email}).";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi gửi email: " + ex.Message;
            }

            return RedirectToAction("InvalidUserForms", "Admin");
        }
        
        [HttpGet]
        public IActionResult DownloadAllUserFiles()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var wwwRootPath = _env.WebRootPath;

            var userForms = _db.UserForms.OrderBy(f => f.MaNhapHoc).ToList();

            var zipPath = Path.Combine(Path.GetTempPath(), $"AllUserFiles_{Guid.NewGuid()}.zip");

            using (var zipStream = new FileStream(zipPath, FileMode.Create))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var userForm in userForms)
                {
                    var maNhapHoc = userForm.MaNhapHoc;
                    var userFiles = _db.UserUploadFiles.Where(f => f.MaNhapHoc == maNhapHoc).ToList();

                    foreach (var file in userFiles)
                    {
                        var absolutePath = Path.Combine(wwwRootPath, "uploads", $"{userForm.MaNhapHoc}", Path.GetFileName(file.FilePath));

                        if (System.IO.File.Exists(absolutePath))
                        {
                            var entryName = Path.Combine(maNhapHoc, Path.GetFileName(absolutePath));
                            archive.CreateEntryFromFile(absolutePath, entryName);
                        }
                        else
                        {
                            Console.WriteLine("File does not exist");
                        }
                    }
                }
            }

            var stream = new FileStream(zipPath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/zip", "AllUserFiles.zip");
        }

        public IActionResult pdfSplitter()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View();
        }



    }
}
