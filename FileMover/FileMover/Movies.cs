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
            FileInfo fileInfo = new FileInfo(fullFilePath);
            double fileSizeGB = fileInfo.Length / 1024 / 1024 / 1024;
            string returnString = string.Empty;

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

                try
                {
                    //Get the genre ID, 10751 is family
                    var genre = movie.Results.FirstOrDefault().GenreIds;

                    Console.WriteLine($"Starting Copy {fileName}");

                    if (genre.Contains(10751)) //Kids Movies
                    {
                        if (CheckFileHasCopied($"{Base.data[PropertiesEnum.KidsMovies.ToString()]}{fileName}"))
                        {
                            Console.WriteLine($"File {fileName} location Kids Movies already exists");
                        }
                        else
                        {
                            File.Copy(fullFilePath, $@"{Base.data[PropertiesEnum.KidsMovies.ToString()]}{fileName}", false);
                            Console.WriteLine($"File {fileName} moved to Kids Movies");
                            returnString = $"File {fileName} moved to Kids Movies";
                        }
                    }
                    else if (fileName.ToLower().Contains("2160p") || fileSizeGB > 16)
                    {
                        if (CheckFileHasCopied($"{Base.data[PropertiesEnum.Movies4K.ToString()]}{fileName}"))
                        {
                            Console.WriteLine($"File {fileName} location 4K Movies already exists");
                        }
                        else
                        {
                            File.Copy(fullFilePath, $@"{Base.data[PropertiesEnum.Movies4K.ToString()]}{fileName}", false);
                            Console.WriteLine($"File {fileName} moved to 4K Movies");
                            returnString = $"File {fileName} moved to 4K Movies";
                        }
                    }
                    else
                    {
                        if (CheckFileHasCopied($"{Base.data[PropertiesEnum.Movies.ToString()]}{fileName}"))
                        {
                            Console.WriteLine($"File {fileName} location Movies already exists");
                        }
                        else
                        {
                            File.Copy(fullFilePath, $@"{Base.data[PropertiesEnum.Movies.ToString()]}{fileName}", false);
                            Console.WriteLine($"File {fileName} moved to Movies");
                            returnString = $"File {fileName} moved to Movies";
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Swallow the error if file already exists
                    Console.WriteLine($"File Exists, skipped file {fileName}\r\n{ex}");
                }
            }
            return returnString;
        }


        private bool CheckFileHasCopied(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                    using (File.OpenRead(FilePath))
                    {
                        return true;
                    }
                else
                    return false;
            }
            catch (Exception)
            {
                Thread.Sleep(100);
                return CheckFileHasCopied(FilePath);
            }
        }
    }
}
