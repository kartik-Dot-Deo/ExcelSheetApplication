using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using System.Data.SqlClient;
using TestApplication.Models;

namespace TestApplication.Controllers
{
    public class HomeController : Controller
    {
        #region UploadFile

        public ActionResult TestForm()
        {
          

            return View();
        }
        [HttpPost]
        public ActionResult TestForm( HttpPostedFileBase FileUpload)
        {
            try
            {
                if (FileUpload != null)
                {
                    if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        string filename = FileUpload.FileName;

                        if (filename.EndsWith(".xlsx"))
                        {
                            string targetpath = Server.MapPath("~/DetailFormatInExcel/");
                            FileUpload.SaveAs(targetpath + filename);
                            string pathToExcelFile = targetpath + filename;

                            string sheetName = "Sheet1";
                            DataTable dt = READExcel(pathToExcelFile);
                            var foo = new EmailAddressAttribute();
                            dt.Columns.Add("RowValid", typeof(System.String));
                            //  var duplicates = dt.AsEnumerable().GroupBy(i => new { GSTIN = i.Field<string>("GSTIN"), Number = i.Field<string>("Contact Number"), Email = i.Field<string>("Contact Email") }).Where(g => g.Count() > 1).Select(g => new { g.Key.GSTIN, g.Key.Number,g.Key.Email }).ToList();
                            // dt.AsEnumerable().GroupBy(x => x["GSTIN"],z => z["Contact Email"],r => r["Contact Email"]).Where(x => x.Count() > 1);
                            foreach (DataRow dr in dt.Rows)
                            {
                                var gst = ValidateGST(dr["GSTIN"].ToString());
                                var mobileno = ValidateMobileNo(dr["Contact Number"].ToString());
                                var date = ValidateDate(dr["Start Date"].ToString(), dr["End Date"].ToString());
                                var Email = foo.IsValid(dr["Contact Email"].ToString());
                                var totalamount = ValidedAmount(dr["Trunover Amount"].ToString());

                                if (gst && mobileno && date && Email && totalamount)
                                {
                                    dr["RowValid"] = "valid";
                                }
                                else
                                {
                                    dr["RowValid"] = "error";
                                }
                            }
                            dt.Columns.Remove("Sr.No");

                            SqlParameter[] parm = new SqlParameter[2];// Set the nos of parameter
                            int i = 0;
                            parm[i++] = new SqlParameter("@StatementType", "Insert");// Add the parameter in a array
                            parm[i++] = new SqlParameter("@ttUploadedData", dt);// Add the parameter in a array
                            var retrunavl = DataAccess.ExecuteSPDataTable("SP_SaveDataToTable", parm);// Call the procedure ManageCheckDuplicaterows and return the Datatable
                            if (retrunavl.Rows.Count > 0)
                            {
                                ViewBag.Duplicate = retrunavl.Rows[0]["Duplicate"].ToString();
                                ViewBag.error = retrunavl.Rows[0]["error"].ToString();
                                ViewBag.valid = retrunavl.Rows[0]["valid"].ToString();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
            return View();
        }
        public bool ValidedAmount(string numer) {
            if (numer == null)
            {

                return false;
            }
            else {
                return Convert.ToDecimal(numer) < 0 ? false : true;
            }

        }
        public bool ValidateGST(string no)
        {

            var regex = @"^([0]{1}[1-9]{1}|[1-2]{1}[0-9]{1}|[3]{1}[0-7]{1})([a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ]{1}[0-9a-zA-Z]{1})+$";
            var match = Regex.Match(no, regex);

            
            return match.Success;
        }
        public bool ValidateMobileNo(string no)
        {
            var regex = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            var match = Regex.Match(no, regex, RegexOptions.IgnoreCase);


            return match.Success;
        }
        public bool ValidateDate(string Date1,string Date2)
        {
            var Dateone = DateTime.ParseExact(Date1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var DateTwo = DateTime.ParseExact(Date2, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            TimeSpan t = DateTwo.Subtract(Dateone);
            if (t.Days > 0)
            {
                return true;
            }
            else {
                return false;
            }

            
        }
        public DataTable READExcel(string path)
        {
            Microsoft.Office.Interop.Excel.Application objXL = null;
            Microsoft.Office.Interop.Excel.Workbook objWB = null;
            objXL = new Microsoft.Office.Interop.Excel.Application();
            objWB = objXL.Workbooks.Open(path);
            Microsoft.Office.Interop.Excel.Worksheet objSHT = objWB.Worksheets[1];

            int rows = objSHT.UsedRange.Rows.Count;
            int cols = objSHT.UsedRange.Columns.Count;
            DataTable dt = new DataTable();
            int noofrow = 1;

            for (int c = 1; c <= cols; c++)
            {
                string colname = objSHT.Cells[1, c].Text;
                dt.Columns.Add(colname);
                noofrow = 2;
            }

            for (int r = noofrow; r <= rows; r++)
            {
                DataRow dr = dt.NewRow();
                for (int c = 1; c <= cols; c++)
                {
                    dr[c - 1] = objSHT.Cells[r, c].Text;
                }

                dt.Rows.Add(dr);
            }

            objWB.Close();
            objXL.Quit();
            return dt;
        }

        #endregion UploadFile

        #region ViewFile

        public ActionResult UploadedDataList()
        {
            List<mdlRecord> LstmdlRecord = new List<mdlRecord>();
            DataTable _dt = new DataTable();
            try {
                SqlParameter[] parm = new SqlParameter[1];// Set the nos of parameter
                int i = 0;
                parm[i++] = new SqlParameter("@StatementType", "Select");// Add the parameter in a array
                _dt = DataAccess.ExecuteSPDataTable("SP_SaveDataToTable", parm);// Call the procedure ManageCheckDuplicaterows and return the Datatable

                if (_dt.Rows.Count != 0)
                {
                    foreach (DataRow dr in _dt.Rows)
                    {
                        LstmdlRecord.Add(new mdlRecord
                        {
                            PrimaryKey = Convert.ToInt32(dr["PrimaryKey"].ToString()),
                            CompanyName = dr["CompanyName"].ToString(),
                            GSTIN = dr["GSTIN"].ToString(),
                            StartDate = dr["StartDate"].ToString(),
                            EndDate = dr["EndDate"].ToString(),
                            TrunOverAmount = dr["TrunOverAmount"].ToString(),
                            ContactEmail = dr["ContactEmail"].ToString(),
                            ContactNumber = dr["ContactNumber"].ToString(),
                            RowValid = dr["RowValid"].ToString()

                        });

                    }
                }

            }
            catch (Exception ex) {

            }

            return View(LstmdlRecord);
        }
        #endregion ViewFile

        #region Saveline

        public JsonResult SaveLine(mdlRecord obj) {
            try
            {
                var foo = new EmailAddressAttribute();
                var gst = ValidateGST(obj.GSTIN);
                var mobileno = ValidateMobileNo(obj.ContactNumber);
                var date = ValidateDate(obj.StartDate, obj.EndDate);
                var Email = foo.IsValid(obj.ContactEmail);
                var totalamount = ValidedAmount(obj.TrunOverAmount);

                if (gst && mobileno && date && Email && totalamount)
                {
                    obj.RowValid = "valid";
                }
                else
                {
                    obj.RowValid = "error";
                }
                SqlParameter[] parm = new SqlParameter[10];// Set the nos of parameter
                int i = 0;
                parm[i++] = new SqlParameter("@StatementType", "InsertLine");// Add the parameter in a array
                parm[i++] = new SqlParameter("@CompanyName", obj.CompanyName);// Add the parameter in a array
                parm[i++] = new SqlParameter("@GSTIN", obj.GSTIN);// Add the parameter in a array
                parm[i++] = new SqlParameter("@StartDate", obj.StartDate);// Add the parameter in a array
                parm[i++] = new SqlParameter("@EndDate", obj.EndDate);// Add the parameter in a array
                parm[i++] = new SqlParameter("@TrunOverAmount", obj.TrunOverAmount);// Add the parameter in a array
                parm[i++] = new SqlParameter("@ContactEmail", obj.ContactEmail);// Add the parameter in a array
                parm[i++] = new SqlParameter("@ContactNumber", obj.ContactNumber);// Add the parameter in a array
                parm[i++] = new SqlParameter("@RowValid", obj.RowValid);// Add the parameter in a array
                parm[i++] = new SqlParameter("@PrimaryKey", obj.PrimaryKey);// Add the parameter in a array
                var retrunavl = DataAccess.ExecuteSPDataTable("SP_SaveDataToTable", parm);// Call the procedure ManageCheckDuplicaterows and return the Datatable
                
                return Json(retrunavl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Saveline
    }
}