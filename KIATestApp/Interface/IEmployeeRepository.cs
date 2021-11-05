using KIATestApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace KIATestApp.Interface
{
  public  interface IEmployeeRepository
    {
        Task<Employee> AddEmployee(Employee employee);

        Task<Employee> UpdateEmployee(Employee employee);

        string AddImportedEmployee(DataTable dt, String ImportedBy);

        Task<List<Employee>> GetEmployees();

        Task<Employee> GetEmployeesByID(int id);

        Task<Employee> DeleteEmployee(int id);


    }
}
