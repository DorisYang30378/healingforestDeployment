//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace postArticle.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Message
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Message()
        {
            this.Report_Message = new HashSet<Report_Message>();
        }
    
        public int MessageID { get; set; }
        public int UserID { get; set; }
        public int ArticleID { get; set; }
        public string Content { get; set; }
        public System.DateTime Time { get; set; }
    
        public virtual Article Article { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Report_Message> Report_Message { get; set; }
        public virtual UserManage UserManage { get; set; }
    }
}
