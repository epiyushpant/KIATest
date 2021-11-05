using Dapper;
using KIATestApp.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public IDbConnection connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("mySqlConnection"));
            }
        }


        public async Task<List<Employee>> GetEmployees()
        {
            try
            {
          
                using (IDbConnection con = connection)
                {
                  
                    string Query = "GetEmployee";
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Id", null);
                    var result = await con.QueryAsync<Employee>(Query, param, commandType: CommandType.StoredProcedure);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<Employee> AddEmployee(Employee employee )
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    string sQuery = "InsertEmployee";
                    con.Open();

                
                    string[] dataURL = employee.FileSource.Split(new char[] { ',' }, 2);
                    byte[] file = Convert.FromBase64String(dataURL[1]);
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@FULLNAME", employee.FullName);
                    param.Add("@DOB", employee.DOB);
                    param.Add("@GENDER", employee.Gender);
                    param.Add("@SALARY", employee.Salary);
                    param.Add("@DESIGNATION", employee.Designation);
                    param.Add("@PHOTO", file);
                    var result = await con.QueryAsync<Employee>(sQuery, param, commandType: CommandType.StoredProcedure);          
                    return result.FirstOrDefault();


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

   


        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    string sQuery = "UpdateEmployee";
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Id", employee.Id);
                    param.Add("@FULLNAME", employee.FullName);
                    param.Add("@DOB", employee.DOB);
                    param.Add("@GENDER", employee.Gender);
                    param.Add("@SALARY", employee.Salary);
                    param.Add("@DESIGNATION", employee.Designation);

                    var result = await con.QueryAsync<Employee>(sQuery, param, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();

                 
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<Employee> GetEmployeesByID(int id)
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    string sQuery = "GetEmployee";
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Id", id);
                    var result = await con.QueryAsync<Employee>(sQuery, param, commandType: CommandType.StoredProcedure);

                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string AddImportedEmployee(DataTable dt , String ImportedBy)
        {
            try
            {

                foreach (DataRow dr in dt.Rows)
                {
                    using (IDbConnection con = connection)
                    {
                        string sQuery = "ImportEmployee";
                        con.Open();
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@FULLNAME", dr["FullName"].ToString());
                        param.Add("@DOB", dr["DOB"].ToString());
                        param.Add("@GENDER", dr["GENDER"].ToString());
                        param.Add("@SALARY", Convert.ToDouble(dr["SALARY"].ToString()));
                        param.Add("@DESIGNATION", dr["Designation"].ToString());
                        param.Add("@IMPORTEDBY",  "SuperAdmin" );

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


        public async Task<Employee> DeleteEmployee(int id)
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    string Query = "DeleteEmployee";
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Id", id);
                    var result = await con.QueryAsync<Employee>(Query, param, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
