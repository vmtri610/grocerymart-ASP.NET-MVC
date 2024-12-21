using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace grocerymart.Models;

[Table("order_history")]
public class OrderHistoryModel : BaseModel
{
    [PrimaryKey("id")] public string Id { get; set; }

    [Column("user_id")] public string UserId { get; set; }

    [Column("total_amount")] public long TotalAmount { get; set; }

    [Column("created_at")] public DateTime OrderDate { get; set; }
}