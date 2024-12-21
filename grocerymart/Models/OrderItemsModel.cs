using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace grocerymart.Models;

[Table("order_items")]
public class OrderItemsModel : BaseModel
{
    [PrimaryKey("id")] public string Id { get; set; }

    [Column("order_id")] public string OrderId { get; set; }

    [Column("product_id")] public string ProductId { get; set; }

    [Column("quantity")] public long Quantity { get; set; }
}