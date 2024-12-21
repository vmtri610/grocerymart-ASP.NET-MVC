using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace grocerymart.Models;

[Table("addresses")]
public class AddressModel : BaseModel
{
    [PrimaryKey("id")] public string Id { get; set; }

    [Column("user_id")] public string UserId { get; set; }

    [Column("type")] public string Type { get; set; }

    [Column("address")] public string Address { get; set; }

    [Column("city")] public string City { get; set; }

    [Column("state")] public string State { get; set; }

    [Column("zip_code")] public string ZipCode { get; set; }
}