using System.Text.Json.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace grocerymart.Models;

[Table("profiles")]
public class ProfileModel : BaseModel
{
    [PrimaryKey("id")] [JsonIgnore] public string Id { get; set; }

    [Column("created_at")] [JsonIgnore] public string CreatedAt { get; set; }

    [Column("full_name")] public string FullName { get; set; }

    [Column("phone_number")] public string PhoneNumber { get; set; }

    [Column("username")] public string Username { get; set; }

    [Column("email")] public string Email { get; set; }

    [Column("avatar_url")] public string AvatarUrl { get; set; }
}