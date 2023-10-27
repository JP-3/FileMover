using FileMover;
using System.Text.RegularExpressions;


FileSystemWatcher watcher = new FileSystemWatcher();
watcher.Path = @"C:\Users\sneaker\FinishedTorrents";
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
// Add event handlers.  
watcher.Created += new FileSystemEventHandler(OnChanged);
watcher.IncludeSubdirectories = true;
watcher.EnableRaisingEvents = true;

while (true) { System.Threading.Thread.Sleep(1000); } //infinite loop

static void OnChanged(object source, FileSystemEventArgs e)
{
    //System.Threading.Thread.Sleep(10000);
    string pattern1900s = @"\.19\d\d\.";
    string pattern2000s = @"\.20\d\d\.";
    string patternTvShow1 = @"S\d\dE\d\d"; //S10EE11
    string patternTvShow2 = @"S\d\dE\d";   //S10E1
    string patternTvShow3 = @"S\dE\d\d";   //S1E11
    string patternTvShow4 = @"S\dE\d\d";   //S1E1
    string filePath = @"C:\Users\sneaker\FinishedTorrents";

    Dictionary<string, bool> processedFiles = new Dictionary<string, bool>();

    var directories = Directory.GetDirectories(filePath);


    foreach (var folder in directories)
    {
        processedFiles.TryAdd(Path.GetFileName(folder), false);
        bool value;
        processedFiles.TryGetValue(Path.GetFileName(folder), out value);
        //If it's a .rar'd thing then look to see if it's beeen unrared yet otherwise unrar it
        var vv = Directory.GetFiles(folder, "*.r00").Length == 1;
        var aa = Directory.GetFiles(folder, "*.mkv").Length < 1;
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
        Console.WriteLine($"Processed folder {folder}");
    }
}