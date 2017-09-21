using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jdean_blog.Models
{
    public class Comment
    {
        public int Id {get; set;}
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; }
        public string Body { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedReason { get; set; }

        public virtual ApplicationUser Author { get; set; }
        public virtual Post BlogPost { get; set; }
    }
    
}