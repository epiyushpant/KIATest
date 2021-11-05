using KIATestApp.Interface;
using KIATestApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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


        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetByCustomerByID(int id)
        {
            return await _employeeRep.GetEmployeesByID(id);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        // POST api/<EmployeeController>
        [HttpPost]
         public async Task<ActionResult<Employee>> AddEmployee([FromBody] Employee employee  )
        {

            if (employee == null || !ModelState.IsValid)
            { 
            
                return BadRequest("Invalid State");
            }  
             return await _employeeRep.AddEmployee(employee);

          

        }


        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee([FromBody] Employee employee)
        {

            if (employee == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid State");
            }

            return await _employeeRep.UpdateEmployee(employee);

        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> DeleteById(int id)
        {
            return await _employeeRep.DeleteEmployee(id);

        }
    }
}
