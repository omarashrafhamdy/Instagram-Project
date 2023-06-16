using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instagram.ViewModel
{
    public class Post_Info
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public string PostImage { get; set; }
        public int PostID { get; set; }
        public string ProfileImage { get; set; }
        public int PostLikes { get; set; }
        public int IfLike { get; set; }
        public IEnumerable<CommentPost> CommentData { get; set; }
        public IEnumerable<LikePost> LikesData { get; set; }
        public string FileExtention { get; set; }

    }
}