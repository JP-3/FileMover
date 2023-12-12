using System.Text.RegularExpressions;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace FileMover
{
    internal class Movies
    {
        public string MoveFile(string fullFilePath, string fileName, int year)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fullFilePath);
                double fileSizeGB = fileInfo.Length / 1024 / 1024 / 1024;

                if (!fileName.ToLower().Contains("sample") || fileInfo.Length > 500000000)
                {
                    int yearIndex;

                    //Find the year index and remove everything after leaving you with the movie name... Hopefully
                    if (Regex.Match(fileName, Base.pattern2000s).Success)
                    {
                        yearIndex = Regex.Match(fileName, Base.pattern2000s).Index;
                    }
                    else //pattern1900s
                    {
                        yearIndex = Regex.Match(fileName, Base.pattern1900s).Index;
                    }
                    string movieName = fileName.Remove(yearIndex).Replace(".", " ");

                    TMDbClient client = new TMDbClient(Base.data[PropertiesEnum.apiKey.ToString()]);
                    SearchContainer<SearchMovie> movie = new SearchContainer<SearchMovie>();
                    movie = client.SearchMovieAsync(movieName, 0, false, year, null, year).Result;
                    if (movie.TotalResults == 0)
                    {
                        movie = client.SearchMovieAsync(movieName).Result;
                    }

                    //Get the genre ID, 10751 is family
                    var genre = movie.Results.FirstOrDefault().GenreIds;

                    Console.WriteLine($"Starting Copy {fileName}");

                    if (genre.Contains(10751)) //Kids Movies
                    {
                        if (!File.Exists($"{Base.data[PropertiesEnum.KidsMovies.ToString()]}{fileName}"))
                        {
                            File.Copy(fullFilePath, $@"{Base.data[PropertiesEnum.KidsMovies.ToString()]}{fileName}", false);
                            Console.WriteLine($"File {fileName} moved to Kids Movies");
                            return $"{fileName} moved to Kids Movies";
                        }
                    }
                    else if (fileName.ToLower().Contains("2160p") || fileSizeGB > 16)
                    {
                        if (!File.Exists($"{Base.data[PropertiesEnum.Movies4K.ToString()]}{fileName}"))
                        {
                            File.Copy(fullFilePath, $@"{Base.data[PropertiesEnum.Movies4K.ToString()]}{fileName}", false);
                            Console.WriteLine($"File {fileName} moved to 4K Movies");
                            return $"{fileName} moved to 4K Movies";
                        }
                    }
                    else
                    {
                        if (!File.Exists($"{Base.data[PropertiesEnum.Movies.ToString()]}{fileName}"))
                        {
                            File.Copy(fullFilePath, $@"{Base.data[PropertiesEnum.Movies.ToString()]}{fileName}", false);
                            Console.WriteLine($"File {fileName} moved to Movies");
                            return $"{fileName} moved to Movies";
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Movie Copy Failed, {fileName}\r\n{ex}");
                return $"Movie Copy Failed, {fileName}\r\n{ex}";
            }
        }
    }
}

