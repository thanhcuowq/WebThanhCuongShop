namespace ThanhCuongShop.Models
{
    public class ProductFilterViewModel
    {
        // Filter parameters
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Gender { get; set; }
        public string? Form { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SearchTerm { get; set; }
        
        // Available options for dropdowns
        public List<string> AvailableColors { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();
        public List<string> AvailableGenders { get; set; } = new();
        public List<string> AvailableForms { get; set; } = new();
        public List<string> AvailableCategories { get; set; } = new();
        
        // Price ranges
        public List<PriceRange> PriceRanges { get; set; } = new()
        {
            new PriceRange { Label = "Dưới 300k", MinPrice = 0, MaxPrice = 300000 },
            new PriceRange { Label = "300k - 500k", MinPrice = 300000, MaxPrice = 500000 },
            new PriceRange { Label = "Trên 500k", MinPrice = 500000, MaxPrice = decimal.MaxValue }
        };
        
        // Results
        public List<Product> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
    }
    
    public class PriceRange
    {
        public string Label { get; set; } = "";
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
} 