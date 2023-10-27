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
                TMDbClient client = new TMDbClient(File.ReadAllText(@"C:\\Plex\Key.txt"));

                SearchContainer<SearchMovie> movie = new SearchContainer<SearchMovie>();

                movie = client.SearchMovieAsync(name.TrimStart(), 0, false, year, null, year).Result;
                if (movie.TotalResults == 0)
                {
                    movie = client.SearchMovieAsync(name.TrimStart()).Result;
                }

                //Get the genre ID, 10751 is family
                var genre = movie.Results.FirstOrDefault().GenreIds;
                try
                {
                    Console.WriteLine("Starting Copy");
                    var movedLocation = string.Empty;
                    if (genre.Contains(10751)) //Kids Movies
                    {
                        File.Copy(fullFilePath, $@"\\ripper\Kids Movies\{fileName}", false);
                        movedLocation = "Kids Movies";
                    }
                    else if (fileName.ToLower().Contains("2160p") || fileSizeGB > 16)
                    {
                        File.Copy(fullFilePath, $@"\\ripper\4k2\{fileName}", false);
                        movedLocation = "4K Movies";
                    }
                    else
                    {
                        File.Copy(fullFilePath, $@"\\ripper\Movies\{fileName}", false);
                        movedLocation = "Movies";
                    }
                    CheckFileHasCopied(fullFilePath);
                    Console.WriteLine($"File {fileName} moved to {movedLocation}");
                }
                catch
                {
                    //Swallow the error if file already exists
                    Console.WriteLine($"Skipped file {fileName}");
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
                Console.WriteLine("Not Done");
                return CheckFileHasCopied(FilePath);
            }
        }
    }
}
