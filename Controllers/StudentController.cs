using ASPNETCore_DB.Interfaces;
using ASPNETCore_DB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCore_DB.Controllers
{
    [TypeFilter(typeof(CustomExceptionFilter))]
    public class StudentController : Controller
    {
        private readonly IStudent _studentRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentController(
            IStudent studentRepo,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment)
        {
            _studentRepo = studentRepo;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StudentNumberSortParm"] = string.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            try
            {
                var students = _studentRepo.GetStudents(searchString, sortOrder).AsNoTracking();
                int pageSize = 3;
                return View(PaginatedList<Student>.Create(students, pageNumber ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception("No student records detected");
            }
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var student = _studentRepo.ByEmail(User.Identity?.Name);
                if (student == null)
                {
                    return NotFound();
                }
                return View(student);
            }
            else
            {
                var student = _studentRepo.Details(id);
                if (student == null)
                {
                    return NotFound();
                }
                return View(student);
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Create()
        {
            var studentExist = _studentRepo.ByEmail(User.Identity?.Name);
            if (studentExist != null)
            {
                return RedirectToAction("Details", new { id = studentExist.StudentNumber });
            }

            return View(new Student
            {
                Photo = "default.png",
                Email = User.Identity?.Name,
                EnrollmentDate = DateTime.Now
            });
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + WebConstants.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                student.Photo = fileName + extension;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    student.Email = User.Identity?.Name;
                    _studentRepo.Create(student);

                    var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                    var signInManager = HttpContext.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
                    var user = await userManager.FindByEmailAsync(student.Email);

                    if (user != null)
                    {
                        var currentRoles = await userManager.GetRolesAsync(user);
                        await userManager.RemoveFromRolesAsync(user, currentRoles);
                        await userManager.AddToRoleAsync(user, "Student");
                        await signInManager.RefreshSignInAsync(user);
                    }

                    var createdStudent = _studentRepo.ByEmail(User.Identity?.Name);
                    return RedirectToAction("Details", new { id = createdStudent?.StudentNumber });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to save student record: {ex.Message}");
                }
            }

            return View(student);
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var student = _studentRepo.Details(id);
            if (student == null)
            {
                return NotFound();
            }

            if (student.Email != User.Identity?.Name)
            {
                return Forbid();
            }

            return View(student);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string photoName, Student student)
        {
            if (student.Email != User.Identity?.Name)
            {
                return Forbid();
            }

            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + WebConstants.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                var oldFile = Path.Combine(upload, photoName);
                if (System.IO.File.Exists(oldFile) && photoName != "default.png")
                {
                    System.IO.File.Delete(oldFile);
                }

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                student.Photo = fileName + extension;
            }
            else
            {
                student.Photo = photoName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _studentRepo.Edit(student);
                    return RedirectToAction("Details", new { id = student.StudentNumber });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to save changes: {ex.Message}");
                }
            }

            return View(student);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var student = _studentRepo.Details(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var student = _studentRepo.Details(id);
            if (student == null)
            {
                return NotFound();
            }

            try
            {
                if (student.Photo != "default.png")
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string upload = webRootPath + WebConstants.ImagePath;
                    var oldFile = Path.Combine(upload, student.Photo);
                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }

                _studentRepo.Delete(student);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Unable to delete student: {ex.Message}");
                return View(student);
            }
        }
    }
}