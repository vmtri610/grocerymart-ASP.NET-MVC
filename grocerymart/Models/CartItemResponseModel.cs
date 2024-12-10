namespace grocerymart.Models;

public class CartItemResponseModel
{
    public string CartId { get; set; }
    public string ProdId { get; set; }
    public string Name { get; set; }
    public long Price { get; set; }
    public long Quantity { get; set; }
    public string Img { get; set; }
}