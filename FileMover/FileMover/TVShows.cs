using MyEmails;
using System.ComponentModel.Design;
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

                int yearIndex;

                if (Regex.Match(fileName, Base.patternSeason1).Success)
                {
                    season = episode.Substring(1, 2);
                    name = fileName.Remove(fileName.IndexOf(episode)).Replace(".", "").TrimEnd();
                }
                else if (Regex.Match(fileName, Base.patternSeason2).Success)
                {
                    season = $"0{episode.Substring(1, 1)}";
                    name = fileName.Remove(fileName.IndexOf(episode)).Replace(".", "").TrimEnd();
                }

                //Find the year index and remove everything after leaving you with the movie name... Hopefully
                if (Regex.Match(name, Base.pattern2000sNoPeriod).Success)
                {
                    yearIndex = Regex.Match(fileName, Base.pattern2000sNoPeriod).Index;
                    name = fileName.Remove(yearIndex).Replace(".", " ");
                }
                else if (Regex.Match(name, Base.pattern1900sNoPeriod).Success)
                {
                    yearIndex = Regex.Match(fileName, Base.pattern1900sNoPeriod).Index;
                    name = fileName.Remove(yearIndex).Replace(".", " ");
                }
                name = name.First().ToString().ToUpper() + name.Substring(1).Trim();
                var tvShow = Base.data[PropertiesEnum.TV.ToString()];
                var tvFile = Path.GetFileName(fullFilePath);

                Console.WriteLine($"Starting Copy {tvFile}");
                if (!Directory.Exists(@$"{Base.data[PropertiesEnum.TV.ToString()]}{name}\Season {season}"))
                {
                    Email email = new Email();
                    email.SendEmail("Folder Created", @$"{Base.data[PropertiesEnum.TV.ToString()]}{name}\Season {season}");
                    Directory.CreateDirectory(@$"{Base.data[PropertiesEnum.TV.ToString()]}{name}\Season {season}");
                }

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
