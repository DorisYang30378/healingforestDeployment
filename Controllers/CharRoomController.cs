using postArticle.Models;
using postArticle.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Threading.Tasks;


namespace postArticle.Controllers
{
    public class CharRoomController : Controller
    {
        //---------------基礎屬性-----------------------------------------
        #region 基礎屬性
        private healingForestEntities db = new healingForestEntities();

        public viewmodel.ChatMemberViewModel CMV = new ChatMemberViewModel();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;

        public int GetUserID() => Convert.ToInt32(Session["UserID"]);





        #endregion
        // -----------------------------------------===============================
        public ActionResult FindChatUser()
        {
            if (CheckLoggedIn())
            {
                int MyUserId = GetUserID();
                UserManage currentUser = db.UserManages.Find(MyUserId);
                string currentUserUsername = currentUser.UserName;

                var chatroom = db.Chatrooms.FirstOrDefault(cr => cr.member.Contains("/" + currentUserUsername + "/") ||
                                                   cr.member.StartsWith(currentUserUsername + "/") ||
                                                   cr.member.EndsWith("/" + currentUserUsername));

                if (chatroom != null)
                {
                    string member = chatroom.member;

                    // 拆分 member 字符串，并选择非当前用户的用户名
                    string[] usernames = member.Split('/');
                    string otherUserName = "";
                    int OtherUserID;

                    foreach (string username in usernames)
                    {
                        if (username != currentUserUsername)
                        {
                            otherUserName = username;
                            break;
                        }
                    }
                    var UserManageID = from UserManagedb in db.UserManages
                                       where UserManagedb.UserName == otherUserName
                                       select UserManagedb.UserID;

                    if (UserManageID.Any())
                    {
                        OtherUserID = UserManageID.FirstOrDefault();

                        return RedirectToAction("DemoChar", "CharRoom", new { id = OtherUserID });
                    }
                }
            }
            return RedirectToAction("UserIndex", "CharRoom");
        }

        public ActionResult UserIndex()
        {
            if (CheckLoggedIn())
            {
                int currentUserID = GetUserID();// 填入當前使用者的 userID

                var userManage = db.UserManages
                            .Where(u => u.UserID != currentUserID)
                            .Include(u => u.ThanksfulThing)
                            .ToList();

                return View(userManage.ToList());
            }
            return RedirectToAction("ArticleIndex", "Home");
        }


        public ActionResult DemoChar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int MyUserId = GetUserID();
            UserManage currentUser = db.UserManages.Find(MyUserId);
            UserManage otherUser = db.UserManages.Find(id);

            if (otherUser == null || !CheckLoggedIn())
            {
                return HttpNotFound();
            }

            string currentUserUsername = currentUser.UserName;
            string otherUserUsername = otherUser.UserName;

            string memberName1 = currentUserUsername + "/" + otherUserUsername;
            string memberName2 = otherUserUsername + "/" + currentUserUsername;

            Chatroom chatroom = db.Chatrooms.FirstOrDefault(cr => cr.member == memberName1 || cr.member == memberName2);

            if (chatroom == null)
            {
                chatroom = new Chatroom
                {
                    member = memberName1,
                    ChatRoomName = memberName2 + "的私聊"
                };

                db.Chatrooms.Add(chatroom);
                db.SaveChanges();
            }

            int chatRoomID = chatroom.ChatroomID;

            var chatRoomLogList = db.ChatroomLogs.Where(cl => cl.ChatroomID == chatRoomID);

            TempData["ChatRoomID"] = chatRoomID;
            TempData["userManageID"] = id;

            List<Chatroom> chatrooms = db.Chatrooms
                .Where(cr => cr.member.Contains("/" + currentUserUsername + "/") ||
                             cr.member.StartsWith(currentUserUsername + "/") ||
                             cr.member.EndsWith("/" + currentUserUsername))
                .ToList();

            return View(new DemoCharViewModel
            {
                ChatRoomLog = chatRoomLogList,
                OtherUser = otherUserUsername,
                MyUser = currentUserUsername,
                ChatRoom = chatrooms,
                ChatRoomName = chatroom.ChatRoomName,
                OtherUserId = (int)id,
                myUserId = MyUserId
            });
        }

        public ActionResult CreateCharLog(DemoCharViewModel demoCharViewModel)
        {
            if (demoCharViewModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int ChatRoomID = (int)(TempData["ChatRoomID"]);
            int id = (int)TempData["userManageID"];

            Chatroom Chatroom = db.Chatrooms.Find(ChatRoomID);

            if (Chatroom == null || !CheckLoggedIn())
            {
                return HttpNotFound();
            }

            int UserID = GetUserID();

            if (!string.IsNullOrEmpty(demoCharViewModel.Log))
            {
                if (ModelState.IsValid)
                {
                    ChatroomLog ChatroomLog = new ChatroomLog
                    {
                        Content = demoCharViewModel.Log,
                        UserID = UserID,
                        ChatroomID = ChatRoomID,
                        Time = DateTime.Now
                    };


                    db.ChatroomLogs.Add(ChatroomLog);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("DemoChar", "CharRoom", new { id });
        }

        public ActionResult OtherChatRoom(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Chatroom Chatroom = db.Chatrooms.Find(id);

            if (Chatroom == null || !CheckLoggedIn())
            {
                return HttpNotFound();
            }

            int UserID = GetUserID();

            UserManage UserManage = db.UserManages.Find(UserID);
            string MyUserName = UserManage.UserName;

            string member = Chatroom.member;

            // 拆分 member 字符串，并选择非当前用户的用户名
            string[] usernames = member.Split('/');
            string otherUserName = "";
            int OtherUserID;

            foreach (string username in usernames)
            {
                if (username != MyUserName)
                {
                    otherUserName = username;
                    break;
                }
            }

            var UserManageID = from UserManagedb in db.UserManages
                               where UserManagedb.UserName == otherUserName
                               select UserManagedb.UserID;

            if (UserManageID.Any())
            {
                OtherUserID = UserManageID.FirstOrDefault();

                return RedirectToAction("DemoChar", "CharRoom", new { id = OtherUserID });
            }
            else
            {
                return HttpNotFound();

            }

        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                                        //
        //                                                                                                                        //
        //                                                                                                                        //
        //                                                                                                                        //
        //                                                                                                                        //
        //                                                                                                                        //
        //                                                                                                                        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                                                                                                                        




        ////////////////////////////////////////Chat_Room_Member 畫面//////////////////////////////////////////////////////////////

        //ActionResult 

        public ActionResult Charmember(string searchString)
        {

            //Respone UserID
            if (CheckLoggedIn())
            {
                int MyUserID = GetUserID();
                ViewBag.UserID = MyUserID;

                //UserName
                string username = db.UserManages.Find(MyUserID).UserName;
                CMV.UserName = username;


                
                if (!String.IsNullOrEmpty(searchString))
                {
                    searchmember(searchString);
                }

                Friend(username);

                Expert();

                return View(CMV);
            }



            //如果未登入狀態傳送錯誤
            return HttpNotFound();





        }


        public ActionResult Search(string searchString)  
        {
            List<UserManage> Member = db.UserManages.Where(a => a.UserType == "Member").ToList();

            return  RedirectToAction("Charmember", CMV);

        }




        //Method



        //搜尋要找的使用者
        public void searchmember(string searchString)
        {
            var member = from m in db.UserManages
                         select m;
            member = member.Where(s => s.UserName.Contains(searchString));

            int MyUserID = GetUserID();

            var searchchatroom = from m in db.Chatrooms.Where(m=>m.UserID == MyUserID || m.OtherUserID ==MyUserID) select m;


            CMV.SMChatRoom = searchchatroom;
            CMV.SearchMember = member;

        }



        //曾經聊天過的人
        public void Friend(string name)
        {
            var friend = from a in db.Chatrooms select a;
            friend = friend.Where(s => s.member.Contains(name));
            CMV.ChatRoom = friend;
        }




        public void Expert()
        {
            int MyUserID = GetUserID();

            List<UserManage> Experts = db.UserManages.Where(a => a.UserType == "Expert").ToList();

            var Expertchatroom = from m in db.Chatrooms.Where(m => m.UserID == MyUserID || m.OtherUserID == MyUserID) select m;
            CMV.EChatRoom = Expertchatroom;
            CMV.Expert = Experts;
        }

        

        ////////////////////////////////////////////////////進入聊天室畫面////////////////////////////////////////////////////////////

        public ActionResult ChatRoom(int? id)
        {

            ChatRoomViewModel Message = new ChatRoomViewModel();

            int MainUserNameID = (int)Session["UserID"];
            Message.MainUserID = (int)MainUserNameID;
            Message.ChatRoomID = (int)id;



            if (id != null)
            {
                var message = from a in db.ChatroomLogs where(a.ChatroomID ==id) select a;
                
                Message.ChatContext = message;

                var chatroom = from a in db.Chatrooms where (a.ChatroomID == id) select a;
                Message.ChatRooms = chatroom;


                return View(Message);
            }

            else
            {
                return HttpNotFound();
            }

           

        }
 

        /////write message///
        public ActionResult Writemessage(int ?id)
        {
            return RedirectToAction("ChatRoom", "CharRoom",new {id});
        }



    }
}