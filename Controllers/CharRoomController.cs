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
        public viewmodel.ChatRoomViewModel Message = new ChatRoomViewModel();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;

        public int GetUserID() => Convert.ToInt32(Session["UserID"]);

        public string GetUserName() => db.UserManages.Find(GetUserID()).UserName;

        #endregion
        // -----------------------------------------===============================



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

            string username;

            //Respone UserID
            if (CheckLoggedIn())
            {
                int MyUserID = GetUserID();
                ViewBag.UserID = MyUserID;

                //UserName
                try
                {
                   username = db.UserManages.Find(MyUserID).UserName;
                }

                catch {
                    return HttpNotFound();
                }
               
                CMV.UserName = username;



                if (!String.IsNullOrEmpty(searchString))
                {
                    searchmember(searchString);
                }

                Friend();

                Expert();

                return View(CMV);
            }



            //如果未登入狀態傳送錯誤
            return HttpNotFound();





        }



        //Method



        //搜尋要找的使用者
        public void searchmember(string searchString)
        {

            int MyUserID = GetUserID();
            string MyUserName = GetUserName();

            if (searchString != MyUserName)
            {
                var member = from m in db.UserManages select m;
                member = member.Where(s => s.UserName.Contains(searchString));
                var searchchatroom = from m in db.Chatrooms.Where(m => m.UserID == MyUserID || m.OtherUserID == MyUserID) select m;
                CMV.SMChatRoom = searchchatroom;
                CMV.SearchMember = member;



            }

        }



        //有創建過聊天室的人
        public void Friend()
        {

            int MyUserID = GetUserID();

            var Memberchatroom = from m in db.Chatrooms.Where(m => m.UserID == MyUserID || m.OtherUserID == MyUserID) select m;

            CMV.ChatRoom = Memberchatroom;
            Message.ChatRoom = Memberchatroom;
        }




        public void Expert()
        {
            int MyUserID = GetUserID();

            List<UserManage> Experts = db.UserManages.Where(a => a.UserType == "Expert" && a.UserID != MyUserID).ToList();

            var Expertchatroom = from m in db.Chatrooms.Where(m => m.UserID == MyUserID || m.OtherUserID == MyUserID) select m;
            CMV.EChatRoom = Expertchatroom;
            CMV.Expert = Experts;
        }

        public List<int> GetRoom(List<int> e)
        {

            List<int> f = new List<int>();
            int MyUserID = GetUserID();
            var Memberchatroom = from m in db.Chatrooms.Where(m => m.UserID == MyUserID || m.OtherUserID == MyUserID) select m.ChatroomID;
            f = Memberchatroom.ToList();
            return f;
        }


        ////////////////////////////////////////////////////進入聊天室畫面////////////////////////////////////////////////////////////




        public ActionResult ChatRoom(int? id)
        {

            //判斷是否為使用者的chatroom
            List<int> f = new List<int>();

            f = GetRoom(f);

            f = GetRoom(f);




            int MainUserNameID = (int)Session["UserID"];
            var username = db.UserManages.Find(MainUserNameID).UserName;

            Message.UserName = username;
            Message.MainUserID = (int)MainUserNameID;
            Message.ChatRoomID = (int)id;

            Message.UserMange = db.UserManages;
            Friend();


            if (id != null)
            {

                if (f.Contains((int)id) == true)
                {
                    //System.Diagnostics.Debug.WriteLine("PASS");
                    var message = from a in db.ChatroomLogs where (a.ChatroomID == id) select a;

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

            else
            {
                return HttpNotFound();
            }



        }


        /////write message///
        

        //
        [HttpPost]
        public ActionResult Writemessage(int? id)
        {

            int UserID = int.Parse(Request.Form["userID"]);
            int ChatRoomID = int.Parse(Request.Form["ChatRoomID"]);
            string Content = "";

            try
            {
                Content = Request.Form["inputcontext"];
            }

            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
            }


            ChatroomLog newMessage = new ChatroomLog { UserID = UserID, ChatroomID = ChatRoomID, Content = Content, Time = DateTime.Now };
            db.ChatroomLogs.Add(newMessage);
            try
            {
                db.SaveChanges();
            }
            catch {
                TempData["ErrorMessage"] ="輸入未成功,留言太長了";
            }
            

            return RedirectToAction("ChatRoom", "CharRoom", new { id });
        }


        [HttpGet]
        public ActionResult CreatChatRoom(int? id)
        {
            if (id!= null)
            {
                int MainID = GetUserID();
                string MainName = GetUserName();
                int OtherID = (int)id;
                string OtherName = db.UserManages.Find(OtherID).UserName;
                string member = MainName + "/" + OtherName;
                string RoomName = MainName + "/" + OtherName + "的私聊";
                Chatroom newchatroom = new Chatroom { ChatRoomName = RoomName, member = member, UserID = MainID, OtherUserID = OtherID };
                db.Chatrooms.Add(newchatroom);
                db.SaveChanges();
                return RedirectToAction("Charmember", "CharRoom");
            }
            else
            {
                return RedirectToAction("Charmember", "CharRoom");
            }
        }
    }
}