using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Instagram.Models;
using Instagram.ViewModel;

namespace Instagram.Controllers
{
    
        public class UserController : Controller
        {

        public ActionResult HomePage( )
            {
            if (Session["Id"] != null)
            {
                var db = new Database1Entities();
            
            Int32 intsessionid = Convert.ToInt32(Session["Id"]);
            
                List<Post_Info> post_Infos = new List<Post_Info>();
                var followingID = db.FollowUsers.Where(c => c.UserSender == intsessionid).Where(c => c.state == "following").Select(c => c.UserReciver).ToArray();
                foreach (int folowid in followingID)
                {
                    var postsids = db.Posts.Where(c => c.UserID == folowid).Select(c => c.PostID).ToArray();
                    foreach (int idposts in postsids)
                    {
                        post_Infos.Add(new Post_Info
                        {
                            PostID = idposts,
                            UserID = folowid,
                            Name = db.UserInfoes.Where(c => c.ID == folowid).Select(c => c.Name).FirstOrDefault(),
                            Caption = db.Posts.Where(c => c.UserID == folowid).Where(c => c.PostID == idposts).Select(c => c.Captian).FirstOrDefault(),
                            PostImage = db.Posts.Where(c => c.UserID == folowid).Where(c => c.PostID == idposts).Select(c => c.Image).FirstOrDefault(),
                            ProfileImage = db.UserInfoes.Where(c => c.ID == folowid).Select(c => c.ProfilePicture).FirstOrDefault(),
                            PostLikes = db.PostLikes.Where(c => c.PostID == idposts).Select(c => c.LikeID).Count(),
                            IfLike = db.PostLikes.Where(c => c.UserID == intsessionid).Where(c => c.PostID == idposts).Select(c => c.LikeID).Count(),
                            CommentData = db.Comments.Where(c => c.PostID == idposts).Join(db.UserInfoes,
                            c => c.CommentMaker,
                            ui => ui.ID,
                            (c, ui) => new CommentPost
                            {
                                CommentText = c.Text,
                                CommentUserID = (Int32)c.CommentMaker,
                                PostID = idposts,
                                CommentMaker = ui.Name,
                                CommentID = c.CommentID,
                                CommentMakerImage = ui.ProfilePicture
                            }).ToList(),
                            LikesData = db.PostLikes.Where(c => c.PostID == idposts).Join(db.UserInfoes,
                            c => c.UserID,
                            ui => ui.ID,
                            (c, ui) => new LikePost
                            {
                                LikeID = c.LikeID,
                                LikePostID = c.PostID,
                                LikeUserID = c.UserID,
                                LikeMaker = ui.Name,
                                LikeProfileImage = ui.ProfilePicture,
                            }
                            ).ToList()

                        }
                        );
                    }

                }
                Post_Info[] Info_Post = post_Infos.ToArray();
                ViewBag.info_postt = Info_Post;

                ViewBag.UserProfilePicture = Session["Image"];

                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

            }
        [HttpPost]
        public ActionResult addComment(string theComment, int postID)
        {
            Database1Entities db = new Database1Entities();
            Int32 intsessionid = Convert.ToInt32(Session["Id"]);
            Comment c = new Comment();
            c.Text = theComment;
            c.PostID = postID;
            c.CommentMaker = intsessionid;
            db.Comments.Add(c);
            db.SaveChanges();
            return RedirectToAction("HomePage", "User");
        }

        [HttpPost]
        public ActionResult LikeSystem(int postID)
        {
            Database1Entities db = new Database1Entities();
            Int32 intsessionid = Convert.ToInt32(Session["Id"]);
            var id = db.PostLikes.Where(c => c.PostID == postID).Where(c => c.UserID == intsessionid).Select(c=>c.LikeID).ToList();
            @ViewBag.ifLike = id.Count;
            if(id.Count==0)
            {
               PostLike x = new PostLike();
               x.UserID = intsessionid;
               x.PostID = postID;
               db.PostLikes.Add(x);
               db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("delete from PostLike where PostID=@p0 and UserID=@p1", postID, intsessionid);
            }
            return RedirectToAction("HomePage", "User");
        }

        public ActionResult SearchPage(string SearchData)
        {
            if (Session["Id"] != null)
            {
                Database1Entities db = new Database1Entities();
                Int32 intsessionid = Convert.ToInt32(Session["Id"]);
                var usr = db.UserInfoes.Select(u => new UserInfoSearch { ID = u.ID, Name = u.Name, ProfileImage = u.ProfilePicture, UserName = u.UserName }).ToArray();

                List<UserInfoSearch> UsersFounded = new List<UserInfoSearch>();
                // Will be replaced by SQL query
                foreach (var user in usr)
                {

                    if (user.Name == SearchData || user.UserName == SearchData) {
                        var state = db.FollowUsers.Where(c => c.UserSender == intsessionid).Where(c => c.UserReciver == user.ID).Select(c => c.state).FirstOrDefault();
                        UsersFounded.Add(new UserInfoSearch { ID = user.ID, Name = user.Name, ProfileImage = user.ProfileImage, UserName = user.UserName, FollowState = state });
                            };
                }
                UserInfoSearch[] FoundedUsers = UsersFounded.ToArray();
                @ViewBag.FoundedUserss = FoundedUsers;
                ViewBag.UserProfilePicture = Session["Image"];
                ViewBag.sessionId = intsessionid;
                //end of the SQL replacer


                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }









        public ActionResult ProfilePage()
            {
            if (Session["Id"] != null)
            {

                Database1Entities db = new Database1Entities();
                Int32 intsessionid = Convert.ToInt32(Session["Id"]);

               
                var postsids = db.Posts.Where(c => c.UserID == intsessionid).Select(c => c.PostID).ToArray();
                List<Post_Info> post_Infos = new List<Post_Info>();
                foreach (int idposts in postsids)
                {
                    post_Infos.Add(new Post_Info
                    {
                        PostID = idposts,
                        UserID = intsessionid,
                        Name = db.UserInfoes.Where(c => c.ID == intsessionid).Select(c => c.Name).FirstOrDefault(),
                        Caption = db.Posts.Where(c => c.UserID == intsessionid).Where(c => c.PostID == idposts).Select(c => c.Captian).FirstOrDefault(),
                        PostImage = db.Posts.Where(c => c.UserID == intsessionid).Where(c => c.PostID == idposts).Select(c => c.Image).FirstOrDefault(),
                        ProfileImage = db.UserInfoes.Where(c => c.ID == intsessionid).Select(c => c.ProfilePicture).FirstOrDefault(),
                        PostLikes = db.PostLikes.Where(c => c.PostID == idposts).Select(c => c.LikeID).Count(),
                        IfLike = db.PostLikes.Where(c => c.UserID == intsessionid).Where(c => c.PostID == idposts).Select(c => c.LikeID).Count(),
                        FileExtention = Path.GetExtension("~/images/profiles/" + db.Posts.Where(c => c.UserID == intsessionid).Where(c => c.PostID == idposts).Select(c => c.Image).FirstOrDefault()),
                        CommentData = db.Comments.Where(c => c.PostID == idposts).Join(db.UserInfoes,
                        c => c.CommentMaker,
                        ui => ui.ID,
                        (c, ui) => new CommentPost
                        {
                            CommentText = c.Text,
                            CommentUserID = (Int32)c.CommentMaker,
                            PostID = idposts,
                            CommentMaker = ui.Name,
                            CommentID = c.CommentID,
                            CommentMakerImage = ui.ProfilePicture
                        }).ToList(),
                        LikesData = db.PostLikes.Where(c => c.PostID == idposts).Join(db.UserInfoes,
                        c => c.UserID,
                        ui => ui.ID,
                        (c, ui) => new LikePost
                        {
                            LikeID = c.LikeID,
                            LikePostID = c.PostID,
                            LikeUserID = c.UserID,
                            LikeMaker = ui.Name,
                            LikeProfileImage = ui.ProfilePicture,
                        }
                        ).ToList()

                    }
                    );
                }

                Post_Info[] Info_Post = post_Infos.ToArray();
                ViewBag.info_posttt = Info_Post;
                ViewBag.UserProfilePicture = Session["Image"];



                var usernamee = db.UserInfoes.Where(c => c.ID == intsessionid).Select(c => c.Name).FirstOrDefault();

                var postsOfCurrentUser = db.Posts.Where(c=> c.UserID== intsessionid).Select(c=>c.Image).ToList();
                var numberofposts = postsOfCurrentUser.Count();


                ViewBag.postsnumber = numberofposts;

                ViewBag.image = db.UserInfoes.Where(z => z.ID == intsessionid).Select(z => z.ProfilePicture).FirstOrDefault();




                var friends_id_followings = db.FollowUsers.Where(x => x.UserSender == intsessionid).Where(x => x.state == "following").Select(x => x.UserReciver).ToList();
                var friends_id_followers = db.FollowUsers.Where(x => x.UserReciver == intsessionid).Where(x => x.state == "following").Select(x => x.UserSender).ToList();
                var numbers_followings = friends_id_followings.Count;


                var numbers_followers = friends_id_followers.Count;
                var followerRequests= db.FollowUsers.Where(x => x.UserReciver == intsessionid).Where(x => x.state == "wait").Select(x => x.UserSender).ToList();
                ViewBag.followerRequests = followerRequests;
                ViewBag.follower_request_number = followerRequests.Count();
                ViewBag.followings_number = numbers_followings;
                ViewBag.followers_number = numbers_followers;
                ViewBag.friends = friends_id_followings;
                ViewBag.username = usernamee;
                ViewBag.posts = postsOfCurrentUser;

                List<FollowRequestData> waitingList = new List<FollowRequestData>();
                foreach (var id in followerRequests)
                {
                    var followerimage = db.UserInfoes.Where(c => c.ID == id).Select(c => c.ProfilePicture).FirstOrDefault();
                    var followername = db.UserInfoes.Where(c => c.ID == id).Select(c => c.Name).FirstOrDefault();
                    waitingList.Add(new FollowRequestData
                    {
                        name = followername,
                        id=id,
                        image= followerimage,
                    });
                    
                }
                ViewBag.waitingrequest = waitingList;

                List<string> followingList = new List<string>();
                foreach (var id in friends_id_followings)
                {
                    var followername = db.UserInfoes.Where(c => c.ID == id).Select(c => c.Name).FirstOrDefault();
                    followingList.Add(followername);
                }

                List<string> followersList = new List<string>();
                foreach (var id in friends_id_followers)
                {
                    var followername = db.UserInfoes.Where(c => c.ID == id).Select(c => c.Name).FirstOrDefault();
                    followersList.Add(followername);
                }
                ViewBag.followersnameV = followersList;
                ViewBag.followingsnameV = followingList;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }



            }
        [HttpPost]
        public ActionResult ProfilePage(int user_sender)
        {
            if (Session["Id"] != null)
            {
                Database1Entities db = new Database1Entities();
                Int32 intsessionid = Convert.ToInt32(Session["Id"]);
                db.Database.ExecuteSqlCommand("update FollowUser set state='following' where UserSender = @p0 and UserReciver = @p1", user_sender, intsessionid);
                
                return RedirectToAction("ProfilePage");
            }
            else
            {
                return RedirectToAction("Login");
            }



        }


        public ActionResult LogIn()
        {
            return View();
        }




        //login data checker and session maker
        [HttpPost]
        public ActionResult Login(UserInfo user)
        {      // database name
            using (var db = new Database1Entities())
            {
                var usr = db.UserInfoes.FirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);
                // session maker
                if (usr != null)
                {
                    Session["Id"] = usr.ID.ToString();
                    Session["Email"] = usr.UserName.ToString();
                    Session["Image"] = usr.ProfilePicture.ToString();
                    Session["Name"] = usr.Name.ToString();
                    //redirection to loggedin view
                    return RedirectToAction("Loggedin");
                }
                else
                {
                    //error msg
                    ModelState.AddModelError("login", @"email or password is wrong ¯\_(ツ)_/¯ ");
                }


            }
            return View();
        }

        //session checker
        public ActionResult Loggedin()
        {
            if (Session["Id"] != null)
            {
                //if true redirection to homepage
                return RedirectToAction("HomePage");

            }
            else
            {
                //if false redirection to login
                return RedirectToAction("login");
            }
        }


        //this controller sends u to register with no conditions (important)
        public ActionResult Registration()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Registration(UserInfo account)
        {
            //checking data not null and valid
            if (ModelState.IsValid)
            {
                //   database name
                using (var db = new Database1Entities())
                {
                    if (db.UserInfoes.Any(x => x.UserName == account.UserName))
                    {
                        ViewBag.ErrorMessage = "This email already exist.";
                        return View("Registration", account);
                    }
                    else
                    {
                        db.UserInfoes.Add(account);
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("update userinfo set ProfilePicture=@p0 where ProfilePicture is null", "Default profile.png");

                    }
                }
                ModelState.Clear();
                //success msg
                ViewBag.Message = account.Name + " Your account succesfully created";
                return View();
            }
            else
            {
                //if false return to same view with same data
                return View(account);
            }
        }

        public ActionResult EditProfile()
        {
            var db = new Database1Entities();
            if (Session["Id"] != null)
            {
                ViewBag.UserProfilePicture = Session["Image"];
                Int32 intsessionid = Convert.ToInt32(Session["Id"]);
                var usr = db.UserInfoes.FirstOrDefault(u => u.ID == intsessionid);
                ViewBag.usr = usr;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult EditProfile(UserInfo account, HttpPostedFileBase file)
        {
            var db = new Database1Entities();
            if (Session["Id"] != null)
            {
                Int32 intsessionid = Convert.ToInt32(Session["Id"]);
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/profiles/"), pic);

                    file.SaveAs(path);

                    account.ProfilePicture = file.FileName;
                    db.Database.ExecuteSqlCommand("update userinfo set ProfilePicture=@p0 where Id=@p1", account.ProfilePicture, intsessionid);
                }

                db.Database.ExecuteSqlCommand("update userinfo set Name=@p0 where Id=@p1", account.Name, intsessionid);
                db.Database.ExecuteSqlCommand("update userinfo set UserName=@p0 where Id=@p1", account.UserName, intsessionid);
                db.Database.ExecuteSqlCommand("update userinfo set Mobile=@p0 where Id=@p1", account.Mobile, intsessionid);
                return RedirectToAction("HomePage");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        public ActionResult Changepassword()
        {
            var db = new Database1Entities();
            if (Session["Id"] != null)
            {
                Int32 intsessionid = Convert.ToInt32(Session["Id"]);
                var usr = db.UserInfoes.FirstOrDefault(u => u.ID == intsessionid);
                ViewBag.UserProfilePicture = Session["Image"];
                ViewBag.usr = usr;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public ActionResult Changepassword(ChangePassword password)
        {
            Database1Entities db = new Database1Entities();
            if (Session["Id"] != null)
            {
                if (ModelState.IsValid)
                {
                    Int32 intsessionid = Convert.ToInt32(Session["Id"]);
                    var usr = db.UserInfoes.FirstOrDefault(u => u.ID == intsessionid);
                    ViewBag.UserProfilePicture = Session["Image"];
                    ViewBag.usr = usr;
                    if (password.Newpassword.Equals(password.Confirmpassword))
                    {
                        if (password.Oldpassword.Equals(db.UserInfoes.Where(c => c.ID.Equals(intsessionid)).Select(c => c.Password).FirstOrDefault()))
                        {
                            db.Database.ExecuteSqlCommand("update UserInfo set Password=@p0 where Id=@p1", password.Newpassword, intsessionid);
                            ViewBag.success = "Password has changed successfully";
                            return View();
                        }
                        else
                        {
                            ViewBag.password = "This isn't your old-password";
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.confirm = "Confirm password doesn't match";
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        public ActionResult AddPostPage()
        {
            return View();
        }


        [HttpPost]
         public ActionResult AddPostPage(Post post, HttpPostedFileBase file)
         {
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/images/profiles/"), pic);

                file.SaveAs(path);

                post.Image = file.FileName;
            }


            using (var db = new Database1Entities())
             {
                db.Database.ExecuteSqlCommand("insert into Post(UserID, Captian, Image) values(@p0 , @p1, @p2)", Convert.ToInt32(Session["Id"]),post.Captian,post.Image);

            }
            return RedirectToAction("HomePage");
         }


        public ActionResult Externalprofile(int id)
        {
            Database1Entities db = new Database1Entities();

            Int32 intsessionid = Convert.ToInt32(Session["Id"]);

            var checkfollow = db.FollowUsers.Where(c => c.UserSender == intsessionid).Where(c => c.UserReciver == id).Select(c => c.state).FirstOrDefault();
            ViewBag.checkfollow = checkfollow;

            
            var postsids = db.Posts.Where(c => c.UserID == id).Select(c => c.PostID).ToArray();
            List<Post_Info> post_Infos = new List<Post_Info>();
            foreach (int idposts in postsids)
            {
                
                post_Infos.Add(new Post_Info
                {
                    PostID = idposts,
                    UserID = id,
                    Name = db.UserInfoes.Where(c => c.ID == id).Select(c => c.Name).FirstOrDefault(),
                    Caption = db.Posts.Where(c => c.UserID == id).Where(c => c.PostID == idposts).Select(c => c.Captian).FirstOrDefault(),
                    PostImage = db.Posts.Where(c => c.UserID == id).Where(c => c.PostID == idposts).Select(c => c.Image).FirstOrDefault(),
                    ProfileImage = db.UserInfoes.Where(c => c.ID == id).Select(c => c.ProfilePicture).FirstOrDefault(),
                    PostLikes = db.PostLikes.Where(c => c.PostID == idposts).Select(c => c.LikeID).Count(),
                    IfLike = db.PostLikes.Where(c => c.UserID == intsessionid).Where(c => c.PostID == idposts).Select(c => c.LikeID).Count(),
                    CommentData = db.Comments.Where(c => c.PostID == idposts).Join(db.UserInfoes,
                    c => c.CommentMaker,
                    ui => ui.ID,
                    (c, ui) => new CommentPost
                    {
                        CommentText = c.Text,
                        CommentUserID = (Int32)c.CommentMaker,
                        PostID = idposts,
                        CommentMaker = ui.Name,
                        CommentID = c.CommentID,
                        CommentMakerImage = ui.ProfilePicture
                    }).ToList(),
                    LikesData = db.PostLikes.Where(c => c.PostID == idposts).Join(db.UserInfoes,
                    c => c.UserID,
                    ui => ui.ID,
                    (c, ui) => new LikePost
                    {
                        LikeID = c.LikeID,
                        LikePostID = c.PostID,
                        LikeUserID = c.UserID,
                        LikeMaker = ui.Name,
                        LikeProfileImage = ui.ProfilePicture,
                    }
                    ).ToList()

                }
                );
            }

            Post_Info[] Info_Post = post_Infos.ToArray();
            ViewBag.info_posttt = Info_Post;
            ViewBag.UserProfilePicture = Session["Image"];



            var usernamee = db.UserInfoes.Where(c => c.ID == id).Select(c => c.Name).FirstOrDefault();

            var postsOfCurrentUser = db.Posts.Where(c => c.UserID == id).Select(c => c.Image).ToList();
            var numberofposts = postsOfCurrentUser.Count();


            ViewBag.postsnumber = numberofposts;

            ViewBag.image = db.UserInfoes.Where(z => z.ID == id).Select(z => z.ProfilePicture).FirstOrDefault();




            var friends_id_followings = db.FollowUsers.Where(x => x.UserSender == id).Where(x => x.state == "following").Select(x => x.UserReciver).ToList();
            var friends_id_followers = db.FollowUsers.Where(x => x.UserReciver == id).Where(x => x.state == "following").Select(x => x.UserSender).ToList();
            var numbers_followings = friends_id_followings.Count;


            var numbers_followers = friends_id_followers.Count;
            var followerRequests = db.FollowUsers.Where(x => x.UserReciver == id).Where(x => x.state == "wait").Select(x => x.UserSender).ToList();
            ViewBag.followerRequests = followerRequests;
            ViewBag.follower_request_number = followerRequests.Count();
            ViewBag.followings_number = numbers_followings;
            ViewBag.followers_number = numbers_followers;
            ViewBag.friends = friends_id_followings;
            ViewBag.username = usernamee;
            ViewBag.posts = postsOfCurrentUser;

            List<string> followingList = new List<string>();
            foreach (var idd in friends_id_followings)
            {
                var followername = db.UserInfoes.Where(c => c.ID == idd).Select(c => c.Name).FirstOrDefault();
                followingList.Add(followername);
            }

            List<string> followersList = new List<string>();
            foreach (var idd in friends_id_followers)
            {
                var followername = db.UserInfoes.Where(c => c.ID == idd).Select(c => c.Name).FirstOrDefault();
                followersList.Add(followername);
            }
            
            
            ViewBag.followersnameV = followersList;
            ViewBag.followingsnameV = followingList;

            return View();
        }
        [HttpPost]
        public ActionResult SearchPage (int id)
        {
            Database1Entities db = new Database1Entities();
            Int32 intsessionid = Convert.ToInt32(Session["Id"]);
            var usersfollowing = db.FollowUsers.Where(c => c.UserSender == intsessionid).Where(c=>c.state=="following").Select(c => c.UserReciver).ToArray();
            ViewBag.UsersFollowing = usersfollowing;
            db.Database.ExecuteSqlCommand("insert into FollowUser(UserSender, UserReciver, state) values(@p0 , @p1 ,@p2)", Convert.ToInt32(Session["Id"]), id, "wait");
            return RedirectToAction("SearchPage");
        }



    }




}