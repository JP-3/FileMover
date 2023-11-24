using MyEmails;

namespace FileMover
{
    internal class TVShows
    {
        public void MoveFile(string fullFilePath, string fileName, string episode)
        {
            Email email = new Email();
            var split = fileName.Split('.');
            string name = string.Empty;
            string season = string.Empty;
            foreach (var item in split)
            {
                //If it's an integer it's either the year or maybe 1080P/2160P
                if (item == episode)
                {
                    season = episode.Remove(item.IndexOf("e")).Replace("s", "");
                    if (season.Length == 1)
                    {
                        season = 0 + season;
                    }
                    break;
                }
                name = $"{name} {item}";
            }
            name = name.TrimStart().Replace(".", " ");
            var tvShows = Directory.GetDirectories(Base.data[PropertiesEnum.TV.ToString()]);
            var tvFile = Path.GetFileName(fullFilePath);
            foreach (var tvShow in tvShows)
            {
                Console.WriteLine($"Starting Copy {tvFile}");
                try
                {
                    File.Copy(fullFilePath, @$"{Base.data[PropertiesEnum.TV.ToString()]}{name}\Season {season}\{tvFile}");
                    Console.WriteLine($"Finished Copy {tvFile}");
                    email.SendEmail($"Finished Copy {tvFile}");
                    break;
                }
                catch
                {
                    File.AppendAllText(Base.data[PropertiesEnum.LogFile.ToString()], 
                        $"Copy Failed {Base.data[PropertiesEnum.TV.ToString()]}{name}\\Season {season}\\{tvFile}\r\n");

                    Console.WriteLine($"Copy Failed {Base.data[PropertiesEnum.TV.ToString()]}{name}\\Season {season}\\{tvFile}");
                    break;
                }
            }
        }
    }
}
