using FileMover;
using System.Text.RegularExpressions;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

string pattern1900s = @"\.19\d\d\.";
string pattern2000s = @"\.20\d\d\.";
string patternTvShow1 = @"S\d\dE\d\d"; //S10EE11
string patternTvShow2 = @"S\d\dE\d";   //S10E1
string patternTvShow3 = @"S\dE\d\d";   //S1E11
string patternTvShow4 = @"S\dE\d\d";   //S1E1

string regexAnySpaces = @"\s";
//string filePath = @"C:\Users\sneaker\FinishedTorrents";
//string movies = @"\\ripper\Movies";
//string copiedFiles = "C:\\Users\\sneaker\\Documents\\FileCopier\\FileCopier\\CopiedFiles.txt";

string filePath = @"C:\Users\JP\Documents\FileCopier\FinishedTorrents";
string moviesPath = @"C:\Users\JP\Documents\FileCopier\Movies";
string copiedFiles = @"C:\Users\JP\source\repos\FileCopier\FileCopier\copiedfiles.txt";

//var directories = Directory.GetDirectories(filePath);


//if (File.ReadAllText(copiedFiles) == string.Empty)
//{
//foreach (var folder in directories)
//{
//    foreach (var file in Directory.GetFiles(folder))
//    {

//        string fileName = Path.GetFileName(file);
//        if (fileName.EndsWith(".mkv"))
//        {



//            //File.AppendAllText(copiedFiles, $"{DateTime.Now} - {fileName}\r\n");
//        }
//    }
//}
//}
Dictionary<string, bool> processedFiles = new Dictionary<string, bool>();
while (true)
{
    var directories = Directory.GetDirectories(filePath);


    foreach (var folder in directories)
    {
        processedFiles.TryAdd(Path.GetFileName(folder), false);
        bool value;
        processedFiles.TryGetValue(Path.GetFileName(folder), out value);
        if (!value)
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                string fileName = Path.GetFileName(file).ToLower();
                if (fileName.EndsWith(".mkv") || fileName.EndsWith(".mp4") || fileName.EndsWith(".api"))
                {
                    fileName = fileName.Replace(" ", ".").Replace("(", ".").Replace(")", ".");

                    Movies movies = new Movies();
                    if (Regex.Match(fileName, pattern2000s).Success)
                    {
                        var year = int.Parse(Regex.Match(fileName, pattern2000s).Value.Replace(".", ""));
                        movies.MoveFile(file, fileName, year);
                    }
                    else if (Regex.Match(fileName, pattern1900s).Success)
                    {
                        var year = int.Parse(Regex.Match(fileName, pattern1900s).Value.Replace(".", ""));
                        movies.MoveFile(file, fileName, year);
                    }
                }
            }

        }
        processedFiles[Path.GetFileName(folder)] = true;
    }

}

//var files = File.ReadAllText(copiedFiles);
//if (!files.Contains(fileName))
//{

//Match match1900s;
//Match match2000s;
//Match test;
//if (Regex.IsMatch(fileName, patternTvShow1, RegexOptions.IgnoreCase) ||
//    Regex.IsMatch(fileName, patternTvShow2, RegexOptions.IgnoreCase) ||
//    Regex.IsMatch(fileName, patternTvShow3, RegexOptions.IgnoreCase) ||
//    Regex.IsMatch(fileName, patternTvShow4, RegexOptions.IgnoreCase))
//{
//    Console.WriteLine($"{fileName}\r\n");
//}
//else
//{
//Remove the search for . in the pattern match in case it's spaces in file name
//if (Regex.Match(fileName, regexAnySpaces).Success)
//{
//    match1900s = Regex.Match(fileName, pattern1900s = pattern1900s.Replace(@"\.", ""));
//    match2000s = Regex.Match(fileName, pattern2000s = pattern2000s.Replace(@"\.", ""));
//}
//else
//{
//    match1900s = Regex.Match(fileName, pattern1900s);
//    match2000s = Regex.Match(fileName, pattern2000s);
//}

//if (fileName.EndsWith(".mkv"))
//{
//    if (match1900s.Success || match2000s.Success)
//    {
//        string name = string.Empty;
//        int year = 0;
//        var split = fileName.Split('.');
//        foreach (var item in fileName.Split('.'))
//        {


//            if (Regex.Match(item, pattern1900s.Replace(@"\.", "")).Success || Regex.Match(item, pattern2000s.Replace(@"\.", "")).Success)
//            {
//                year = int.Parse(item);
//                break;
//            }
//            name = $"{name} {item}";
//        }

//        TMDbClient client = new TMDbClient("9a6e847130f69fd3a0add60f9a24a3c0");
//        //var movie = client.SearchMovieAsync(name.TrimStart(), 0, false, year, null, year).Result;
//        var movie2 = client.SearchMovieAsync("The Little Mermaid", 0, false, 2023, null, 2023).Result;
//        var genre = movie2.Results.FirstOrDefault().GenreIds;
//        var ll = client.GetMovieAsync(447277).Result;
//        var v = client.GetMovieGenresAsync().Result; //10751 family,

//        var vv = client.GetMovieReleaseDatesAsync(447277).Result;

//        foreach (var item in vv.Results)
//        {
//            if (item.Iso_3166_1 == "US")
//            {
//                var g = item.ReleaseDates.FirstOrDefault(a => a.Type == ReleaseDateType.Physical).Certification;

//            }
//        }

//Console.WriteLine($"Movie name: {movie.Title}");

//try
//{
//    //System.Threading.Thread.Sleep(30000);

//    File.Copy(file, Path.Combine($"{movies}\\{fileName}"), false);
//    File.AppendAllText(copiedFiles, $"{DateTime.Now} - {fileName}\r\n");
//}
//catch
//{
//    File.AppendAllText(copiedFiles, $"COPY FAIL {DateTime.Now} - {fileName}\r\n");
//}
//    }
//    }
//}
//}
//    }
//    //System.Threading.Thread.Sleep(60000);
//}