using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETCore5.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ASP.NETCore5.Controllers
{
    public class EmployeeController : Controller
    {
        private UdemyDBContext _context;
        public EmployeeController(UdemyDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Employees);
        }
        public IActionResult Paging(int currentPageIndex)
        {
            if (currentPageIndex <= 0) currentPageIndex = 1;

            return View(GetEmployees(currentPageIndex));
        }
        private EmployeeView GetEmployees(int currentPage)
        {
            int nRows = 10;
            EmployeeView vw = new EmployeeView();

            vw.Employees = (from emp in this._context.Employees
                            select emp)
                            .OrderBy(emp => emp.Id)
                            .Skip((currentPage - 1) * nRows)
                            .Take(nRows).ToList();

            double pageCount = (double)((decimal)this._context.Employees.Count() /Convert.ToDecimal(nRows));
            vw.PageCount = (int)Math.Ceiling(pageCount);
            vw.CurrentPageIndex = currentPage;

            return vw;
        }
        public IActionResult SortingPaging(EmployeeSortPagingView model)
        {
            if (model == null)
            {
                model = new EmployeeSortPagingView();
                model.CurrentPageIndex = 1;
                model.SortField = "Id";
                model.SortOrder = "ASC";
                model.CurrentSortField = "Id";
                model.CurrentSortOrder = "ASC";
            }
            else
            {
                if (model.CurrentPageIndex <= 0)
                    model.CurrentPageIndex = 1;
                if (string.IsNullOrEmpty(model.SortField))
                    model.SortField = "Id";

                // change ASC/DESC if the field name is similiar
                if (model.SortField == model.CurrentSortField)
                {
                    if (model.CurrentSortOrder == "ASC")
                        model.SortOrder = "DESC";
                    else
                        model.SortOrder = "ASC";
                }
                else
                {
                    if(string.IsNullOrEmpty(model.SortOrder))
                    model.SortOrder = "ASC";
                }

            }
            ViewBag.CurrentPageIndex = model.CurrentPageIndex;
            ViewBag.CurrentSortField = model.CurrentSortField;
            ViewBag.CurrentSortOrder = model.CurrentSortOrder;

            return View(GetEmployeesSortingPaging(model.CurrentPageIndex,
                model.SortField, model.SortOrder));
        }
        private EmployeeSortPagingView GetEmployeesSortingPaging(int currentPage,
            string field, string order)
        {
            int nRows = 10;
            EmployeeSortPagingView vw = new EmployeeSortPagingView();

            var propertyInfo = typeof(Employee).GetProperty(field);

            if (order == "ASC")
            {
                vw.Employees = (from emp in this._context.Employees
                                select emp).ToList()
                                .OrderBy(emp => propertyInfo.GetValue(emp, null))
                                .Skip((currentPage - 1) * nRows)
                                .Take(nRows).ToList();
            }
            else
            {
                vw.Employees = (from emp in this._context.Employees
                                select emp).ToList()
                                .OrderByDescending(emp => propertyInfo.GetValue(emp, null))
                                .Skip((currentPage - 1) * nRows)
                                .Take(nRows).ToList();
            }

            double pageCount = (double)((decimal)this._context.Employees.Count() / Convert.ToDecimal(nRows));
            vw.PageCount = (int)Math.Ceiling(pageCount);
            vw.CurrentPageIndex = currentPage;
            vw.SortField = field;
            vw.SortOrder = order;
            return vw;
        }
        public IActionResult GenerateData()
        {
            for (int i = 0; i < 12; i++)
            {
                Employee o = new Employee();
                o.FirstName = Guid.NewGuid().ToString().Substring(0, 5);
                o.LastName = Guid.NewGuid().ToString().Substring(0, 5);
                o.Email = o.FirstName + "@email.com";
                o.Age = 10 + 2 * i;
                o.Created = DateTime.Now;

                this._context.Employees.Add(o);
            }
            this._context.SaveChanges();
            return RedirectToAction("Paging");
        }
        public IActionResult Create()
        {
            return View(new Employee());
        }
        [HttpPost]
        public IActionResult Create([FromForm]Employee model)
        {
            model.Created = DateTime.Now;
            _context.Employees.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            var employee = _context.Employees.Where(o=>o.Id == id).FirstOrDefault();

            return View(employee);
        }
        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.Where(o => o.Id == id).FirstOrDefault();

            return View(employee);
        }
        [HttpPost]
        public IActionResult Edit(int id, [FromForm] Employee model)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.Where(o => o.Id == id).FirstOrDefault();

            return View(employee);
        }
        [HttpPost]
        public IActionResult Delete(int id, string btn)
        {
            //var employee = _context.Employees.Where(o => o.Id == id).FirstOrDefault();
            //if (btn=="Delete")  
            //{
            //    _context.Employees.Remove(employee);
            //    _context.SaveChanges();
            //    return RedirectToAction("Index");

            //}
            //return View(employee);
            if (btn == "Delete")
            {
                using var transaction = _context.Database.BeginTransaction();
                    try
                    {
                        var empPict = this._context.EmployeePictures.Where(a => a.EmpId == id).FirstOrDefault();
                        if (empPict != null)
                        {
                            this._context.EmployeePictures.Remove(empPict);
                            this._context.SaveChanges();
                        }

                        var emp = this._context.Employees.Where(a => a.Id == id).FirstOrDefault();
                        this._context.Employees.Remove(emp);
                        this._context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
            }

            return RedirectToAction("List");
        }
        public IActionResult EditPicture(int id)
        {
            ViewBag.ImageUrl = null;
            var empPict = this._context.EmployeePictures.Where(a => a.EmpId == id).FirstOrDefault();
            if (empPict == null)
            {
                empPict = new EmployeePicture { EmpId = id };
            }
            else
            {
                string imgString = Convert.ToBase64String(empPict.ImageData);
                string imageDataURL = string.Format("data:{0};base64,{1}", empPict.ImageType, imgString);
                ViewBag.ImageUrl = imageDataURL;
            }

            return View(empPict);
        }

        [HttpPost]
        public async Task<IActionResult> EditPicture([FromForm] EmployeePicture model, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (files.Count > 0)
                {
                    var formFile = files[0];
                    MemoryStream ms = new MemoryStream();
                    await formFile.CopyToAsync(ms);

                    model.ImageData = ms.ToArray();
                    model.ImageType = formFile.ContentType;
                }

                model.Created = DateTime.Now;

                var empPict = this._context.EmployeePictures.Where(a => a.EmpId == model.EmpId).FirstOrDefault();

                if (empPict == null)
                {
                    this._context.EmployeePictures.Add(model);
                    this._context.SaveChanges();
                }
                else
                {
                    empPict.ImageData = model.ImageData;
                    empPict.ImageType = model.ImageType;
                    empPict.Created = DateTime.Now;
                    _context.EmployeePictures.Update(empPict); 
                    this._context.SaveChanges();
                }

                return RedirectToAction("ImageDetail", new { id = model.EmpId});
            }
            return View(model);
        }

        public IActionResult ImageDetail(int id)
        {
            ViewBag.ImageUrl = null;
            var empPict = this._context.EmployeePictures.Where(a => a.EmpId == id).FirstOrDefault();

            if (empPict != null)
            {
                string imgString = Convert.ToBase64String(empPict.ImageData);
                string imageDataURL = string.Format("data:{0};base64,{1}", empPict.ImageType, imgString);

                ViewBag.ImageUrl = imageDataURL;
            }

            var emp = this._context.Employees.Where(a => a.Id == id).FirstOrDefault();

            return View(emp);
        }

        public IActionResult ErrorHandling()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ErrorHandling(string val)
        {
            try
            {
                int total = Convert.ToInt32(val) * 10;
                ViewBag.Result = total.ToString();
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }
    }
}
