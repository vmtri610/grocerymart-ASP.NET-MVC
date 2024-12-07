using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace grocerymart.Models;

[Table("products")]
public class ProductModel : BaseModel
{
    [PrimaryKey("id")] public string Id { get; set; }
    [Column("name")] public string Name { get; set; }
    [Column("price")] public double Price { get; set; }

    [Column("description")] public string Description { get; set; }

    [Column("img_url")] public string Img { get; set; }

    [Column("type")] public string Type { get; set; }

    [Column("user_liked")] public string[] UserLiked { get; set; }
}