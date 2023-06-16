using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instagram.ViewModel
{
    public class CommentPost
    {
        public string CommentText { get; set; }
        public int CommentUserID { get; set; }
        public int PostID { get; set; }
        public int CommentID { get; set; }
        public string CommentMaker { get; set; }
        public string CommentMakerImage { get; set; }
    }
}