﻿using _Platform.Service;
using postArticle.Models;
using postArticle.viewmodel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace postArticle.Controllers
{
    public class UserManagesController : Controller
    {
        #region 基礎屬性
        private healingForestEntities db = new healingForestEntities();

        MemberDetailsViewModel UserManage = new MemberDetailsViewModel();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;
        public int GetUserID() => Convert.ToInt32(Session["UserID"]);
        #endregion

        public ActionResult Login()
        {
            return View();
        }


        //登入
        [HttpPost]
        public ActionResult Login(RegisterViewModel registerViewModel)
        {

            if (IsValidUser(registerViewModel.userManage.Account, registerViewModel.userManage.Password))
            {


                var querySQL = from UserManagedb in db.UserManages
                               where UserManagedb.Password == registerViewModel.userManage.Password && UserManagedb.Account == registerViewModel.userManage.Account
                               select new
                               {
                                   UserManagedb.UserID,
                                   UserManagedb.Status
                               };

                #region ===如果有進入首頁===

                if (querySQL.Any() && querySQL.First().Status == 0)
                {
                    var user = querySQL.FirstOrDefault();
                    Session["UserID"] = user.UserID;

                    return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
                }
                #endregion
                else
                {
                    ViewBag.UserStatus = "封鎖";
                    return View();
                }
            }
            else
            {


                var queryAccountSQL = from UserManagedb in db.UserManages
                                      where UserManagedb.Account == registerViewModel.userManage.Account
                                      select new
                                      {
                                          UserManagedb.UserID
                                      };

                if (queryAccountSQL.Any())
                {
                    ModelState.AddModelError("userManage.Password", "密碼錯誤");
                    ViewBag.InvalidPassword = true;
                    return View();
                }
                else
                {
                    ModelState.AddModelError("userManage.Account", "無此帳號");
                    ViewBag.InvalidPassword = true;
                    return View();
                }
            }

        }

        private bool IsValidUser(string account, string password)
        {
            UserManage user = db.UserManages.FirstOrDefault(u => u.Account == account);

            if (user != null)
            {
                // 验证密码是否匹配
                if (user.Password == password)
                {
                    return true;
                }
            }

            return false;
        }

        // 發送驗證碼
        [HttpPost]
        public ActionResult SendVerificationCode(string email)
        {
            MaillService ms = new MaillService();
            Session["UserEmail"] = email;
            try
            {
                string subject = "註冊驗證碼";
                var verificationCode = GenerateRandomCode(4);
                string body = @"
                        <p>歡迎來到療癒之森!您的驗證碼為:{verificationCode}<br />
                            療癒之森是一個以撫慰人心為出發點，由一群大學生開發的網站!<br />
                           在這裡，你可以盡情地與各式各樣的人進行談心，結交更多志同道合的麻吉!<br />
                        
                           <br />
                            <br />

                            我們的客服信箱為<p>HealingForestWeb@gmail.com</p>
                            若有任何問題，歡迎與我們聯繫。
                            最後，祝您使用愉快! 療癒之森開發團隊<br />

                        ==================================<br />
                        此為系統自動回覆之信件，若有問題可以聯繫開發團隊<br />
                        ==================================<br />
                        </p>
                        ";
                body = body.Replace("{verificationCode}", verificationCode);
                ms.SendMail(email, subject, body);
                Session["VerificationCode"] = verificationCode;
            }
            catch (Exception)
            {
                // 发送失败的处理逻辑
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }



        //password
        private string GenerateRandomCode(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            return new string(code);
        }


        // GET: UserManages/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.ThanksfulThings, "UserID", "Content");
            return View();
        }
        // POST: UserManages/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterViewModel registerViewModel)
        {
            if (registerViewModel != null && registerViewModel.userManage.Password.Equals(registerViewModel.ConfirmPassword))
            {
                // 验证验证码是否正确
                var sessionVerificationCode = Session["VerificationCode"] as string;
                if (registerViewModel.code == sessionVerificationCode)
                {
                    // 密码匹配，继续保存用户
                    // 添加您的保存用户逻辑

                    registerViewModel.userManage.Experience = 0.0;
                    registerViewModel.userManage.LevelValue = 0;
                    registerViewModel.userManage.UserType = "Member";
                    registerViewModel.userManage.Status = 0;
                    registerViewModel.userManage.Email = (string)Session["UserEmail"];


                    db.UserManages.Add(registerViewModel.userManage);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
            }


            return View();
        }


        public ActionResult forget_password()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult forget_password(string account, string email)
        {
            if (IsValidPassword(account, email))
            {
                MaillService ms = new MaillService();
                try
                {
                    UserManage user = db.UserManages.FirstOrDefault(u => u.Account == account);
                    string subject = "忘記密碼";
                    var RandomPassword = GenerateRandomCode(8);
                    // 更新會員密碼為臨時密碼
                    user.Password = RandomPassword;
                    db.SaveChanges(); // 保存變更
                    string body = @"
                <p>您的臨時密碼為: {RandomPassword}<br />
                    登入後請記得至會員資料處進行密碼修改! <br />              
                    若有任何問題，歡迎與我們聯繫。<br />
                    我們的客服信箱為<p>healingforestweb@gmail.com<p/>
                    最後，祝您使用愉快! 療癒之森開發團隊</p>
                ===============================<br />
                此為系統自動回覆之信件，若有問題可以聯繫開發團隊。<br />
                ===============================<br />
                ";
                    /* body = body.Replace("{Password}", user.Password);
                     ms.SendMail(email, subject, body);*/

                    body = body.Replace("{RandomPassword}", RandomPassword);
                    ms.SendMail(email, subject, body);
                    Session["RandomPassword"] = RandomPassword;

                }
                catch (Exception)
                {
                    // 发送失败的处理逻辑
                    return Json(new { success = false, message = "發送錯誤" });
                }
                return RedirectToAction("login");
            }
            else
            {
                ModelState.AddModelError("Email", "帳號與Email並不匹配!");
                return View();
            }
        }
        private bool IsValidPassword(string account, string email)
        {
            UserManage user = db.UserManages.FirstOrDefault(u => u.Account == account);

            if (user != null)
            {
                // Email是否匹配
                if (user.Email == email)
                {
                    return true;
                }
            }

            return false;
        }

        [HttpGet]
        public ActionResult ApplyExpert()
        {
            return View();
        }



        //限制暱稱不重複
        public JsonResult IsUserNameUnique(string username)
        {
            bool isUnique = !db.UserManages.Any(u => u.UserName == username);
            return Json(isUnique, JsonRequestBehavior.AllowGet);
        }

        //限制帳號不重複
        public JsonResult IsAccountUnique(string account)
        {
            bool isUnique = !db.UserManages.Any(u => u.Account == account);
            return Json(isUnique, JsonRequestBehavior.AllowGet);
        }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        // GET: UserManages/Details/5
        public ActionResult MemberDetails(int? id)
        {

            var UserID = GetUserID();

            moodcount();


            if (id == null)
            {
                return HttpNotFound();
            }

            //ViewModel
            var userQuestionUserIDs = db.UserQuestions.Where(m => m.UserID == UserID).Select(m => m.UserQuestionID).ToList();
            var expertAnswers = db.ExpertAnswers.Where(m => userQuestionUserIDs.Contains(m.QuestionID)).ToList();


            /////////////////////////////////
            UserManage.ExpertAS = expertAnswers;
            UserManage.UserManagesDetail = db.UserManages.Find(id);
            UserManage.UserQuestions = db.UserQuestions.Where(m => m.UserID == UserID);


            ////////////////////////////////
            var record = db.Moods.Where(m => m.UserID == UserID).ToList().Where(m => m.Time.ToString("M") == DateTime.Now.ToString("M")).ToList();


            if (record.Count > 0) {

                UserManage.nowmood = record.First().Mood1;

            }
           




            if (UserManage.UserManagesDetail == null)
            {
                return HttpNotFound();
            }
            return View(UserManage);
        }


        //////////////////////////////////////////////////////////////////////////////////////

        public ActionResult MemberEdit(int? id)
        {

            int UserID = GetUserID();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserManage userManage = db.UserManages.Find(id);

            if (userManage == null)
            {

                return HttpNotFound();
            }

            if (id == UserID)
            {
                return View(userManage);
            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberEdit(UserManage userManage)
        {
            if (ModelState.IsValid)
            {

                using (healingForestEntities db = new healingForestEntities())
                {

                    UserManage existingUser = db.UserManages.Find(userManage.UserID);

                    if (existingUser != null)
                    {
                        existingUser.UserName = userManage.UserName;
                        existingUser.Account = userManage.Account;
                        existingUser.Email = userManage.Email;
                        existingUser.Birthday = userManage.Birthday;

                        db.SaveChanges();
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }

                return RedirectToAction("MemberDetails", new { id = userManage.UserID });
            }

            return View(userManage);
        }



        /////////////////////////////////////////////////////////////////////////////////////
        public ActionResult EditPassword(int? id)
        {

            int UserID = GetUserID();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserManage user = db.UserManages.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            if (id == UserID)
            {
                EditPasswordViewModel viewModel = new EditPasswordViewModel();
                viewModel.userManage = user;
                return View(viewModel);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPassword(EditPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                UserManage existingUser = db.UserManages.Find(viewModel.userManage.UserID);

                if (existingUser != null)
                {
                    if (existingUser.Password == viewModel.OldPassword)
                    {
                        if (viewModel.NewPassword == viewModel.ConfirmPassword)
                        {
                            existingUser.Password = viewModel.NewPassword;
                            db.SaveChanges();
                            return RedirectToAction("MemberDetails", new { id = viewModel.userManage.UserID });
                        }
                        else
                        {
                            ModelState.AddModelError("ConfirmPassword", "新密碼和確認密碼不匹配");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("OldPassword", "舊密碼不正確");
                    }
                }
                else
                {
                    return HttpNotFound();
                }

            }

            return View(viewModel);
        }


        ///////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public ActionResult Submit_UserQuestion(MemberDetailsViewModel MQ)
        {

            MemberDetailsViewModel QuestionModel = new MemberDetailsViewModel();
            var Question = QuestionModel.SubmitUQ;

            var UserID = GetUserID();

            MQ.SubmitUQ.UserID = UserID;
            MQ.SubmitUQ.QuestionTime = DateTime.Now;


            db.UserQuestions.Add(MQ.SubmitUQ);

            try
            {
                db.SaveChanges();
            }
            catch
            {

            }

            return RedirectToAction("MemberDetails", "UserManages", new { id = UserID });
        }



        ///////////////樹洞
        ///
        public ActionResult TreeHole()
        {

            if (!CheckLoggedIn())
            {
                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
            }
            return View();
        }


        ///////////////心情更新創建
        ///
        public ActionResult MoodEdit()
        {

            int UserID = GetUserID();
            var time = DateTime.Now.ToString("M");
            var moodcontext = Request.Form["moodcontext"];



            //將當天的心情紀錄寫出

            var recorddb = db.Moods;

            var record = recorddb.Where(m => m.UserID == UserID).ToList().Where(m => m.Time.ToString("M") == time).ToList();

            //如果心情紀錄不等於空則更新Mood
            if (record.Count > 0)
            {
                foreach (var i in record)
                {
                    i.Mood1 = moodcontext;
                }
                db.SaveChanges();
            }
            //如果心情紀錄等於空則創建新的mood 

            else
            {
                var newMood = new Mood
                {
                    UserID = UserID,
                    Time = DateTime.Now,
                    Mood1 = moodcontext
                };

                recorddb.Add(newMood);
                db.SaveChanges();
            }

           

            return RedirectToAction("MemberDetails", "UserManages", new { id = UserID });

        }





        ///////////////////////////Method/////////////////////////////////

        public void moodcount()
        {

            int UserID = GetUserID();

            var mood = db.Moods.Where(m => m.UserID == UserID).ToList();

            ViewBag.happyCount = mood.Where(m => m.Mood1 == "happy").ToList().Count;
            ViewBag.unhappyCount = mood.Where(m => m.Mood1 == "unhappy").ToList().Count;
            ViewBag.sadCount = mood.Where(m => m.Mood1 == "sad").ToList().Count;
            ViewBag.angryCount = mood.Where(m => m.Mood1 == "angry").ToList().Count;

        }







    }
}