using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eLibrary.DataLayers.Context;
using eLibrary.DataLayers.Entities;
using eLibrary.ViewModels;
using eLibrary.BusinessLayer.Services.Interfaces;

namespace eLibrary.Controllers
{
    public class ClientsController : Controller
    {
        private readonly eLibraryDbContext _context;
        private readonly IUserService _userService;

        public ClientsController(eLibraryDbContext context,
            IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
              return _context.Clients != null ? 
                          View(await _context.Clients.ToListAsync()) :
                          Problem("Entity set 'eLibraryDbContext.Clients'  is null.");
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastnName,FullName,Email,Password,DataRegjistrimit,IsAdmin,ID,Created,Updated,Deleted")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        /// <summary>
        /// Return Register View
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Register()
        {
            return View();
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="userRegisterVM"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register([Bind("FirstName", "LastName", "Email", "Password")] UserRegisterVM userRegisterVM )
        {
            try
            {
                await _userService.RegisterUser(userRegisterVM);
                return View(userRegisterVM);
            }
            catch(Exception ex)
            {
                ViewBag.errorMessage = ex.Message;
                return View("Error");
            }
        }

        /// <summary>
        /// Log In User View
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LogIn()
        {
            return View();
        }

        /// <summary>
        /// Log In user
        /// </summary>
        /// <param name="logInVM"></param>
        /// <returns></returns>
        [HttpPost] 
        public async Task<IActionResult> LogIn([Bind("Email", "Password")] LogInVM logInVM)
        {
            try
            {
                var token = await _userService.LogIn(logInVM);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = ex.Message;
                return View("Error");
            }
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastnName,FullName,Email,Password,DataRegjistrimit,IsAdmin,ID,Created,Updated,Deleted")] Client client)
        {
            if (id != client.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'eLibraryDbContext.Clients'  is null.");
            }
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
          return (_context.Clients?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
