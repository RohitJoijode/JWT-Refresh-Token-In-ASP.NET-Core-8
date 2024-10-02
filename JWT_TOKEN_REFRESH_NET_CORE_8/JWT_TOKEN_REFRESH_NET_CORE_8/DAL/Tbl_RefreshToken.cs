using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_TOKEN_REFRESH_NET_CORE_8.DAL
{
    [Table("Tbl_RefreshToken")]
    public class Tbl_RefreshToken1
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key,Column(Order = 1)]
        public int? Id { get; set; }
        public string? UserId { get; set; }
        public string? Token { get; set; }
        public string? TokenExpiry { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedOn { get; set; }
        public DateTime? ModifyOn { get; set; }
    }
}
