using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KIATestApp.Interface;
using KIATestApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KIATestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class EmployeeController : ControllerBase
    {

        private readonly IEmployeeRepository _employeeRep;
        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRep = employeeRepository;
        }


        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetEmployee()
        {

            List<Employee> a  =  await _employeeRep.GetEmployees();
            foreach (Employee emp in a)
            {
                if (emp.Photo != null)
                {
                    string base64String = Convert.ToBase64String(emp.Photo, 0, emp.Photo.Length);
                    emp.FileSource = "data:image/png;base64," + base64String;
                }
              
            }
            return a;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetByCustomerByID(int id)
        {
            return await _employeeRep.GetEmployeesByID(id);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpPost]
         public async Task<ActionResult<Employee>> AddEmployee([FromBody] Employee employee  )
        {

            if (employee == null || !ModelState.IsValid)
            { 
            
                return BadRequest("Invalid State");
            }  
             return await _employeeRep.AddEmployee(employee);

          

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee([FromBody] Employee employee)
        {

            if (employee == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid State");
            }

            return await _employeeRep.UpdateEmployee(employee);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> DeleteById(int id)
        {
            return await _employeeRep.DeleteEmployee(id);

        }
    }
}
