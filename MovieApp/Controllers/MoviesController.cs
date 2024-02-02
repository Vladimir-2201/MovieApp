using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.Data;
using MovieApp.Models;
using System.Diagnostics;

namespace MovieApp.Controllers;

public class MoviesController(MovieAppContext context, IWebHostEnvironment webHostEnvironment) : Controller
{
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // GET: Movies
    public async Task<IActionResult> Index(string movieGenre, string searchString)
    {
        if (context.Movie == null)
        {
            return Problem("Entity set 'MovieAppContext.Movie'  is null.");
        }
        IQueryable<string> genreQuery = from m in context.Movie orderby m.Genre select m.Genre;
        var movies = from m in context.Movie select m;
        if (!string.IsNullOrEmpty(searchString))
        {
            movies = movies.Where(s => s.Title!.Contains(searchString));
        }
        if (!string.IsNullOrEmpty(movieGenre))
        {
            movies = movies.Where(x => x.Genre == movieGenre);
        }
        var movieGenreVM = new MovieGenreViewModel
        {
            Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
            Movies = await movies.ToListAsync()
        };
        return View(movieGenreVM);
    }

    // GET: Movies/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || context.Movie == null)
        {
            return NotFound();
        }

        var movie = await context.Movie
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IFormFile image, [Bind("Id,Title,ReleaseDate,Genre,Imdb,Rating,Image,Trailer,Description,Director")] Movie movie)
    {
        ImageError(image);
        if (ModelState.IsValid)
        {
            SaveImage(image, movie);
            TrailerUrl(movie);
            context.Add(movie);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(movie);
    }

    // GET: Movies/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || context.Movie == null)
        {
            return NotFound();
        }

        var movie = await context.Movie.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    // POST: Movies/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(IFormFile? image, int id, [Bind("Id,Title,ReleaseDate,Genre,Imdb,Rating,Image,Trailer,Description,Director")] Movie movie)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }

        ImageError(image);
        if (ModelState.IsValid)
        {
            try
            {
                SaveImage(image, movie);
                TrailerUrl(movie);
                context.Update(movie);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(movie.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(movie);
    }

    // GET: Movies/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || context.Movie == null)
        {
            return NotFound();
        }

        var movie = await context.Movie
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: Movies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (context.Movie == null)
        {
            return Problem("Entity set 'MovieAppContext.Movie'  is null.");
        }
        var movie = await context.Movie.FindAsync(id);
        if (movie != null)
        {
            DeleteImage(movie);
            context.Movie.Remove(movie);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MovieExists(int id)
    {
        return (context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    public async void SaveImage(IFormFile? image, Movie movie)
    {
        if (image != null)
        {
            DeleteImage(movie);
            string path = "image/" + movie.Title!.Replace(" ", "") + "Cover" + Path.GetExtension(image.FileName);
            using var fileStream = new FileStream(webHostEnvironment.WebRootPath + "/" + path, FileMode.Create);
            await image.CopyToAsync(fileStream);
            movie.Image = path;
        }
    }

    public void DeleteImage(Movie movie)
    {
        string path = webHostEnvironment.WebRootPath + "/" + movie.Image;
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }

    public void ImageError(IFormFile? image)
    {
        if (image != null)
        {
            if (!string.Equals(image.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(image.ContentType, "image/png", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(image.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Image", "The file must be an image (.jpg, .png, .jpeg)");
            }
        }
    }

    public void TrailerUrl(Movie movie)
    {
        if (movie.Trailer != null)
        {
            string trailerUrl = movie.Trailer!.Replace("https://www.youtube.com/watch?v=", "https://www.youtube.com/embed/");
            try
            {
                trailerUrl = trailerUrl.Remove(trailerUrl.IndexOf('&'), trailerUrl.Length - trailerUrl.IndexOf('&'));
                movie.Trailer = trailerUrl;
            }
            catch
            {
                movie.Trailer = trailerUrl;
            }
        }

    }
}