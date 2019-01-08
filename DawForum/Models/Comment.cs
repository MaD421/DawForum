using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DawForum.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}