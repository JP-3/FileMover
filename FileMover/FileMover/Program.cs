using FileMover;
using System.Text.RegularExpressions;
using MyEmails;
using System.Text;

Base.GetProperties();
Dictionary<string, bool> processedFiles = new Dictionary<string, bool>();

// Watch for any new folders
FileSystemWatcher watcher = new FileSystemWatcher();
watcher.Path = Base.data[PropertiesEnum.FinishedTorrents.ToString()];
watcher.NotifyFilter = NotifyFilters.DirectoryName; 
watcher.Filter = "*.*";

// Add event handlers
watcher.Created += new FileSystemEventHandler(OnChanged);
watcher.EnableRaisingEvents = true;

try
{
    while (true) { System.Threading.Thread.Sleep(2000); } //infinite loop
}
catch (Exception ex)
{
    Email email = new Email();
    email.SendEmail("FileMover Crashed", ex.ToString());
}

static void OnChanged(object source, FileSystemEventArgs e)
{
    Email email = new Email();
    StringBuilder movedFiles = new StringBuilder();

    try
    {
        email.SendEmail("FileMover Started", e.Name);

        string filePath = Base.data[PropertiesEnum.FinishedTorrents.ToString()];
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
                                movedFiles.AppendLine(tvShows.MoveFile(file, fileName, episode));
                            }
                            else if (Regex.Match(fileName, Base.pattern2000s).Success)
                            {
                                var year = int.Parse(Regex.Match(fileName, Base.pattern2000s).Value.Replace(".", ""));
                                movedFiles.AppendLine(movies.MoveFile(file, fileName, year));
                            }
                            else if (Regex.Match(fileName, Base.pattern1900s).Success)
                            {
                                var year = int.Parse(Regex.Match(fileName, Base.pattern1900s).Value.Replace(".", ""));
                                movedFiles.AppendLine(movies.MoveFile(file, fileName, year));
                            }
                        }
                    }
                }
            }
            Base.processedFiles[Path.GetFileName(folder)] = true;
        }
    }
    catch (Exception ex)
    {
        email.SendEmail("FileMover Crashed", ex.ToString());
    }
    Console.WriteLine(movedFiles.Replace("\r\n", "").Replace("Copied to", "\r\n").ToString());
    string files = movedFiles.Replace("\r\n", "").Replace("Copied to", "\r\n").ToString();

    if (files != string.Empty)
    {
        email.SendEmail("FileMover Finished", files);
        Console.WriteLine($"FileMover Finished \r\n {files}");
    }
    else
    {
        email.SendEmail("FileMover Finished Couldn't move file", e.Name);
        Console.WriteLine($"\"FileMover Finished Couldn't move file \r\n {e.Name}");
    }
}
