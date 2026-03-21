using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstMVCApp.Dto;
using MyFirstMVCApp.Models;

namespace MyFirstMVCApp.Controllers
{
    [Authorize]
    public class DashboardController(AppDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            var list = _context.Employees.Select(x=> new EmployeeDto { EmployeeName = x.EmployeeName,
            Department = x.Department, Email = x.Email, Designation = x.Designation, Id = x.Id}).ToList();
            return View(list);
        }

        public IActionResult AddEmployee()
        {
            return View();
        }
             

        public IActionResult RedirectToEmployeeForm()
        {
            return RedirectToAction("AddEmployee");
        }

        public async Task<IActionResult> AddEmployeeDetail(EmployeeDto dto)
        {
            if(dto == null)
            {
                ViewBag.Message = "Please Fill All the Details in the form";
                return View("AddEmployee");
            }

            var isexist = _context.Employees.Any(e => e.Email == dto.Email);

            if(isexist)
            {
                ViewBag.Message = "Employee with the same email already exists.";
                return View("AddEmployee");
            }

            _context.Employees.Add(new Employee
            {
                EmployeeName = dto.EmployeeName,
                Department = dto.Department,
                Email = dto.Email,
                Designation = dto.Designation
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var data = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            _context.Employees.Remove(data);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> UpdateEmployee(int id)
        {
            var data = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

            return View(new EmployeeDto
            {
                Id = data.Id,
                EmployeeName = data.EmployeeName,
                Department = data.Department,
                Email = data.Email,
                Designation = data.Designation
            });

        }

        public async Task<IActionResult> UpdateEmployeeDetail(EmployeeDto dto)
        {
            
            if(dto == null)
            {
                ViewBag.Message = "Please Fill All the Details in the form";
                return View("UpdateEmployee");
            }

            var data = await _context.Employees.FirstOrDefaultAsync(x => x.Email == dto.Email);

            data.Email = dto.Email;
            data.EmployeeName = dto.EmployeeName;
            data.Designation = dto.Designation;
            data.Department = dto.Department;


            _context.Employees.Update(data);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
