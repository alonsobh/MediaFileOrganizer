using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MediaFileOrganizerTest.Organize
{
    public class FileManagement
    {
        IFileManager FileManager { get; }

        public FileManagement(IFileManager fileManager)
            => FileManager = fileManager;

        public void MoveFile(string filePath, string unit)
        {
            filePath = ReplaceSpaceWithPoint(filePath);
            var path = filePath.Split("\\");
            var originalFileName = path.Last();
            if (originalFileName.ToUpper().StartsWith("SUB"))
                throw new Exception("Sub");
            var extension = originalFileName.Split(".").Last().ToLower();
            //Extension processor = extension switch
            //{
            //    "txt" => new GenericExtension(filePath, FileManager),
            //    "nfo" => new GenericExtension(filePath, FileManager),
            //    "srt" => new SubtitleExtension(filePath, FileManager),
            //    _ => throw new NotImplementedException(),
            //};
            //;
            //processor.Process(directoryTo);
            //return;

            var fileName = string.Join(".", originalFileName.Split(" "));
            if (fileName.Contains("("))
            {
                var fileSplit = fileName.Split(".");
                for (int i = 0; i < fileSplit.Length; i++)
                {
                    if (!fileSplit[i].Contains("("))
                        continue;
                    var l = fileSplit[i].Length - 1;
                    if (int.TryParse(fileSplit[i][1..l], out _))
                        fileSplit[i] = fileSplit[i].Remove('(').Remove(')');
                }

            }

            fileName = TitleCase(fileName);
            if (fileName.Split(".").Count() > 3
                && !fileName.ToUpper().EndsWith("FULL.ENG.SRT")
                && !fileName.ToUpper().EndsWith("SDH.ENG.SRT"))
                fileName = fileName.Replace(".Eng.srt", ".srt");
            if (fileName.ToUpper().StartsWith("AFI-"))
                return;
            if (fileName.ToUpper().StartsWith("GI6-"))
                return;
            if (fileName.ToUpper().StartsWith("PFA-"))
                return;
            var parentFolder = path[path.Length - 3];
            var currentFolder = path[path.Length - 2];
            var names = fileName.Split(".");
            var first = names.First();
            var secondLast = names.LastOrDefault(s => s != extension);
            var thirdLast = names.LastOrDefault(s => s != extension && s != secondLast);
            var forthLast = names.LastOrDefault(s => s != extension && s != secondLast && s != thirdLast);
            var countRemove = extension.Length + 1;
            if (extension == "nfo")
            {
                return;
            }
            if (extension == "exe")
            {
                return;
            }
            if (extension == "txt")
            {
                return;
            }
            if (IsSubtitle(extension))
            {
                var folder = currentFolder.ToUpper().StartsWith("SUB") ? parentFolder : currentFolder;
                if (IsLanguageExtension(forthLast) && IsLanguageExtension(thirdLast) && IsLanguageExtension(secondLast))//n, extension
                {
                    var sub = $"{forthLast} {thirdLast} {secondLast}";
                    if (currentFolder.ToUpper().StartsWith("SUB"))
                        fileName = $"{parentFolder}.{sub}.{extension}";
                    else if (currentFolder.EndsWith(sub.Replace(" ", ".")))
                        fileName = $"{currentFolder.Substring(0, currentFolder.Length - sub.Length)}.{sub}.{extension}";
                    else
                        fileName = $"{currentFolder}.{sub}.{extension}";
                    countRemove += secondLast.Length + 1 + thirdLast.Length + 1 + forthLast.Length + 1;
                }
                else if (IsLanguageExtension(thirdLast) && IsLanguageExtension(secondLast))//n, extension
                {
                    if (currentFolder.ToUpper().StartsWith("SUB"))
                        fileName = $"{parentFolder}.{thirdLast} {secondLast}.{extension}";
                    else
                        fileName = $"{currentFolder}.{thirdLast} {secondLast}.{extension}";
                    countRemove += secondLast.Length + 1 + thirdLast.Length + 1;
                }
                else if (first == secondLast)
                {
                    if (currentFolder.ToUpper().StartsWith("SUB"))
                        fileName = $"{parentFolder}.{secondLast}.{extension}";
                    else
                        fileName = $"{currentFolder}.{secondLast}.{extension}";
                    countRemove += secondLast.Length + 1;
                }
                else if (!currentFolder.ToUpper().StartsWith("SUB")
                    && !RemoveSimbols(fileName.ToUpper()).StartsWith(RemoveSimbols(folder).Replace(" ", ".").ToUpper().Substring(0, 10)))//n, extension
                {
                    return;
                    throw new Exception($"error en subtitulo {fileName}");
                }
                else if (IsLanguageExtension(secondLast))//n, extension
                {
                    if (secondLast.Split(" ").Count() > 1)
                        throw new Exception($"error en subtitulo {fileName}");
                    countRemove += secondLast.Length + 1;
                }
                else if (!RemoveSimbols(fileName.ToUpper()).EndsWith(RemoveSimbols(folder).Replace(" ", ".").ToUpper().Substring(RemoveSimbols(folder).Length - 10)))//n, extension
                {
                    fileName = fileName.Insert(fileName.Length - extension.Length, "ENG.");
                    countRemove += 4;
                    //return;
                    //throw new Exception($"subtitulo no reconocido {fileName}");
                }
            }
            fileName = TitleCaseOnly(fileName);
            var fileNameSplit = fileName.Split(".");
            var folderName = fileName.Substring(0, fileName.Length - countRemove);
            var folderPath = $"{unit}:\\#Ordenado\\{GetFolderName(folderName)}";
            if (IsSubtitle(extension) && !ExpectedSubs.Any(e => fileNameSplit[fileNameSplit.Length - 2].ToUpper().EndsWith(e)))
            {
                //return;
                folderPath = $"{unit}:\\#Subs\\{GetFolderName(folderName)}";
                var folderSplit = folderPath.Split(".");
                while (OtherSubs.Any(s => folderSplit.Last().ToUpper().EndsWith(s)))
                {
                    var folderLast = folderSplit.Length - 2;
                    folderPath = string.Join(".", folderSplit[0..folderLast]);
                    folderSplit = folderPath.Split(".");
                }
            }
            var folderSingleName = folderPath.Split("\\").Last();
            FileManager.Move(filePath, folderPath, fileName);
        }

        private static bool IsSubtitle(string extension) => extension == "srt"
                        || extension == "ssa"
                        || extension == "ass"
                        || extension == "sub"
                        || extension == "smi";

        private string ReplaceSpaceWithPoint(string filePath)
        {
            if (filePath.Contains(" "))
            {
                var newfilePath = filePath;
                while (newfilePath.Contains("  "))
                    newfilePath = newfilePath.Replace("  ", " ");
                newfilePath = newfilePath.Replace(" ", ".");

                FileManager.Move(filePath, newfilePath);
                filePath = newfilePath;
            }

            return filePath;
        }

        private string TitleCase(string name)
        {
            var t = new CultureInfo("en-US", true).TextInfo;
            var names = name.Split(".");
            var extension = names.Last();
            var nn = new List<string>();
            foreach (var n in names)
                if (n == extension || n.StartsWith("["))
                    nn.Add(n.ToLower());
                else if (new[] { "480P", "720P", "1080P", "2160P" }.Contains(n.ToUpper()))
                    nn.Add(t.ToTitleCase(n.ToLower()));
                else if (n.ToUpper() == "BRRip".ToUpper())
                    nn.Add("BRRip");
                else if (n.Contains("["))
                    nn.Add(ExistsKey(n));
                else
                    nn.Add(t.ToTitleCase(n.ToLower()));

            return string.Join('.', nn);
        }

        private string TitleCaseOnly(string name)
        {
            var t = new CultureInfo("en-US", true).TextInfo;
            var names = name.Split(".");
            var nn = new List<string>();
            foreach (var n in names)
                if (nn.Count == names.Count() - 1)
                    nn.Add(n.ToLower());
                else
                    nn.Add(t.ToTitleCase(n.ToLower()));
            return string.Join('.', nn);
        }
        private object GetFolderName(string name)
            => GetNewDirectory(name, "-Tk")
                ?? GetNewDirectory(name, "480p")
                ?? GetNewDirectory(name, "720p")
                ?? GetNewDirectory(name, "1080p")
                ?? GetNewDirectory(name, "2160p")
                ?? GetNewDirectory(name, "HDTV")
                ?? GetNewDirectory(name, "WEB-DL")
                ?? GetNewDirectory(name, "Webrip")
                ?? GetNewDirectory(name, "Web")
                ?? GetNewDirectory(name, "Dvdrip")
                ?? GetNewDirectory(name, "Bdrip")
                ?? GetNewDirectoryNoFormat(name);

        private string[] ExpectedSubs = new[] { "ENG"
                ,"ESP"
                ,"SPA"
                ,"ENGLISH"
                ,"ESPAÑOL"
                ,"LATINOAMERICANO"
                ,"MEXICO"
                ,"SPANISH"
                 };
        private string[] OtherSubs = new[] {
                 "ARA"
                ,"AMERICAN"
                ,"ARABIC"
                ,"BOKMAL"
                ,"BRASIL"
                ,"BRAZILIAN"
                ,"BULGARIAN"
                ,"CANADA"
                ,"CANADIAN"
                ,"CANADIEN"
                ,"CANTONESE"
                ,"CASTILIAN"
                ,"CC"
                ,"CHI"
                ,"CHINESE"
                ,"CHINESE"
                ,"CHS"
                ,"CHT"
                ,"CROATIAN"
                ,"CZE"
                ,"CZECH"
                ,"Čeština"
                ,"DAN"
                ,"DANISH"
                ,"DEUTSCH"
                ,"DUT"
                ,"DUTCH"
                ,"ESTONIAN"
                ,"FIL"
                ,"FIN"
                ,"FINNISH"
                ,"FORCED"
                ,"FRA"
                ,"FrançaiS"
                ,"FRE"
                ,"FRENCH"
                ,"FULL"
                ,"GER"
                ,"GERMAN"
                ,"GRE"
                ,"GREEK"
                ,"HEB"
                ,"HEBREW"
                ,"HI"
                ,"HIN"
                ,"HRV"
                ,"HUN"
                ,"HUNGARIAN"
                ,"ICE"
                ,"ICELANDIC"
                ,"IND"
                ,"INDONESIAN"
                ,"ITA"
                ,"ITALIAN"
                ,"JAPANESE"
                ,"JPN"
                ,"KOR"
                ,"KOREAN"
                ,"LITHUANIAN"
                ,"MAGYER"
                ,"MANDARIN"
                ,"MALAY"
                ,"MAY"
                ,"NEDERLANDS"
                ,"NOB"
                ,"NOR"
                ,"NORWEGIAN"
                ,"POL"
                ,"POLISH"
                ,"POR"
                ,"Português"
                ,"PORTUGUESE"
                ,"ROMANIAN"
                ,"RUM"
                ,"RUS"
                ,"RUSSIAN"
                ,"SDH"
                ,"SERBIAN"
                ,"SIMPLIFIED"
                ,"SLO"
                ,"SLOVAK"
                ,"SLOVENIAN"
                ,"SUOMI"
                ,"SWE"
                ,"SWEDISH"
                ,"TAM"
                ,"TEL"
                ,"THA"
                ,"THAI"
                ,"TRADITIONAL"
                ,"TUR"
                ,"TURKISH"
                ,"Türkçe"
                ,"UKR"
                ,"UKRANIAN"
                ,"VIE"
                ,"VIETNAMESE"
            };

        private bool IsLanguageExtension(string n)//, string extension, string secondLast)
        {
            if (string.IsNullOrEmpty(n))
                return false;
            var r = RemoveSimbols(n);
            return ExpectedSubs.Any(s => r.ToUpper().EndsWith(s.ToUpper())
                    || r.ToUpper().StartsWith(s.ToUpper()))
                || OtherSubs.Any(s => r.ToUpper().EndsWith(s.ToUpper())
                    || r.ToUpper().StartsWith(s.ToUpper()));

            //if (extension != "srt")
            //    return false;
            //if (n != secondLast)
            //    return false;
        }

        private string ExistsKey(string name)
        {
            var t = new CultureInfo("en-US", false).TextInfo;
            var names = name.Split("[");
            var nn = new List<string>();
            var isfirst = true;
            foreach (var n in names)
                if (isfirst)
                {
                    nn.Add(t.ToTitleCase(n.ToLower()));
                    isfirst = false;
                }
                else
                    nn.Add(n.ToUpper());

            return string.Join('[', nn);
        }

        private string GetNewDirectoryNoFormat(string oldPath)
        {
            for (var season = 1; season < 40; season++)
            {
                var spos = oldPath.IndexOf($"S{season.ToString("00")}");
                if (spos < 0)
                    continue;
                var pos = oldPath.IndexOf($".", spos);
                if (pos == -1)
                    return oldPath.Remove(spos + 3, oldPath.Length - spos - 3);
                return oldPath.Remove(spos + 3, pos - spos - 3);
            }
            return oldPath;
        }
        private string GetNewDirectory(string oldPath, string format)
        {
            var pos = oldPath.ToUpper().IndexOf(format.ToUpper());
            if (pos < 0)
                return null;
            if (format.StartsWith("-"))
                pos++;
            for (var season = 1; season < 40; season++)
            {
                var spos = oldPath.IndexOf($"S{season.ToString("00")}");
                if (spos < 0)
                    continue;
                return oldPath.Remove(spos + 3, pos - spos - 4);
            }
            return oldPath;
        }
        private string RemoveSimbols(string s)
            => s.Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "");
    }
}
