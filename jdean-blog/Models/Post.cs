 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jdean_blog.Models
{
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>(); //creating a new hash set for the virtual property. provides for a faster search through tables
        }
        public int Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string MediaUrl { get; set; }
        public bool Published {get; set;}
        public string Slug {get; set;}

        public virtual ICollection<Comment> Comments { get; set; } 
    }
}