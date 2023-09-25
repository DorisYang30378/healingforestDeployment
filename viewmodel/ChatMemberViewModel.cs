using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using postArticle.Models;


namespace postArticle.viewmodel
{
    public class ChatMemberViewModel
    {


        //會員ID
        public int? UserID { get; set; }
        //會員名稱
        public string UserName { get; set; }





        //將會員曾經的聊天室加入資料庫
        public IEnumerable<Chatroom> ChatRoom { get; set; }

        //將搜尋欄的聊天室加入資料庫
        public IEnumerable<Chatroom> SMChatRoom { get; set; }

        //將專家欄的聊天室加入資料庫
        public IEnumerable<Chatroom> EChatRoom { get; set; }

        //專家資料庫
        public IEnumerable<UserManage> Expert { get; set; }

        //搜尋使用者
        public IEnumerable<UserManage> SearchMember { get; set; }

    }
}