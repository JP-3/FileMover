using System.Diagnostics;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace FileMover
{
    internal class Movies
    {
        public void MoveFile(string fullFilePath, string fileName, int year)
        {
            FileInfo fileInfo = new FileInfo(fullFilePath);
            double fileSizeGB = fileInfo.Length / 1024 / 1024 / 1024;
            if (!fileName.ToLower().Contains("sample") || fileInfo.Length > 500000000)
            {
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
                TMDbClient client = new TMDbClient(Base.data[PropertiesEnum.apiKey.ToString()]);

                SearchContainer<SearchMovie> movie = new SearchContainer<SearchMovie>();
                movie = client.SearchMovieAsync(name.TrimStart(), 0, false, year, null, year).Result;
                if (movie.TotalResults == 0)
                {
                    movie = client.SearchMovieAsync(name.TrimStart()).Result;
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
                        }
                    }
                }
                catch(Exception ex)
                {
                    //Swallow the error if file already exists
                    Console.WriteLine($"File Exists, skipped file {fileName}\r\n{ex}");
                }
            }
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
