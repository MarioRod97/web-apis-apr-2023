using HrApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HrApi.Controllers;

public class DepartmentsController : ControllerBase
{
    [HttpGet("/departments")]
    public async Task<ActionResult<DepartmentResponse>> GetDepartments()
    {
        var response = new DepartmentResponse
        {
            Data = new List<DepartmentSummaryItem> {
                new DepartmentSummaryItem { Id="1", Name="Developers" },
                new DepartmentSummaryItem { Id="2", Name="Testers" }
            }
        };
        
        return Ok(response);
    }
}
