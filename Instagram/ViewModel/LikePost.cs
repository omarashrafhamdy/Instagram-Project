using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instagram.ViewModel
{
    public class LikePost
    {
        public int LikeID { get; set; }
        public int LikePostID { get; set; }
        public int LikeUserID { get; set; }
        public String LikeMaker { get; set; }
        public string LikeProfileImage { get; set; }
    }
}