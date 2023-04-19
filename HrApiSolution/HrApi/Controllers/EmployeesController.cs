using HrApi.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrApi.Controllers;

public class EmployeesController : ControllerBase
{
    private readonly HrDataContext _context;

    public EmployeesController(HrDataContext context)
    {
        _context = context;
    }

    [HttpGet("/employees")]
    public async Task<ActionResult> GetAllEmployees()
    {
        var results = await _context.Employees.ToListAsync(); // TODO: Don't send domain objects to the client!
        return Ok(new { Data = results });
    }
}