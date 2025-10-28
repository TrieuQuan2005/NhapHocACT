using Microsoft.AspNetCore.Mvc;
using hehehe.Data;
namespace hehehe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public HomeController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        [Route("Home/Error")]
        public IActionResult Error()
        {
            return View();
        }
        
        public IActionResult WelcomeCards()
        {
            var ma = HttpContext.Session.GetString("MaNhapHoc");
            var user = _db.InitUserForm.FirstOrDefault(u => u.MaNhapHoc == ma);
            ViewBag.user = user;
            return View();
        }

        
    }
    
}

