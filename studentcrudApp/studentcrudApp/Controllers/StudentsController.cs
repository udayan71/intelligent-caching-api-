using Microsoft.AspNetCore.Mvc;
using StudentCrudApp.Models;
using StudentCrudApp.Services;

namespace StudentCrudApp.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        // ✅ GET: /Students
        public async Task<IActionResult> Index()
        {
            var students = await _service.GetAllStudentsAsync();
            return View(students);
        }

        // ✅ GET: /Students/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var student = await _service.GetStudentByIdAsync(id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        // ✅ GET: /Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // ✅ POST: /Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateStudentAsync(student);
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // ✅ GET: /Students/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _service.GetStudentByIdAsync(id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        // ✅ POST: /Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateStudentAsync(student);
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // ✅ GET: /Students/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _service.GetStudentByIdAsync(id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        // ✅ POST: /Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteStudentAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}