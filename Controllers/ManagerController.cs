using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace postArticle.Controllers
{
    public class ManagerController : Controller
    {

        //初始化資料庫
        private healingForestEntities1 db = new healingForestEntities1();



        // GET: Manager
        public ActionResult UserMange(String search)
        {

            if (search != null)
            {
                var searchinput = from p in db.UserManage where p.UserName == search select p;
                return View("UserMange", searchinput);
            }

            else
            {
                var Name = db.UserManage.ToList().OrderBy(m=>m.UserName);
                return View(Name);
            }

        }

        public ActionResult Delete(int? id)
        {
                var ID = id;
                var p = db.UserManage.Find(ID);
                p.Status = "off";
                db.SaveChanges();
                var Name = db.UserManage.ToList();
                return View("UserMange", Name);
       
        }


        [HttpGet]
        public ActionResult Resize(int? id)
        {
            /*
            String ID = Convert.ToString(id);
            TempData["UserID"] = ID;
            ViewBag.UserID = TempData["UserID"];
            */

            UserManage emp = db.UserManage.Find(id);
            
            return View(emp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resize([Bind(Include ="UserID, UserName, Account, Password, Email, UserType, Status, Birthday")]UserManage emp)
        {

            if (ModelState.IsValid)
            {
                db.Entry(emp).State = EntityState.Modified;
                try { 
                    db.SaveChanges(); 
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors;
                    Response.Write(errorMessages);
                    throw ex;
                }
                

                return RedirectToAction("UserMange");
            }
            return View(emp);
        }
       

    }
}