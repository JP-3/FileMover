using MyEmails;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace FileMover
{
    internal class TVShows
    {
        public string MoveFile(string fullFilePath, string fileName, string episode)
        {
            string returnString = string.Empty;
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
            //foreach (var tvShow in tvShows)
            //{
                Console.WriteLine($"Starting Copy {tvFile}");
                try
                {
                    File.Copy(fullFilePath, @$"{Base.data[PropertiesEnum.TV.ToString()]}{name}\Season {season}\{tvFile}");
                    Console.WriteLine($"Finished Copy {tvFile}");
                    return @$"Copied to {Base.data[PropertiesEnum.TV.ToString()]}{name}\Season {season}\{tvFile}";
                }

                catch
                {
                    File.AppendAllText(Base.data[PropertiesEnum.LogFile.ToString()],
                        $"Copy Failed {Base.data[PropertiesEnum.TV.ToString()]}{name}\\Season {season}\\{tvFile}\r\n");

                    Console.WriteLine($"Copy Failed {Base.data[PropertiesEnum.TV.ToString()]}{name}\\Season {season}\\{tvFile}");
                   
                }
            //}
            return returnString;
        }
    }
}
