using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LeaveManagementSystem.Controllers
{
    public class LeaveController : Controller
    {
        private readonly LeaveRepository _repo;

        public LeaveController(LeaveRepository repo)
        {
            _repo = repo;
        }

        // GET: Leave/Index
        public IActionResult Index(string search)
        {
            var data = _repo.GetAll();

            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x => x.EmployeeName.Contains(search)).ToList();
            }

            return View(data);
        }

        // GET: Leave/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leave/Create
        [HttpPost]
        public IActionResult Create(LeaveRequest model)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(model);
                TempData["msg"] = "Saved Successfully";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Leave/Edit/5
        public IActionResult Edit(int id)
        {
            var data = _repo.GetById(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // POST: Leave/Edit
        [HttpPost]
        public IActionResult Edit(LeaveRequest model)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Leave/Delete/5
        public IActionResult Delete(int id)
        {
            _repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}