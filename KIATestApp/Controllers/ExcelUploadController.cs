using ExcelDataReader;
using KIATestApp.Interface;
using KIATestApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KIATestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelUploadController : ControllerBase
    {

        private readonly IEmployeeRepository _employeeRep;
        public ExcelUploadController(IEmployeeRepository employeeRepository)
        {
            _employeeRep = employeeRepository;
        }

        [HttpPost("upload", Name = "upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken )
        {
            //var file = Request.Form.Files[0];
            if (CheckIfExcelFile(file))
            {

                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                string fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                await WriteFile(file , path);
                DataTable dt = ReadExcelFile(path, extension);
                string msg = ValidateData(dt);
                _employeeRep.AddImportedEmployee(dt, "");
            }
            else
            {
                return BadRequest(new { message = "Invalid file extension" });
            }

            return Ok();
        }


        private bool CheckIfExcelFile(IFormFile file )
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension == ".xlsx" || extension == ".xls" || extension == ".csv");
        }


        private async Task<bool> WriteFile(IFormFile file  , string path  )
        {
            bool isSaveSuccess = false;
          
            try
            {
                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                isSaveSuccess = true;        
            }
            catch (Exception e)
            {
                //log error
            }

            return isSaveSuccess;
        }




        private DataTable ReadExcelFile(string filePath , string fileExtension )
        {

            DataTable dt = new DataTable();

            try
            {
                string excelConString = "";

                switch (fileExtension)
                {
                    //If uploaded file is Excel 1997-2007.
                    case ".xls":
                        excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                    //If uploaded file is Excel 2007 and above
                    case ".xlsx":
                        excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                }


                //Read data from first sheet of excel into datatable
           
                excelConString = string.Format(excelConString, filePath);

                using (OleDbConnection excelOledbConnection = new OleDbConnection(excelConString))
                {
                    using (OleDbCommand excelDbCommand = new OleDbCommand())
                    {
                        using (OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter())
                        {
                            excelDbCommand.Connection = excelOledbConnection;

                            excelOledbConnection.Open();
                            //Get schema from excel sheet
                            DataTable excelSchema = GetSchemaFromExcel(excelOledbConnection);
                            //Get sheet name
                            // string sheetName = "Sheet1";
                            string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();
                            excelOledbConnection.Close();

                            //Read Data from First Sheet.
                            excelOledbConnection.Open();
                            excelDbCommand.CommandText = "SELECT * From [" + sheetName + "]";
                            excelDataAdapter.SelectCommand = excelDbCommand;
                            //Fill datatable from adapter
                            excelDataAdapter.Fill(dt);
                            excelOledbConnection.Close();
                        }
                    }
                }
               
            }
            catch(Exception ex)
            {

            }

           return dt;
        }

        private static DataTable GetSchemaFromExcel(OleDbConnection excelOledbConnection)
        {
            return excelOledbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        }



      private  bool AreAllColumnsEmpty(DataRow dr)
        {
            if (dr == null)
            {
                return true;
            }
            else
            {
                foreach (var value in dr.ItemArray)
                {
                    if (value != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        private string ValidateData( DataTable dt)
        {
            try
            {
                string  msg = "";
                int skipcount = 0;
                int rowcount = 0;
                string skip = "";

                foreach (DataRow dr in dt.Rows)
                {
                    rowcount++;

                    if (AreAllColumnsEmpty(dr))
                    {
                        skipcount++;
                        skip = skip + " " + rowcount+ ","; 
                    }
                    else
                    {

                        if (String.IsNullOrEmpty(dr["FirstName"].ToString()))
                        {
                            throw new ValidationException("First Name Must Have value");
                        }

                        if (String.IsNullOrEmpty(dr["DOB"].ToString()))
                        {
                            throw new ValidationException("DOB must have value");
                        }


                        if (String.IsNullOrEmpty(dr["GENDER"].ToString()))
                        {
                            throw new ValidationException("GENDER ,ust have value");
                        }


                        if (String.IsNullOrEmpty(dr["SALARY"].ToString()))
                        {
                            throw new ValidationException("Salary must Have value");
                        }

                        if (String.IsNullOrEmpty(dr["DESIGNATION"].ToString()))
                        {
                            throw new ValidationException("Designation must have value");
                        }


                        bool result = int.TryParse(dr["SALARY"].ToString(), out var n);
                        if (!result)
                        {
                            throw new ValidationException("Salary must be decimal value");
                        }

                        bool result2 = DateTime.TryParse(dr["DOB"].ToString(), out var n1);
                        if (!result2)
                        {
                            throw new ValidationException("DOB must be Date value");
                        }
                    }


                }

                if (skipcount > 0)
                {
                    msg = skipcount + " rows [" + skip + " ]";             
                  
                 }
                return msg;

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

    }
    }

