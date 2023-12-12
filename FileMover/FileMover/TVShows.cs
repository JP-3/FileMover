using System.Text.RegularExpressions;

namespace FileMover
{
    internal class TVShows
    {
        public string MoveFile(string fullFilePath, string fileName, string episode)
        {
            try
            {
                string name = string.Empty;
                string season = string.Empty;

                if (Regex.Match(fileName, Base.patternSeason1).Success)
                {
                    season = episode.Substring(1, 2);
                }
                else
                {
                    season = $"0{episode.Substring(1, 1)}";
                }
                name = fileName.Remove(fileName.IndexOf(episode)).Replace(".", " ").TrimEnd();

                var tvShow = Base.data[PropertiesEnum.TV.ToString()];
                var tvFile = Path.GetFileName(fullFilePath);

                Console.WriteLine($"Starting Copy {tvFile}");

                if (!File.Exists(@$"{Base.data[PropertiesEnum.TV.ToString()]}{name}\Season {season}\{tvFile}"))
                {
                    File.Copy(fullFilePath, @$"{Base.data[PropertiesEnum.TV.ToString()]}{name}\Season {season}\{tvFile}");
                    Console.WriteLine($"Finished Copy {tvFile}");
                    return @$"{Base.data[PropertiesEnum.TV.ToString()]}{name}\Season {season}\{tvFile}";
                }
                return string.Empty;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Copy Failed {fileName}\r\n{ex}");
                return $"Copy Failed {fileName}\r\n{ex}";
            }
        }
    }
}
