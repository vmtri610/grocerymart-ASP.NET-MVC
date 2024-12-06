using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace grocerymart.Models;

[Table("profiles")]
public class ProfileModel : BaseModel
{
    [PrimaryKey("id")] public string Id { get; set; }

    [Column("created_at")] public string CreatedAt { get; set; }

    [Column("full_name")] public string FullName { get; set; }

    [Column("email")] public string Email { get; set; }

    [Column("avatar_url")] public string AvatarUrl { get; set; }
}