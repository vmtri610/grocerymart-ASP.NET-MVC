using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace grocerymart.Models;

[Table("cart_items")]
public class CartModel : BaseModel
{
    [Column("id")] public string Id { get; set; }

    [Column("product_id")] public string ProductId { get; set; }

    [Column("user_id")] public string UserId { get; set; }

    [Column("created_at")] public string CreatedAt { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, ProductId: {ProductId}, UserId: {UserId}, CreatedAt: {CreatedAt} ";
    }
}