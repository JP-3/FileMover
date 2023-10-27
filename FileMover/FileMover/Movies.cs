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
                if (genre.Contains(10751)) //Kids Movies
                {
                    File.Copy(fullFilePath, $@"C:\Users\JP\Documents\FileCopier\Movies\{fileName}", false);
                }
                else
                {
                    File.Copy(fullFilePath, $@"C:\Users\JP\Documents\FileCopier\Movies\{fileName}", false);
                }
            }
            catch { }
        }
    }
}
