using BMS.Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS.Domain.Entity;

[Table("Book")]
public class BookEntity : EntityBase
{
    [Column("Title")]
    public string Title { get; set; }

    [Column("Author")]
    public string Author { get; set; }

    [Column("PublishedDate")]
    public DateTime PublishedDate { get; set; }

    [Column("CategoryId")]
    public BookCategory CategoryId { get; set; } = BookCategory.Other;
}
