using System;
using System.Web;
using Microsoft.AspNet.SignalR;
namespace postArticle
{
    public class ChatHub : Hub
    {
        public void Send(int UserID, int MainID, string UserName, string inputContext, string inputTime)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(UserID, MainID, UserName, inputContext, inputTime);
        }
    }
}