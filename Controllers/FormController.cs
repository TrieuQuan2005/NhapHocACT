using hehehe.Data;
using hehehe.Models;
using hehehe.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace hehehe.Controllers
{
    public class FormController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FormController> _logger;

        public FormController(ApplicationDbContext db, IWebHostEnvironment env, ILogger<FormController> logger)
        {
            _db = db;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult FormNhapThongTin()
        {
            var ma = HttpContext.Session.GetString("MaNhapHoc").ToUpper();
            if (string.IsNullOrEmpty(ma)) return RedirectToAction("Login", "Auth");

            var initData = _db.InitUserForm.FirstOrDefault(x => x.MaNhapHoc == ma);
            if (initData == null) return NotFound("Không tìm thấy dữ liệu ban đầu.");

            var formData = _db.UserForms.FirstOrDefault(x => x.MaNhapHoc == ma);
            
            var fileDict = _db.UserUploadFiles.Where(f => f.MaNhapHoc == ma).ToDictionary(f => f.FileType, f => f.FilePath);
            ViewBag.FileDict = fileDict;
            
            if (formData?.IsLocked == true)
            {
                var fileList = _db.UserUploadFiles.Where(f => f.MaNhapHoc == ma).ToList();
                ViewBag.FileList = fileList;
                return View("Locked", formData);
            }
            
            var viewModel = new FormViewModel
            {
                InitData = initData,
                FormData = formData
            };
            
            var avt = _db.UserUploadFiles.FirstOrDefault(f => f.MaNhapHoc == ma && f.FileType == "Avatar");
            if (avt != null)
            {
                ViewBag.AvatarFilePath = avt.FilePath;
            }
            else
            {
                ViewBag.AvatarFilePath = null;
            }
            
            return View(viewModel);
        }
        
        bool IsValidFileSignature(IFormFile file)
        {
            byte[] buffer = new byte[8]; // đọc nhiều hơn để so với các signature dài
            using (var stream = file.OpenReadStream())
            {
                stream.Read(buffer, 0, buffer.Length);
            }

            var signatures = new List<byte[]>
            {
                new byte[] { 0x25, 0x50, 0x44, 0x46 },             // PDF (%PDF)
                new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D },       // PDF (%PDF-)
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },             // JPG (JFIF)
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },             // JPG (EXIF)
                new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },             // JPG
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, // PNG
                new byte[] { 0x52, 0x49, 0x46, 0x46 }              // WEBP (RIFF....WEBP)
            };

            return signatures.Any(sig => buffer.Take(sig.Length).SequenceEqual(sig));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        private async Task SaveFileWithType(string ma, IFormFile file, string fileType)
        {
            if (file == null || file.Length == 0) return;
            
            const long maxSize = 5 * 1024 * 1024;
            if(!IsValidFileSignature(file)) throw new Exception("Invalid file signature.");
            if (file.Length > maxSize) throw new Exception($"File {fileType} is too big.");
            
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{ma}_{fileType}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{ext}";

            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", ma); 
            Directory.CreateDirectory(uploadDir);
            
            var safePath = Path.GetFullPath(Path.Combine(uploadDir, fileName));
            if (!safePath.StartsWith(Path.GetFullPath(uploadDir))) throw new Exception("Đường dẫn không hợp lệ.");
            
            using var stream = new FileStream(safePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = $"/uploads/{ma}/{fileName}";

            var existing = _db.UserUploadFiles.FirstOrDefault(f => f.MaNhapHoc == ma && f.FileType == fileType);
            if (existing != null)
            {
                existing.FilePath = relativePath;
                existing.FileTail = ext;
                _db.UserUploadFiles.Update(existing);
            }
            else
            {
                _db.UserUploadFiles.Add(new User_UploadFile
                {
                    MaNhapHoc = ma,
                    FileType = fileType,
                    FilePath = relativePath,
                    FileTail = ext
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormNhapThongTin(
            UserForm model,
            IFormFile avatar,
            IFormFile HocBa,
            IFormFile KetQuaThiTHPT,
            IFormFile TotNghiepTamThoi,
            IFormFile CCCD,
            IFormFile GiayBaoTrungTuyen,
            IFormFile KhaiSinh,
            IFormFile SoYeuLyLich,
            IFormFile PSV,
            IFormFile BHYT,
            IFormFile MinhChungLePhi,
            IFormFile CCTA,
            IFormFile UT1,
            IFormFile UT2,
            IFormFile UT3
            )
        {
            var ma = HttpContext.Session.GetString("MaNhapHoc");
            if (string.IsNullOrEmpty(ma)) return RedirectToAction("Login", "Auth");
        
            var formData = _db.UserForms.FirstOrDefault(x => x.MaNhapHoc == ma);

            if (formData != null && formData.IsLocked)
            {
                var uploadedFiles = _db.UserUploadFiles
                    .Where(f => f.MaNhapHoc == ma)
                    .ToList();

                ViewBag.UploadedFiles = uploadedFiles;
                return View("Locked", formData);
                
            }
            
            var initData = _db.InitUserForm.FirstOrDefault(x => x.MaNhapHoc == ma);
            var userFolder = Path.Combine("uploads", ma);
            var absolutePath = Path.Combine(_env.WebRootPath, userFolder);
            Directory.CreateDirectory(absolutePath);
        
            try
            {
                await SaveFileWithType(ma, avatar, "Avatar");
                await SaveFileWithType(ma, HocBa, "HocBa");
                await SaveFileWithType(ma, KetQuaThiTHPT, "KetQuaThiTHPT");
                await SaveFileWithType(ma, TotNghiepTamThoi, "TotNghiepTamThoi");
                await SaveFileWithType(ma, CCCD, "CCCD");
                await SaveFileWithType(ma,GiayBaoTrungTuyen, "GiayBaoTrungTuyen");
                await SaveFileWithType(ma, KhaiSinh, "KhaiSinh");
                await SaveFileWithType(ma, SoYeuLyLich, "SoYeuLyLich");
                await SaveFileWithType(ma, PSV, "PSV");
                await SaveFileWithType(ma, BHYT, "BHYT");
                await SaveFileWithType(ma, MinhChungLePhi, "MinhChungLePhi");
                await SaveFileWithType(ma, CCTA, "CCTA");
                await SaveFileWithType(ma, UT1, "UT1");
                await SaveFileWithType(ma, UT2, "UT2");
                await SaveFileWithType(ma, UT3, "UT3");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi upload file cho người dùng {MaNhapHoc}", ma);
                TempData["Message"] = "Định dạng file không hợp lệ hoặc dung lượng file vượt quá 5 Mb";
                return View("FormNhapThongTin", new FormViewModel { InitData = initData, FormData = formData });
            }

        
            // Lưu thông tin form
            if (formData == null)
            {
                model.MaNhapHoc = ma;
                model.IsLocked = false;
                model.HoTen = initData.HoTen;
                model.Cccd = initData.Cccd;
                model.NgaySinh = initData.NgaySinh;
                model.GioiTinh = initData.GioiTinh;
                model.NoiSinh = initData.NoiSinh;
                model.DanToc = initData.DanToc;
                model.NoiThuongTru = initData.NoiThuongTru;
                model.ChoOHienNay = initData.ChoOHienNay;
                model.NganhDaoTao = initData.NganhDaoTao;
                model.SoDienThoai = initData.SoDienThoai;
                model.Email = initData.Email;
                model.KhuVuc = initData.KhuVuc;
        
                _db.UserForms.Add(model);
            }
            else
            {
                model.MaNhapHoc = ma;
                model.IsLocked = false;
                model.HoTen = initData.HoTen;
                model.Cccd = initData.Cccd;
                model.NgaySinh = initData.NgaySinh;
                model.GioiTinh = initData.GioiTinh;
                model.NoiSinh = initData.NoiSinh;
                model.DanToc = initData.DanToc;
                model.NoiThuongTru = initData.NoiThuongTru;
                model.ChoOHienNay = initData.ChoOHienNay;
                model.NganhDaoTao = initData.NganhDaoTao;
                model.SoDienThoai = initData.SoDienThoai;
                model.Email = initData.Email;
                model.KhuVuc = initData.KhuVuc;
        
                _db.Entry(formData).CurrentValues.SetValues(model);
                _db.UserForms.Update(formData);
            }
        
            await _db.SaveChangesAsync();
            TempData["Message"] = "Lưu thành công!";

            var avatarPath = _db.UserUploadFiles
                .FirstOrDefault(f => f.MaNhapHoc == ma && f.FileType == "Avatar")
                ?.FilePath ?? "/images/default-avatar.png";
            ViewBag.AvatarPath = avatarPath;

            return View("Success", formData ?? model);
            
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuiYeuCauDinhChinh(YeuCauDinhChinh model, IFormFile? file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    if (!IsValidFileSignature(file))
                    {
                        TempData["Message"] = "❌ Định dạng file không được phép.";
                        return RedirectToAction("FormNhapThongTin");
                    }
                    
                    string rootFolder = Path.Combine(_env.WebRootPath, "MinhChung");
                    string studentFolder = Path.Combine(rootFolder, model.MaNhapHoc);
                    Directory.CreateDirectory(studentFolder);

                    string uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(studentFolder, uniqueFileName);
                    string safePath = Path.GetFullPath(filePath);
                    if (!safePath.StartsWith(Path.GetFullPath(studentFolder)))
                    {
                        TempData["Message"] = "❌ Đường dẫn không hợp lệ.";
                        return RedirectToAction("FormNhapThongTin");
                    }

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fs);
                    }

                    model.FileMinhChung = Path.Combine(model.MaNhapHoc, uniqueFileName);
                }

                model.NgayGui = DateTime.Now;
                model.DaDuyet = false;
                model.BiTuChoi = false;

                _db.YeuCauDinhChinh.Add(model);
                _db.SaveChanges();

                TempData["Message"] = "📨 Yêu cầu đính chính đã được gửi!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi yêu cầu đính chính cho {MaNhapHoc}", model.MaNhapHoc);
                TempData["Message"] = "❌ Gửi yêu cầu thất bại. Vui lòng thử lại.";
            }

            return RedirectToAction("FormNhapThongTin");
        }
        
        [HttpGet]
        public IActionResult HocPhi()
        {
            var vm = new HocPhiViewModel();
            return View(vm);
        }
        
    }
}
