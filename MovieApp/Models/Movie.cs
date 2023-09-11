using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string? Title { get; set; }

        [Display(Name = "Release Date"), DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s-]*$"), StringLength(30)]
        public string? Genre { get; set; }

        [Range(0, 10), Display(Name ="IMDb")]
        [Column(TypeName = "decimal(4,2)")]
        public decimal Imdb { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$"), StringLength(5)]
        public string? Rating { get; set; }

        //[RegularExpression(@"jpg$")]
        public string? Image { get; set; }
    }
}
