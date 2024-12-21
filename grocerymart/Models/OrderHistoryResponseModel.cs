namespace grocerymart.Models;

public class OrderHistoryResponseModel
{
    public string Id { get; set; }

    public string CreatedAt { get; set; }

    public long TotalAmount { get; set; }

    public long Quantity { get; set; }

    public string Name { get; set; }

    public string ImgUrl { get; set; }

    public long Price { get; set; }
}