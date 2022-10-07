using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Category name can't be null or empty")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Image url can't be null or empty")]
        public string ImageUrl { get; set; }
        public ICollection<Property> Properties { get; set; }   

    }
}
