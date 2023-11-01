using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskApp.Models;

namespace TaskApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly TaskContext _context;

        public TaskItemsController(TaskContext context)
        {
            _context = context;
        }

        // GET: api/TaskItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems(
            string? filterDate = null,
            bool? filterCompleted = null,
            string? filterMonth = null,
            string? orderBy = "asc"
            )
        {
            IQueryable<TaskItem> query = _context.TaskItems;
            
            if (!string.IsNullOrEmpty(filterDate))
            {
                if (DateTime.TryParse(filterDate, out var parsedFilterDate))
                {
                    query = query.Where(task => task.Deadline.Date == parsedFilterDate.Date);
                }
                else
                {
                    return BadRequest("Invalid 'filterDate' format. Use a valid date format.");
                }
            }

            if (filterCompleted.HasValue)
            {
                query = query.Where(task => task.Completed == filterCompleted.Value);
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                if (orderBy.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderByDescending(task => task.Deadline);
                }
                else
                {
                    query = query.OrderBy(task => task.Deadline);
                }
            }


            if (!string.IsNullOrEmpty(filterMonth))
            {
                if (DateTime.TryParseExact(filterMonth, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    var year = parsedDate.Year;
                    var month = parsedDate.Month;

                    query = query.Where(task => task.Deadline.Year == year && task.Deadline.Month == month);
                }
                else
                {
                    return BadRequest("Invalid 'filterMonth' format. Use 'YYYY-MM' format.");
                }
            }

            var taskItems = await query.ToListAsync();

            if (taskItems.Count == 0)
            {
                return NotFound();
            }

            return taskItems;
        }
        // GET: api/TaskItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            if (_context.TaskItems == null)
            {
                return NotFound();
            }
            var taskItem = await _context.TaskItems.FindAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

        // PUT: api/TaskItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(taskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TaskItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTaskItem(TaskItem taskItem)
        {
            if (_context.TaskItems == null)
            {
                return Problem("Entity set 'TaskContext.TaskItems'  is null.");
            }
            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskItem", new { id = taskItem.Id }, taskItem);
        }

        // POST: api/TaskItems/toggle/5
        [HttpPost("toggle/{id}")]
        public async Task<ActionResult<TaskItem>> ToggleCompletedTask(int id)
        {
            if (_context.TaskItems == null)
            {
                return Problem("Entity set 'TaskContext.TaskItems'  is null.");
            }
            var taskItem = await _context.TaskItems.FindAsync(id);
            
            if (taskItem == null)
            {
                return Problem("Task not found");
            }
            taskItem.Completed = !taskItem.Completed;

            await _context.SaveChangesAsync();

            return taskItem;
        }

        // DELETE: api/TaskItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            if (_context.TaskItems == null)
            {
                return NotFound();
            }
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskItemExists(int id)
        {
            return (_context.TaskItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
