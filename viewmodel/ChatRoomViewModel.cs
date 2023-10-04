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


        public string inputContext { get; set; }

        public int  MainUserID { get; set; }

        public int OtherUserID { get; set; }
        public int ChatRoomID { get; set; }


    }
}