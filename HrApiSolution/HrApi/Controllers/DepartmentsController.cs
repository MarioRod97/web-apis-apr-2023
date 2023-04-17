using AutoMapper;
using AutoMapper.QueryableExtensions;
using HrApi.Domain;
using HrApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrApi.Controllers;

public class DepartmentsController : ControllerBase
{
    private readonly HrDataContext _context;
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _config;

    public DepartmentsController(HrDataContext context, IMapper mapper, MapperConfiguration config)
    {
        _context = context;
        _mapper = mapper;
        _config = config;
    }

    [HttpPost("/departments")]
    public async Task<ActionResult> AddADepartment([FromBody] DepartmentCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            // return 400 error code
            return BadRequest(ModelState);
        }

        var departmentToAdd = _mapper.Map<DepartmentEntity>(request);
        _context.Departments.Add(departmentToAdd);

        try
        {
            await _context.SaveChangesAsync();

            var response = _mapper.Map<DepartmentSummaryItem>(departmentToAdd);

            return Ok(response);
        } catch (DbUpdateException)
        {
            return BadRequest("That department exists.");
        }
    }

    [HttpGet("/departments")]
    public async Task<ActionResult<DepartmentResponse>> GetDepartments()
    {
        var response = new DepartmentResponse
        {
            Data = await _context.Departments
                                    .ProjectTo<DepartmentSummaryItem>(_config)
                                    .ToListAsync()
        };
        
        return Ok(response);
    }
}
