using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using postArticle.Models;

namespace postArticle.viewmodel
{
    public class ChatRoomViewModel
    {

        public IEnumerable<Chatroom> ChatRooms { get; set; }
        public IEnumerable<ChatroomLog> ChatContext { get; set; }

        public IEnumerable<UserManage> UserMange { get; set; }

        //將會員曾經的聊天室加入資料庫
        public IEnumerable<Chatroom> ChatRoom { get; set; }


        public string inputContext { get; set; }

        public string UserName { get; set; }

        public int  MainUserID { get; set; }

        public int OtherUserID { get; set; }
        public int ChatRoomID { get; set; }


    }
}