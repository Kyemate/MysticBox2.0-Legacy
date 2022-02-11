using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MysticBox.Models
{
    public enum MimeType
    {
        None,
        Images,
        Audio_Videos,
        Documents,
        Files
    };

    public class ImageModel : BaseModel {[Required] [MaxLength(128)] public string ContentType { get; set; } }

    public class AudioVideoModel : BaseModel {[Required] [MaxLength(128)] public string ContentType { get; set; } }

    public class DocumentModel : BaseModel {[Required] [MaxLength(128)] public string ContentType { get; set; } }

    public class FileModel : BaseModel { }

    public abstract class BaseModel
    {
        //[Column(TypeName = "varchar(32)")]
        [Key]
        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string OriginalName { get; set; }

        [Required]
        public long Size { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        [MaxLength(16)]
        public string UploadKey { get; set; }
    }
}
