using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_TOKEN_REFRESH_NET_CORE_8.DAL
{
    [Table("Tbl_Users")]
    public class Tbl_Users1
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key,Column(Order = 1)]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        public string? UserRole { get; set; }
        public string? UserEmail { get; set; }
        public string? UserMobile { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifyOn { get; set; }
    }
}
