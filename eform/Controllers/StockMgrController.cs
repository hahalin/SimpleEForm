using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eform.Attributes;
using System.IO;
using OfficeOpenXml;

namespace eform.Controllers
{
    public class StockMgrController : Controller
    {
        ApplicationDbContext ctx;
        public StockMgrController()
        {
            ctx = new ApplicationDbContext();
        }
        // GET: StockMgr
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Query()
        {
            List<stockItemInit> listA = ctx.stockItemInitList.OrderBy(x=>x.pdId).ThenBy(x=>x.sType).ToList<stockItemInit>();
            return View(listA);
        }
        decimal getDecimal(string v)
        {
            decimal r = 0;
            try
            {
                r = Convert.ToDecimal(v);
            }
            catch (Exception ex)
            {
                r = 0;
            }
            return r;
        }
        [Authorize]
        [HttpGet]
        public ActionResult UploadInit()
        {
            ViewBag.Title = "初始庫存上傳";
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadInit(HttpPostedFileBase file)
        {
            ViewBag.Title = "初始庫存上傳";

            if (file == null)
            {
                ModelState.AddModelError("", "上傳檔案失敗，請重新執行");
                return View();
            }

            if (file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string ext = Path.GetExtension(file.FileName).Replace(".", "").ToUpper();
                if (ext != "XLSX")
                {
                    ModelState.AddModelError("", "請選擇XLSX格式Excel檔案！");
                    return View();
                }

                var ctx = new ApplicationDbContext();

                string path = @"C:\inetpub\upload\stock";
                DirectoryInfo di = new DirectoryInfo(path);
                if (di.Exists == false)
                {
                    di.Create();
                }
                string uploadfile = path + @"\" + filename;
                if (System.IO.File.Exists(uploadfile))
                {
                    System.IO.File.Delete(uploadfile);
                }
                file.SaveAs(uploadfile);
                ExcelPackage pkg = new ExcelPackage(new FileInfo(uploadfile));
                ExcelWorkbook bk = pkg.Workbook;
                ExcelWorksheet sht = bk.Worksheets[1];

                if (ctx.stockItemInitList.Where(x=>x.sType=="1").Count()>0)
                {
                    ctx.stockItemInitList.RemoveRange(ctx.stockItemInitList.Where(x => x.sType == "1").ToList<stockItemInit>());
                    ctx.SaveChanges();
                }

                int startRow = 2;

                for (int itmp=1;itmp<=3;itmp++)
                {
                    if (sht.Cells[itmp,1].Text.Trim()== "公司別")
                    {
                        startRow = itmp+1;
                        break;
                    }
                }

                ctx.stockItemInitList.RemoveRange(ctx.stockItemInitList.Where(x => x.sType == "1").ToList<stockItemInit>());
                ctx.SaveChanges();
                    
                for (int i = startRow; i <= 65535; i++)
                {
                    if (sht.Cells[i, 2].Text.Trim() == "")
                    {
                        break;
                    }
                    stockItemInit model = new stockItemInit();
                    model.id = Guid.NewGuid().ToString();
                    model.sType = "1";
                    model.pdId = sht.Cells["B" + i.ToString()].Text.Trim();
                    model.pdNm = sht.Cells["C" + i.ToString()].Text.Trim();
                    model.spec = sht.Cells["D" + i.ToString()].Text.Trim();
                    model.dateA = sht.Cells["E" + i.ToString()].Text.Trim();
                    model.billNm = sht.Cells["F" + i.ToString()].Text.Trim();
                    model.billInNo = sht.Cells["G" + i.ToString()].Text.Trim();
                    model.InvNo = sht.Cells["H" + i.ToString()].Text.Trim();
                    model.brandNo = sht.Cells["X" + i.ToString()].Text.Trim();
                    model.supPdId = sht.Cells["Y" + i.ToString()].Text.Trim();
                    model.supId = sht.Cells["Z" + i.ToString()].Text.Trim();
                    model.supNm = sht.Cells["AA" + i.ToString()].Text.Trim();
                    model.smemoA = sht.Cells["AB" + i.ToString()].Text.Trim();
                    model.smemoB = sht.Cells["AC" + i.ToString()].Text.Trim();
                    model.qty =getDecimal(sht.Cells["AD" + i.ToString()].Text.Trim());
                    model.priceA = getDecimal(sht.Cells["AE" + i.ToString()].Text.Trim());
                    model.amtA = getDecimal(sht.Cells["AF" + i.ToString()].Text.Trim());
                    model.amtB = getDecimal(sht.Cells["AG" + i.ToString()].Text.Trim());
                    model.curId = sht.Cells["AH" + i.ToString()].Text.Trim();
                    model.curRate = getDecimal(sht.Cells["AI" + i.ToString()].Text.Trim());
                    model.priceB = getDecimal(sht.Cells["AJ" + i.ToString()].Text.Trim());
                    model.amtC = getDecimal(sht.Cells["AK" + i.ToString()].Text.Trim());
                    model.amtD = getDecimal(sht.Cells["AL" + i.ToString()].Text.Trim());
                    model.prjId = sht.Cells["AM" + i.ToString()].Text.Trim();
                    model.prjNm = sht.Cells["AN" + i.ToString()].Text.Trim();
                    model.reqId = sht.Cells["AT" + i.ToString()].Text.Trim();
                    model.reqId2 = sht.Cells["AU" + i.ToString()].Text.Trim();
                    model.dateB = sht.Cells["AV" + i.ToString()].Text.Trim();
                    model.filename = filename;
                    ctx.stockItemInitList.Add(model);
                    ctx.SaveChanges();
                }
                TempData["msg"] = "上傳完成";
                return View();
            }
            else
            {
                ModelState.AddModelError("", "上傳檔案失敗，請重新執行");
                return View();
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult UploadStockIn()
        {
            ViewBag.Title = "進銷明細單上傳";
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadStockIn(HttpPostedFileBase file)
        {
            ViewBag.Title = "進銷明細單上傳";

            if (file == null)
            {
                ModelState.AddModelError("", "上傳檔案失敗，請重新執行");
                return View();
            }

            if (file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string ext = Path.GetExtension(file.FileName).Replace(".", "").ToUpper();
                if (ext != "XLSX")
                {
                    ModelState.AddModelError("", "請選擇XLSX格式Excel檔案！");
                    return View();
                }

                var ctx = new ApplicationDbContext();

                string path = @"C:\inetpub\upload\stock";
                DirectoryInfo di = new DirectoryInfo(path);
                if (di.Exists == false)
                {
                    di.Create();
                }
                string uploadfile = path + @"\" + filename;
                if (System.IO.File.Exists(uploadfile))
                {
                    System.IO.File.Delete(uploadfile);
                }
                file.SaveAs(uploadfile);
                ExcelPackage pkg = new ExcelPackage(new FileInfo(uploadfile));
                ExcelWorkbook bk = pkg.Workbook;
                ExcelWorksheet sht = bk.Worksheets[1];

                if (ctx.stockItemInitList.Where(x=>x.filename==filename.ToUpper()).Count() > 0)
                {
                    ctx.stockItemInitList.RemoveRange(ctx.stockItemInitList.Where(x => x.filename == filename.ToUpper()).ToList<stockItemInit>());
                    ctx.SaveChanges();
                }

                int startRow = 2;

                for (int itmp = 1; itmp <= 3; itmp++)
                {
                    if (sht.Cells[itmp, 1].Text.Trim() == "公司別")
                    {
                        startRow = itmp + 1;
                        break;
                    }
                }
                List<stockItemInit> stockInList = new List<stockItemInit>();
                for (int i = startRow; i <= 65535; i++)
                {
                    if (sht.Cells[i, 2].Text.Trim() == "")
                    {
                        break;
                    }
                    stockItemInit model = new stockItemInit();
                    model.id = Guid.NewGuid().ToString();
                    model.sType = "2";
                    model.filename = filename.ToUpper();
                    model.pdId = sht.Cells["B" + i.ToString()].Text.Trim();
                    model.pdNm = sht.Cells["P" + i.ToString()].Text.Trim();
                    model.spec = sht.Cells["U" + i.ToString()].Text.Trim();
                    model.dateA = sht.Cells["D" + i.ToString()].Text.Trim();
                    model.billNm = sht.Cells["E" + i.ToString()].Text.Trim();
                    model.billInNo = sht.Cells["F" + i.ToString()].Text.Trim();
                    model.InvNo = sht.Cells["BL" + i.ToString()].Text.Trim();
                    model.brandNo = sht.Cells["T" + i.ToString()].Text.Trim();
                    model.supPdId = sht.Cells["I" + i.ToString()].Text.Trim();
                    model.supId = sht.Cells["A" + i.ToString()].Text.Trim();
                    model.supNm = sht.Cells["N" + i.ToString()].Text.Trim();
                    model.smemoA = sht.Cells["AF" + i.ToString()].Text.Trim();
                    model.smemoB = sht.Cells["AH" + i.ToString()].Text.Trim();
                    model.qty = getDecimal(sht.Cells["H" + i.ToString()].Text.Trim());
                    model.priceA = getDecimal(sht.Cells["J" + i.ToString()].Text.Trim());
                    model.amtA = getDecimal(sht.Cells["BH" + i.ToString()].Text.Trim());
                    model.amtB = getDecimal(sht.Cells["BI" + i.ToString()].Text.Trim());
                    model.curId = sht.Cells["Y" + i.ToString()].Text.Trim();
                    model.curRate = getDecimal(sht.Cells["Z" + i.ToString()].Text.Trim());
                    model.priceB = getDecimal(sht.Cells["AA" + i.ToString()].Text.Trim());
                    model.amtC = getDecimal(sht.Cells["BJ" + i.ToString()].Text.Trim());
                    model.amtD = getDecimal(sht.Cells["BK" + i.ToString()].Text.Trim());
                    model.prjId = sht.Cells["CE" + i.ToString()].Text.Trim();
                    model.prjNm = sht.Cells["CF" + i.ToString()].Text.Trim();
                    model.reqId = sht.Cells["BR" + i.ToString()].Text.Trim();
                    model.reqId2 = sht.Cells["AX" + i.ToString()].Text.Trim();
                    model.dateB = ""; //sht.Cells["" + i.ToString()].Text.Trim();
                    stockInList.Add(model);  
                }

                List<string> billInNoList = new List<string>();
                foreach (stockItemInit item in stockInList)
                {
                    if (billInNoList.Where(x=>x==item.billInNo).Count()==0)
                    {
                        billInNoList.Add(item.billInNo);
                    }
                }

                ctx.stockItemInitList.RemoveRange(ctx.stockItemInitList.Where(x => billInNoList.Contains(x.billInNo)).ToList<stockItemInit>());
                ctx.SaveChanges();
                foreach (stockItemInit item in stockInList)
                {
                    ctx.stockItemInitList.Add(item);
                    ctx.SaveChanges();
                }
                
                TempData["msg"] = "上傳完成";
                return View();
            }
            else
            {
                ModelState.AddModelError("", "上傳檔案失敗，請重新執行");
                return View();
            }

        }

    }
}