using Microsoft.AspNetCore.Mvc;
using hehehe.Data;
using hehehe.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Identity;
namespace hehehe.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và số căn cước công dân.";
                return View();
            }
            
            var user = _db.Users.FirstOrDefault(u => u.MaNhapHoc == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("MaNhapHoc", user.MaNhapHoc);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

                if (!user.IsActive)
                {
                    ViewBag.Error = "Đã hết hạn nhập học trực truyến, tài khoản của thí sinh đã bị khóa";
                    return View();
                }
                
                return user.IsAdmin
                    ? RedirectToAction("ThongKe", "Admin")
                    : RedirectToAction("WelcomeCards","Home");
            }

            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
            ViewBag.Username = username;

            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var ma = HttpContext.Session.GetString("MaNhapHoc");
            return View(new ChangePasswordViewModel { MaNhapHoc = ma });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.MaNhapHoc == model.MaNhapHoc && u.Password == model.OldPassword);

            if (user != null)
            {
                if (model.OldPassword == model.NewPassword)
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu mới không được trùng mật khẩu cũ.");
                    return View(model);
                }
                
                user.Password = model.NewPassword;
                _db.SaveChanges();

                TempData["Success"] = "Đổi mật khẩu thành công.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Mật khẩu cũ không đúng.");
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login", "Auth");
        }

    }
}
