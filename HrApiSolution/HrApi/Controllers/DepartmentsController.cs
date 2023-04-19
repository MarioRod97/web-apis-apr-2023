using AutoMapper;
using AutoMapper.QueryableExtensions;
using HrApi.Domain;
using HrApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HrApi.Controllers;

[Route("/departments")]
[Produces("application/json")]
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

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> UpdateDepartment(int id, [FromBody] DepartmentUpdateRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var savedThingy = await _context.GetActiveDepartments().SingleOrDefaultAsync(d => d.Id == id);

        if (savedThingy is null)
        {
            return NotFound();
        } else
        {
            savedThingy.Name = request.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    /// <summary>
    /// Use this to remove a department
    /// </summary>
    /// <param name="id">The id of the department to remove, duh.</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> RemoveDepartment(int id)
    {
        var department = await _context.GetActiveDepartments()
            .SingleOrDefaultAsync(d => d.Id == id);
        if (department != null)
        {
            department.Removed = true;
            await _context.SaveChangesAsync();
        }
        
        return NoContent();
    }

    [HttpPost()]
    [ResponseCache(Duration = 5, Location = ResponseCacheLocation.Any)]
    public async Task<ActionResult<DepartmentSummaryItem>> AddADepartment([FromBody] DepartmentCreateRequest request)
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

            return CreatedAtRoute("get-department-by-id", new { id = response.Id }, response);
        } catch (DbUpdateException)
        {
            return BadRequest("That department exists.");
        }
    }

    [HttpGet()]
    [ResponseCache(Duration = 5, Location = ResponseCacheLocation.Any)]
    public async Task<ActionResult<DepartmentsResponse>> GetDepartments(CancellationToken token)
    {
        var response = new DepartmentsResponse
        {
            Data = await _context.GetActiveDepartments()
                                    .ProjectTo<DepartmentSummaryItem>(_config)
                                    .ToListAsync(token)
        };
        
        return Ok(response);
    }

    /// <summary>
    /// Allows you to retreive a document with information about this departments
    /// </summary>
    /// <param name="id">The id of the departments</param>
    /// <returns>Either a 40 or some information about this.</returns>
    [HttpGet("/departments/{id:int}", Name = "get-department-by-id")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> GetDepartmentsById(int id)
    {
        var response = await _context.Departments
            .Where(dept => dept.Id == id)
            .ProjectTo<DepartmentSummaryItem>(_config)
            .SingleAsync();

        if(response is null)
        {
            return NotFound();
        } else
        {
            return Ok(response);
        }
    }
}
