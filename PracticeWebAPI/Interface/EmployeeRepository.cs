using Dapper;
using KIATestApp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace KIATestApp.Interface
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IConfiguration _config;

        public EmployeeRepository(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection => new SqlConnection(_config.GetConnectionString("mssqlConnection"));

        public async Task<List<Employee>> GetEmployees()
        {
            try
            {
                using (IDbConnection con = Connection)
                {
                    string query = "GetEmployees";
                    con.Open();
                    var result = await con.QueryAsync<Employee>(query, commandType: CommandType.StoredProcedure);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Employee> AddEmployee(Employee employee)
        {
            try
            {
                using (IDbConnection con = Connection)
                {
                    string query = "InsertEmployee";
                    con.Open();

                    //string[] dataURL = employee.FileSource?.Split(',', 2);
                    //byte[] file = Convert.FromBase64String(dataUrl.length ? dataURL[1] : null );

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@FULLNAME", employee.FullName);
                    param.Add("@DOB", employee.DOB);
                    param.Add("@GENDER", employee.Gender);
                    param.Add("@SALARY", employee.Salary);
                    param.Add("@DESIGNATION", employee.Designation);
                    param.Add("@PHOTO", (object)DBNull.Value, DbType.Binary);

                    var result = await con.QueryAsync<Employee>(query, param, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            try
            {
                using (IDbConnection con = Connection)
                {
                    string query = "UpdateEmployee";
                    con.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Id", employee.Id);
                    param.Add("@FULLNAME", employee.FullName);
                    param.Add("@DOB", employee.DOB);
                    param.Add("@GENDER", employee.Gender);
                    param.Add("@SALARY", employee.Salary);
                    param.Add("@DESIGNATION", employee.Designation);

                    var result = await con.QueryAsync<Employee>(query, param, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Employee> GetEmployeesByID(int id)
        {
            try
            {
                using (IDbConnection con = Connection)
                {
                    string query = "GetEmployeeById";
                    con.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Id", id);

                    var result = await con.QueryAsync<Employee>(query, param, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> AddImportedEmployee(DataTable dt, string importedBy)
        {
            try
            {
                using (IDbConnection con = Connection)
                {
                    con.Open();

                    foreach (DataRow dr in dt.Rows)
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@FULLNAME", dr["FullName"].ToString());
                        param.Add("@DOB", dr["DOB"].ToString());
                        param.Add("@GENDER", dr["GENDER"].ToString());
                        param.Add("@SALARY", Convert.ToDouble(dr["SALARY"].ToString()));
                        param.Add("@DESIGNATION", dr["Designation"].ToString());
                        param.Add("@IMPORTEDBY", importedBy);

                        await con.ExecuteAsync("ImportEmployee", param, commandType: CommandType.StoredProcedure);
                    }
                }

                return "Successful";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Employee> DeleteEmployee(int id)
        {
            try
            {
                using (IDbConnection con = Connection)
                {
                    string query = "DeleteEmployee";
                    con.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Id", id);

                    var result = await con.QueryAsync<Employee>(query, param, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        string IEmployeeRepository.AddImportedEmployee(DataTable dt, string ImportedBy)
        {

            try
            {

                foreach (DataRow dr in dt.Rows)
                {
                    using (IDbConnection con = Connection)
                    {
                        string sQuery = "ImportEmployee";
                        con.Open();
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@FULLNAME", dr["FullName"].ToString());
                        param.Add("@DOB", dr["DOB"].ToString());
                        param.Add("@GENDER", dr["GENDER"].ToString());
                        param.Add("@SALARY", Convert.ToDouble(dr["SALARY"].ToString()));
                        param.Add("@DESIGNATION", dr["Designation"].ToString());
                        param.Add("@IMPORTEDBY", "SuperAdmin");

                        var result = con.QueryAsync<Employee>(sQuery, param, commandType: CommandType.StoredProcedure);


                    }

                }

                return "Successful";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
