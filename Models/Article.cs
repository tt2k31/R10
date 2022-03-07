using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace R10.models
{
    [Table("Posts")]
    public class Article
    {
        [Key]
        public int Id { set; get; }
        
        [StringLength(255,MinimumLength = 3,ErrorMessage = "nhập từ {2} - {1} kí tự")]
        [Required(ErrorMessage ="{0} buộc nhập")]
        [Column(TypeName ="nvarchar")]
        [DisplayName("Tiêu đề")]
        public string Title { set; get; }
        
        [DataType(DataType.Date)]
        [Required(ErrorMessage ="{0} buộc nhập")]
        [DisplayName("Ngày Tạo")]
        public DateTime Created { set; get; }

        [Column(TypeName ="ntext")]
        [DisplayName("Nội dung")]
        public string Content { set; get; }

    }
}
