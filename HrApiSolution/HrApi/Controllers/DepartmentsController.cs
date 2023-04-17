using HrApi.Domain;
using HrApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrApi.Controllers;

public class DepartmentsController : ControllerBase
{
    private readonly HrDataContext _context;

    public DepartmentsController(HrDataContext context)
    {
        _context = context;
    }

    [HttpPost("/departments")]
    public async Task<ActionResult> AddADepartment([FromBody] DepartmentCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            // return 400 error code
            return BadRequest(ModelState);
        }
        
        return Ok(request);
    }

    [HttpGet("/departments")]
    public async Task<ActionResult<DepartmentResponse>> GetDepartments()
    {
        var response = new DepartmentResponse
        {
            Data = await _context.Departments
                                    .Select(d =>
                                        new DepartmentSummaryItem
                                        {
                                            Id = d.Id.ToString(),
                                            Name = d.Name,
                                        })
                                    .ToListAsync()
        };
        
        return Ok(response);
    }
}
