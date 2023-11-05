using postArticle.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace postArticle.viewmodel
{
    public class EditPasswordViewModel
    {

        public int UserID { get; set; }

        [Required(ErrorMessage = "必須輸入舊密碼")]
        [Display(Name = "請輸入舊密碼")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "必須輸入新密碼")]
        [Display(Name = "請輸入新密碼")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "必須確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼和確認密碼不匹配")]
        [Display(Name = "請確認新密碼")]
        public string ConfirmPassword { get; set; }



       // public UserManage UserManage { get; set; }




    }
}