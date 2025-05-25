using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Group_Role : BaseEntity
    {
        public int? group_id { get; set; }  
        public int? role_id { get; set; }  
        public Group? group {  get; set; }    
        public Role? role {  get; set; }    
    }
}
