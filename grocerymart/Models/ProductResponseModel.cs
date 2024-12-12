namespace grocerymart.Models;

public class ProductResponseModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }

    public string Description { get; set; }

    public string Img_url { get; set; }

    public string Type { get; set; }
}