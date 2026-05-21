using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Models;
using StudyPlanner.Services;
using System.Security.Claims;

namespace StudyPlanner.Controllers
{
    [Authorize]
    public class AssignmentController : Controller
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ApplicationDbContext _context;

        public AssignmentController(
            IAssignmentService assignmentService,
            ApplicationDbContext context)
        {
            _assignmentService = assignmentService;
            _context = context;
        }

        // GET: /Assignment
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var assignments = await _assignmentService.GetUserAssignmentsAsync(userId);

            return View(assignments);
        }

        // GET: /Assignment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id, userId);

            if (assignment == null)
                return NotFound();

            return View(assignment);
        }

        // GET: /Assignment/Create
        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await LoadViewBags(userId);

            return View();
        }

        // POST: /Assignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assignment assignment, int[] selectedTags)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _assignmentService.CreateAssignmentAsync(
                    assignment,
                    userId,
                    selectedTags);

                return RedirectToAction(nameof(Index));
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await LoadViewBags(currentUserId, assignment.SubjectId);

            return View(assignment);
        }

        // GET: /Assignment/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id, userId);

            if (assignment == null)
                return NotFound();

            await LoadViewBags(userId, assignment.SubjectId);

            ViewBag.SelectedTags = assignment.AssignmentTags
                .Select(at => at.TagId)
                .ToArray();

            return View(assignment);
        }

        // POST: /Assignment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Assignment assignment, int[] selectedTags)
        {
            if (id != assignment.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var updated = await _assignmentService.UpdateAssignmentAsync(
                    assignment,
                    userId,
                    selectedTags);

                if (updated == null)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await LoadViewBags(currentUserId, assignment.SubjectId);

            return View(assignment);
        }

        // POST: /Assignment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var success = await _assignmentService.DeleteAssignmentAsync(id, userId);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadViewBags(int userId, int? selectedSubjectId = null)
        {
            ViewBag.Subjects = new SelectList(
                await _context.Subjects
                    .Where(s => s.OwnerId == userId)
                    .ToListAsync(),
                "Id",
                "Name",
                selectedSubjectId);

            ViewBag.Tags = await _context.Tags
                .Where(t => t.OwnerId == userId)
                .ToListAsync();
        }
    }
}