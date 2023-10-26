using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace FileMover
{
    internal class Movies
    {
        public void MoveFile(string fullFilePath, string fileName, int year)
        {
            string pattern1900s = @"\.19\d\d\.";
            string pattern2000s = @"\.20\d\d\.";

            FileInfo fileInfo = new FileInfo(fullFilePath);
            double fileSizeGB = fileInfo.Length; // 1024 / 1024 / 1024;

            string name = string.Empty;
            var split = fileName.Split('.');
            foreach (var item in split)
            {
                //If it's an integer it's either the year or maybe 1080P/2160P
                if (int.TryParse(item, out _))
                {
                    break;
                }
                name = $"{name} {item}";
                string movieYear = string.Empty;
            }

            TMDbClient client = new TMDbClient("");
            var movie = client.SearchMovieAsync(name.TrimStart(), 0, false, year, null, year).Result;
            //var movie2 = client.SearchMovieAsync("The Little Mermaid", 0, false, 2023, null, 2023).Result;

            //Assume first one is correct movie and get the ID
            var movieID = movie.Results.FirstOrDefault().Id;

            //Get the genre ID, 10751 is family
            var genre = movie.Results.FirstOrDefault().GenreIds;

            //878 53
            //var ll = client.GetMovieAsync(447277).Result;
            //var v = client.GetMovieGenresAsync().Result; //10751 family,

            //foreach (var item in v)
            //{
            //    Console.WriteLine($"{item.Name} {item.Id}");
            //}

            var movieReleaseDate = client.GetMovieReleaseDatesAsync(movieID).Result;


            //Loop throught he movie releases and take teh first US one to get the rating... ie PG, PG13, R....
            foreach (var item in movieReleaseDate.Results)
            {
                if (item.Iso_3166_1 == "US")
                {
                    var rating = item.ReleaseDates.FirstOrDefault().Certification.ToUpper();

                    if (rating == "G" || rating == "PG" || genre.Contains(10751))
                    {
                        Console.WriteLine($"{fileName} Rating {rating}, Genre {genre}");
                    }
                    else
                    {
                        Console.WriteLine($"{fileName} Rating {rating}, Genre Not Family");
                    }
                }
            }
        }
    }
}
