using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Services.Administration.Identity.Core.Domain
{
    [Table("Pages")]
    public class Page
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleEn { get; set; }
        public string RouteName { get; set; }
        public string IconName { get; set; }
        public int Order { get; set; } = 1;
        public int? ParentId { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        public int? RoleId { get; set; }

        // public virtual Page Parent { get; set; }
        [ForeignKey("ParentId")]
        public  List<Page> Children { get; set; }
    }
}
