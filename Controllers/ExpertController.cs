using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Net;
using postArticle.viewmodel;
using System.Data.Entity.Validation;

namespace postArticle.Controllers
{
    public class ExpertController : Controller
    {
        public healingForestEntities db = new healingForestEntities();
        public bool CheckLoggedIn() => Session["UserID"] != null;
        public int GetUserID() => Convert.ToInt32(Session["UserID"]);



        // GET: Expert
        public ActionResult Index()
        {
            int userID = GetUserID();

            if (Session["UserID"]!=null)
            {
                var expertApplies = db.ExpertApplies.Where(e => e.UserManage.UserID == userID).ToList();
                return View(expertApplies);
            }
            else
            {
                return RedirectToAction("Login", "UserManages");
            }

        }

        [HttpGet]
        public ActionResult Apply()
        {
            ViewBag.UserID = new SelectList(db.UserManages, "UserID", "UserName");

            return View();
        }
        private bool IsImageFile(HttpPostedFileBase file)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };

            return allowedExtensions.Contains(fileExtension.ToLower());
        }

        // POST: ExpertApplies/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(HttpPostedFileBase file, ExpertApply model, [Bind(Include = "ExpertApplyID,ExpertField,ExpertInfo,Status,UserID,ExpertImgURL,Remark")] ExpertApply expertApply)
        {
            if (CheckLoggedIn())
            {
                if (ModelState.IsValid)
                {
                    bool isApplicationSubmitted = CheckIfApplicationIsSubmitted(model);
                    ViewBag.IsApplicationSubmitted = isApplicationSubmitted;
                    if (file != null && file.ContentLength > 0)
                    {
                        if (IsImageFile(file))
                        {
                            // 獲取檔案名稱
                            string fileName = file.FileName;

                            model.UserID = (int)Session["UserID"];
                            // 將檔案名稱存入模型物件的對應欄位
                            model.ExpertImgURL = fileName;
                            string filePath = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                            file.SaveAs(filePath);


                            // 使用資料庫存取技術將模型物件保存到資料庫中

                            using (healingForestEntities dbContext = new healingForestEntities())
                            {
                                dbContext.ExpertApplies.Add(model);
                                UserManage user = dbContext.UserManages.FirstOrDefault(u => u.UserID == model.UserID);

                                dbContext.SaveChanges();
                            }

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.Message = "請上傳有效的圖片檔案";
                        }

                    }
                }



            }

            ViewBag.UserID = new SelectList(db.UserManages, "UserID", "UserName", expertApply.UserID);
            return View(expertApply);
        }

        private bool CheckIfApplicationIsSubmitted(ExpertApply model)
        {
            using (var context = new healingForestEntities())
            {

                var existingApplication = context.ExpertApplies
                    .FirstOrDefault(a => a.UserID == model.UserID && model.Status == "審核中");

                return existingApplication != null;
            }

        }

        // GET: ExpertApplies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExpertApply expertApply = db.ExpertApplies.Find(id);
            if (expertApply == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.UserManages, "UserID", "UserName", expertApply.UserID);

            return View(expertApply);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExpertApplyID,ExpertField,ExpertInfo,Status,UserID,ExpertImgURL,Remark")] ExpertApply expertApply, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {

                if (file != null && file.ContentLength > 0)
                {
                    if (IsImageFile(file))
                    {
                        // 獲取檔案名稱
                        string fileName = file.FileName;

                        expertApply.UserID = (int)Session["UserID"];
                        // 將檔案名稱存入模型物件的對應欄位
                        expertApply.ExpertImgURL = fileName;
                        db.Entry(expertApply).State = EntityState.Modified;
                        db.SaveChanges();
                        string filePath = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                        file.SaveAs(filePath);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "請上傳有效的圖片檔案";
                    }
                }



                return RedirectToAction("Index");
            }





            return View(expertApply);
        }

    }
}