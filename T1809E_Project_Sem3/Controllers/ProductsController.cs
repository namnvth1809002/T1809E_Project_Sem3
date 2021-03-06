﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using T1809E_Project_Sem3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace T1809E_Project_Sem3.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products

        public ActionResult Index(string sortOrder, int? status, DateTime? start, DateTime? end, string searchString, string currentFilter , int? page)


        {
            var products = db.Products.Include(p => p.category).Include(p => p.CreateBy).Include(p => p.DeleteBy).Include(p => p.UpdateBy);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));
            }
            if (status.HasValue)
            {
                ViewBag.Status = status;
                products = products.Where(p => (int)p.Status == status.Value);

            }
            if (start != null)
            {
                var startDate = start.GetValueOrDefault().Date;
                startDate = startDate.Date + new TimeSpan(0, 0, 0);
                products = products.Where(p => p.CreateAt >= startDate);
            }
            if (end != null)
            {
                var endDate = end.GetValueOrDefault().Date;
                endDate = endDate.Date + new TimeSpan(23, 59, 59);
                products = products.Where(p => p.CreateAt <= endDate);
            }
            if (string.IsNullOrEmpty(sortOrder) || sortOrder.Equals("date-asc"))

            {
                ViewBag.DateSort = "date-desc";
                ViewBag.PriceSort = "price-desc";
                ViewBag.NameSort = "name-desc";
                ViewBag.DiscountSort = "discount-desc";
                ViewBag.SortIcon = "fa fa-sort-asc";
            }
            else if (sortOrder.Equals("date-desc"))
            {
                ViewBag.DateSort = "date-asc";
                ViewBag.SortIcon = "fa fa-sort-desc";
            }
            else if (sortOrder.Equals("price-asc"))
            {
                ViewBag.PriceSort = "price-desc";
                ViewBag.SortIcon = "fa fa-sort-asc";
            }
            else if (sortOrder.Equals("price-desc"))
            {
                ViewBag.PriceSort = "price-asc";
                ViewBag.SortIcon = "fa fa-sort-desc";
            }
            else if (sortOrder.Equals("name-asc"))
            {
                ViewBag.NameSort = "name-desc";
                ViewBag.SortIcon = "fa fa-sort-asc";
            }
            else if (sortOrder.Equals("name-desc"))
            {
                ViewBag.NameSort = "name-asc";
                ViewBag.SortIcon = "fa fa-sort-desc";
            }
            else if (sortOrder.Equals("discount-asc"))
            {
                ViewBag.DiscountSort = "discount-desc";
                ViewBag.SortIcon = "fa fa-sort-asc";
            }
            else if (sortOrder.Equals("discount-desc"))
            {
                ViewBag.DiscountSort = "discount-asc";
                ViewBag.SortIcon = "fa fa-sort-desc";
            }

            switch (sortOrder)
            {
                case "name-asc":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "name-desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "price-asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "date-asc":
                    products = products.OrderBy(p => p.CreateAt);
                    break;
                case "date-desc":
                    products = products.OrderByDescending(p => p.CreateAt);
                    break;
                case "discount-asc":
                    products = products.OrderBy(p => p.Discount);
                    break;
                case "discount-desc":
                    products = products.OrderByDescending(p => p.Discount);
                    break;
                default:
                    products = products.OrderByDescending(p => p.CreateAt);
                    ViewBag.SortIcon = "fa fa-sort";
                    break;
            }



            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name");
            ViewBag.CreateById = new SelectList(db.Users, "Id", "Address");
            ViewBag.DeleteById = new SelectList(db.Users, "Id", "Address");
            ViewBag.UpdateById = new SelectList(db.Users, "Id", "Address");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "Id,Name,Description,Thumbnails,Price,Discount,CreateAt,CategoryID,CreateById,UpdateById,DeleteById")] Product product, string[] thumbnails)

        {
            if (ModelState.IsValid)
            {
                if (thumbnails != null && thumbnails.Length > 0)
                {
                    product.Thumbnails = string.Join(",", thumbnails);
                }
                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = "Create";
                return RedirectToAction("Index");
            }
            else { TempData["message"] = "Fail"; }

            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name", product.CategoryID);
            ViewBag.CreateById = new SelectList(db.Users, "Id", "Address", product.CreateById);
            ViewBag.DeleteById = new SelectList(db.Users, "Id", "Address", product.DeleteById);
            ViewBag.UpdateById = new SelectList(db.Users, "Id", "Address", product.UpdateById);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name", product.CategoryID);
            ViewBag.CreateById = new SelectList(db.Users, "Id", "Address", product.CreateById);
            ViewBag.DeleteById = new SelectList(db.Users, "Id", "Address", product.DeleteById);
            ViewBag.UpdateById = new SelectList(db.Users, "Id", "Address", product.UpdateById);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit([Bind(Include = "Id,Name,Description,Thumbnails,Price,Discount,Status,CreateAt,CategoryID,CreateById,UpdateById,DeleteById")] Product product, string[] thumbnails)
        {
            if (ModelState.IsValid)
            {
                if (thumbnails != null && thumbnails.Length > 0)
                {
                    product.Thumbnails = string.Join(",", thumbnails);
                }
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                TempData["message"] = "Edit";
                return RedirectToAction("Index");
            }
            else { TempData["message"] = "Fail"; }

            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name", product.CategoryID);
            ViewBag.CreateById = new SelectList(db.Users, "Id", "Address", product.CreateById);
            ViewBag.DeleteById = new SelectList(db.Users, "Id", "Address", product.DeleteById);
            ViewBag.UpdateById = new SelectList(db.Users, "Id", "Address", product.UpdateById);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            db.Products.Remove(product);
            db.SaveChanges();
            TempData["message"] = "Delete";
            return RedirectToAction("Index");
        }

        // POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Product product = db.Products.Find(id);
        //    db.Products.Remove(product);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
