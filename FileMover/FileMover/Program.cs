using FileMover;
using System.Text.RegularExpressions;
using MyEmails;
using System.Text;

Base.GetProperties();
Dictionary<string, bool> processedFiles = new Dictionary<string, bool>();

// Watch for any new folders
FileSystemWatcher watcher = new FileSystemWatcher();
watcher.Path = Base.data[PropertiesEnum.FinishedTorrents.ToString()];
watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;
watcher.Filter = "*.*";

// Add event handlers
watcher.Created += new FileSystemEventHandler(OnChanged);
watcher.EnableRaisingEvents = true;
watcher.IncludeSubdirectories = false;

try
{
    while (true) { Thread.Sleep(2000); } //infinite loop
}
catch (Exception ex)
{
    Email email = new Email();
    email.SendEmail("FileMover Crashed", ex.ToString());
}

static void OnChanged(object source, FileSystemEventArgs e)
{
    Email email = new Email();
    List<string> movedFiles = new List<string>();

    try
    {
        string filePath = Base.data[PropertiesEnum.FinishedTorrents.ToString()];
        var directories = Directory.GetDirectories(filePath);

        //Check if any files showed up at top level, sometimes movies do that
        movedFiles = CopyFiles(movedFiles, filePath);

        //Then check each folder and subfolder for files
        foreach (var directory in directories)
        {
            if (!directory.Contains("New folder"))
            {
                Base.processedFiles.TryAdd(Path.GetFileName(directory), false);
                bool value;
                Base.processedFiles.TryGetValue(Path.GetFileName(directory), out value);

                if (Directory.GetDirectories(filePath).Length > 0)
                {

                    //If it's a .rar'd thing then look to see if it's beeen unrared yet otherwise unrar it
                    if (Directory.GetFiles(directory, "*.mkv").Length < 1 && Directory.GetFiles(directory, "*.r00").Length == 1)
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        using (process)
                        {
                            process.StartInfo.CreateNoWindow = true;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.FileName = "\"C:\\Program Files\\WinRAR\\winrar.exe\"";
                            process.StartInfo.Arguments = string.Format(@"x -s -o- ""{0}"" *.* ""{1}\""", Directory.GetFiles(directory, "*.r00").First(), Path.GetFullPath(directory));
                            process.Start();
                            process.WaitForExit();
                        }
                        Console.WriteLine($"Unzipped file in {directory}");
                    }

                    if (!value)
                    {
                        movedFiles = MoveFiles(movedFiles, directory);
                    }
                }
                else
                {
                    movedFiles = CopyFiles(movedFiles, directory);
                }
                Base.processedFiles[Path.GetFileName(directory)] = true;
            }
        }
    }
    catch (Exception ex)
    {
        email.SendEmail("FileMover Crashed", ex.ToString());
    }

    for (int i = movedFiles.Count-1; i > 0; i--)
    {
        if (movedFiles[i] == string.Empty)
        {
            movedFiles.Remove(movedFiles[i]);
        }
    } 

    string formattedFiles = String.Join($"\r\n", movedFiles);
    if (formattedFiles != string.Empty)
    {
        Console.WriteLine($"FileMover Finished \r\n {formattedFiles}");
        email.SendEmail("FileMover Finished", formattedFiles);
    }
}


static List<string> MoveFiles(List<string> movedFiles, string folder)
{

    //Recursive to get to the lowest folder for the files. Sometimes you have Full Shows with ShowName>Season1>Episodes. or multiple seasons.
    //If you just have one level it's usually just a movie or a single episode, movies sometimes have sample subfolder but 
    if (Directory.GetDirectories(folder).Length > 0)
    {
        var subFolder = Directory.GetDirectories(folder);
        foreach (var newFolder in subFolder)
        {
            MoveFiles(movedFiles, newFolder);
        }
    }
    CopyFiles(movedFiles, folder);
    return movedFiles;
}

static List<string> CopyFiles(List<string> movedFiles, string folder)
{
    foreach (var file in Directory.GetFiles(folder))
    {
        string fileName = Path.GetFileName(file).ToLower();
        if (fileName.EndsWith(".mkv") || fileName.EndsWith(".mp4") || fileName.EndsWith(".api"))
        {
            fileName = fileName.Replace(" ", ".").Replace("(", ".").Replace(")", ".");

            Movies movies = new Movies();
            TVShows tvShows = new TVShows();

            if (Regex.Match(fileName, Base.patternTvShow1).Success || Regex.Match(fileName, Base.patternTvShow2).Success ||
                    Regex.Match(fileName, Base.patternTvShow3).Success || Regex.Match(fileName, Base.patternTvShow4).Success)
            {
                string episode = string.Empty;
                if (Regex.Match(fileName, Base.patternTvShow1).Success)
                {
                    episode = Regex.Match(fileName, Base.patternTvShow1).Value.Replace(".", "");
                }
                else if (Regex.Match(fileName, Base.patternTvShow2).Success)
                {
                    episode = Regex.Match(fileName, Base.patternTvShow2).Value.Replace(".", "");
                }
                else if (Regex.Match(fileName, Base.patternTvShow3).Success)
                {
                    episode = Regex.Match(fileName, Base.patternTvShow3).Value.Replace(".", "");
                }
                else
                {
                    episode = Regex.Match(fileName, Base.patternTvShow4).Value.Replace(".", "");
                }
                movedFiles.Add(tvShows.MoveFile(file, fileName, episode));
            }
            else if (Regex.Match(fileName, Base.pattern2000s).Success)
            {
                var year = int.Parse(Regex.Match(fileName, Base.pattern2000s).Value.Replace(".", ""));
                movedFiles.Add(movies.MoveFile(file, fileName, year));
            }
            else if (Regex.Match(fileName, Base.pattern1900s).Success)
            {
                var year = int.Parse(Regex.Match(fileName, Base.pattern1900s).Value.Replace(".", ""));
                movedFiles.Add(movies.MoveFile(file, fileName, year));
            }
        }
    }
    return movedFiles;
}