using FileMover;
using System.Text.RegularExpressions;
using MyEmails;

Base.GetProperties();
FileSystemWatcher watcher = new FileSystemWatcher();
watcher.Path = Base.data[PropertiesEnum.FinishedTorrents.ToString()];

// Watch for all changes specified in the NotifyFilters  
//enumeration.  
watcher.NotifyFilter = NotifyFilters.Attributes |
NotifyFilters.CreationTime |
NotifyFilters.DirectoryName |
NotifyFilters.FileName |
NotifyFilters.LastAccess |
NotifyFilters.LastWrite |
NotifyFilters.Security |
NotifyFilters.Size;
// Watch all files.  
watcher.Filter = "*.*";
Dictionary<string, bool> processedFiles = new Dictionary<string, bool>();

// Add event handlers.  
watcher.Created += new FileSystemEventHandler(OnChanged);
watcher.IncludeSubdirectories = true;
watcher.EnableRaisingEvents = true;
while (true) { System.Threading.Thread.Sleep(60000); } //infinite loop

static void OnChanged(object source, FileSystemEventArgs e)
{
    Email email = new Email();
    email.SendEmail("FileMover Started", "");
    //System.Threading.Thread.Sleep(10000);
    string pattern1900s = @"\.19\d\d\.";
    string pattern2000s = @"\.20\d\d\.";
    string patternTvShow1 = @"s\d\de\d\d"; //S10EE11
    string patternTvShow2 = @"s\d\de\d";   //S10E1
    string patternTvShow3 = @"s\de\d\d";   //S1E11
    string patternTvShow4 = @"s\de\d\d";   //S1E1
    string filePath = Base.data[PropertiesEnum.FinishedTorrents.ToString()]; ;
    var directories = Directory.GetDirectories(filePath);

    foreach (var folder in directories)
    {
        if (Directory.GetFiles(folder).Count() != 0)
        {
            Base.processedFiles.TryAdd(Path.GetFileName(folder), false);
            bool value;
            Base.processedFiles.TryGetValue(Path.GetFileName(folder), out value);
            //If it's a .rar'd thing then look to see if it's beeen unrared yet otherwise unrar it
            if (Directory.GetFiles(folder, "*.mkv").Length < 1 && Directory.GetFiles(folder, "*.r00").Length == 1)
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = "\"C:\\Program Files\\WinRAR\\winrar.exe\"";
                p.StartInfo.Arguments = string.Format(@"x -s -o- ""{0}"" *.* ""{1}\""", Directory.GetFiles(folder, "*.r00").First(), Path.GetFullPath(folder));
                p.Start();
                p.WaitForExit();
                Console.WriteLine($"Unzipped file in {folder}");
            }

            if (!value)
            {
                foreach (var file in Directory.GetFiles(folder))
                {
                    string fileName = Path.GetFileName(file).ToLower();
                    if (fileName.EndsWith(".mkv") || fileName.EndsWith(".mp4") || fileName.EndsWith(".api"))
                    {
                        fileName = fileName.Replace(" ", ".").Replace("(", ".").Replace(")", ".");

                        Movies movies = new Movies();
                        TVShows tvShows = new TVShows();

                        if (Regex.Match(fileName, patternTvShow1).Success || Regex.Match(fileName, patternTvShow2).Success ||
                                Regex.Match(fileName, patternTvShow3).Success || Regex.Match(fileName, patternTvShow4).Success)
                        {
                            string episode = string.Empty;
                            if (Regex.Match(fileName, patternTvShow1).Success)
                            {
                                episode = Regex.Match(fileName, patternTvShow1).Value.Replace(".", "");
                            }
                            else if (Regex.Match(fileName, patternTvShow2).Success)
                            {
                                episode = Regex.Match(fileName, patternTvShow2).Value.Replace(".", "");
                            }
                            else if (Regex.Match(fileName, patternTvShow3).Success)
                            {
                                episode = Regex.Match(fileName, patternTvShow3).Value.Replace(".", "");
                            }
                            else
                            {
                                episode = Regex.Match(fileName, patternTvShow4).Value.Replace(".", "");
                            }
                            tvShows.MoveFile(file, fileName, episode);
                        }
                        else if (Regex.Match(fileName, pattern2000s).Success)
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
        }
        Base.processedFiles[Path.GetFileName(folder)] = true;
    }
    email.SendEmail("FileMover Finished", "");
}
