using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DATUDAS_IDX_TO_IDXJ
{
    public static class Convert
    {
        public static void FromIdxToIdxJ(FileInfo info)
        {
            StreamReader idx;

            try
            {
                idx = info.OpenText();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return;
            }

            // continua só se idx != null

            bool isUdas = false;
            string FileFormat = "DAT";
            int SoundFlag = -1;
            uint FileCount = 0;

            Dictionary<string, string> DatFiles = new Dictionary<string, string>();

            while (!idx.EndOfStream)
            {
                string line = idx.ReadLine()?.Trim();

                if (!(string.IsNullOrEmpty(line)
                   || line.StartsWith("#")
                   || line.StartsWith("\\")
                   || line.StartsWith("/")
                   || line.StartsWith(":")
                   || line.StartsWith("!")
                ))
                {
                    var split = line.Split(new char[] { '=' });
                    if (split.Length >= 2)
                    {
                        string key = split[0].ToUpperInvariant().Trim();
                        string value = split[1].Trim().Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

                        if (key.Contains("SOUNDFLAG"))
                        {
                            int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out SoundFlag);
                            isUdas = true;
                            FileFormat = "UDAS";
                            if (SoundFlag == 0)
                            {
                                SoundFlag = -1;
                            }
                        }
                        else if (key.Contains("FILECOUNT"))
                        {
                            uint.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out FileCount);
                        }
                        else if (key.StartsWith("FILE_"))
                        {
                            if (!DatFiles.ContainsKey(key))
                            {
                                DatFiles.Add(key, value);
                            }
                        }
                    }
                }

            }

            idx.Close();

            if (FileCount == 0)
            {
                Console.WriteLine("FileCount cannot be 0!");
                return;
            }

            StreamWriter idxj;

            try
            {
                FileInfo endFileInfo = new FileInfo(Path.ChangeExtension(info.FullName, "idxJ"));
                idxj = endFileInfo.CreateText();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return;
            }

            // continua só se idxj != null

            uint datAmount = FileCount;
            if (isUdas && SoundFlag > 0 && datAmount > 0)
            {
                datAmount -= 1;
            }

            Line[] datGroup = new Line[datAmount];

            // get files
            for (int i = 0; i < datAmount; i++)
            {
                Line dat = new Line();
                dat.FileID = i;
                dat.FileName = "";
                dat.Extension = "";

                string key = "FILE_" + i;
                if (DatFiles.ContainsKey(key))
                {
                    dat.FileName = DatFiles[key];
                    dat.Extension = Path.GetExtension(DatFiles[key]);
                }
                datGroup[i] = dat;
            }

            Line Snd = new Line();
            Snd.FileID = -1;
            Snd.FileName = "";
            Snd.Extension = "";

            if (isUdas && SoundFlag > 0 && FileCount > 0)
            {
                string key = "FILE_" + (FileCount - 1);
                if (DatFiles.ContainsKey(key))
                {
                    Snd.FileName = DatFiles[key];
                    Snd.Extension = Path.GetExtension(DatFiles[key]);
                }
            }

            // cria novo arquivo dados

            idxj.WriteLine("# github.com/JADERLINK/JADERLINK_DATUDAS_TOOL");
            idxj.WriteLine("# youtube.com/@JADERLINK");
            idxj.WriteLine("# JADERLINK DATUDAS TOOL By JADERLINK");
            idxj.WriteLine("TOOL_VERSION:V04");
            idxj.WriteLine("FILE_FORMAT:" + FileFormat);
            idxj.Write("DAT_AMOUNT:" + datAmount);
            Console.WriteLine("FILE_FORMAT:" + FileFormat);
            Console.WriteLine("DAT_AMOUNT:" + datAmount);

            for (int i = 0; i < datGroup.Length; i++)
            {
                string line = "DAT_" + datGroup[i].FileID.ToString("D3") + ":" + datGroup[i].FileName;
                idxj.Write(Environment.NewLine + line);
                Console.WriteLine(line);
            }
            if (isUdas && SoundFlag > 0)
            {
                idxj.Write(Environment.NewLine + "UDAS_SOUNDFLAG:" + SoundFlag);
                idxj.Write(Environment.NewLine + "UDAS_END:" + Snd.FileName);

                Console.WriteLine("UDAS_SOUNDFLAG:" + SoundFlag);
                Console.WriteLine("UDAS_END:" + Snd.FileName);
            }
            idxj.Close();

        }

        public static void FromIdxjToIdx(FileInfo info)
        {
            StreamReader idxj;

            try
            {
                idxj = info.OpenText();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return;
            }

            // continua só se idxj != null

            string FILE_FORMAT = null;
            uint DAT_AMOUNT = 0;
            Dictionary<string, string> DatFiles = new Dictionary<string, string>();
            int UDAS_SOUNDFLAG = -1;
            string UDAS_END = null;

            while (!idxj.EndOfStream)
            {
                string line = idxj.ReadLine()?.Trim();

                if (!(string.IsNullOrEmpty(line)
                   || line.StartsWith("#")
                   || line.StartsWith("\\")
                   || line.StartsWith("/")
                   || line.StartsWith(":")
                   || line.StartsWith("!")
                   || line.StartsWith("@")
                ))
                {
                    var split = line.Split(new char[] { ':' });
                    if (split.Length >= 2)
                    {
                        string key = split[0].ToUpperInvariant().Trim();
                        string value = split[1].Trim().Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

                        if (key.Contains("FILE_FORMAT"))
                        {
                            FILE_FORMAT = value.ToUpperInvariant();
                        }
                        else if (key.Contains("UDAS_END"))
                        {
                            UDAS_END = value;
                        }
                        else if (key.Contains("UDAS_SOUNDFLAG"))
                        {
                            int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out UDAS_SOUNDFLAG);
                        }
                        else if (key.Contains("DAT_AMOUNT"))
                        {
                            uint.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out DAT_AMOUNT);
                        }
                        else if (key.StartsWith("DAT_"))
                        {
                            if (!DatFiles.ContainsKey(key))
                            {
                                DatFiles.Add(key, value);
                            }
                        }
                    }
                }

            }

            idxj.Close();

            if (FILE_FORMAT == null || !(FILE_FORMAT == "UDAS" || FILE_FORMAT == "DAT"))
            {
                Console.WriteLine("Invalid FILE_FORMAT!");
                return;
            }

            if (DAT_AMOUNT == 0)
            {
                Console.WriteLine("DAT_AMOUNT cannot be 0!");
                return;
            }

            StreamWriter idx_;

            try
            {
                FileInfo endFileInfo = new FileInfo(Path.ChangeExtension(info.FullName, "idx"));
                idx_ = endFileInfo.CreateText();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return;
            }

            // continua só se idx_ != null

            bool isUdas = false;

            Line[] datGroup = new Line[DAT_AMOUNT];

            // get files
            for (int i = 0; i < DAT_AMOUNT; i++)
            {
                Line dat = new Line();
                dat.FileID = i;
                dat.FileName = "";
                dat.Extension = "";

                string key = "DAT_" + i.ToString("D3");
                if (DatFiles.ContainsKey(key))
                {
                    dat.FileName = DatFiles[key];
                    dat.Extension = Path.GetExtension(DatFiles[key]);
                }

                datGroup[i] = dat;
            }

            Line Snd = new Line();
            Snd.FileID = -1;
            Snd.FileName = "";
            Snd.Extension = "";

            if (FILE_FORMAT == "UDAS")
            {
                isUdas = true;

                if (UDAS_END != null)
                {
                    Snd.FileName = UDAS_END;
                    Snd.Extension = Path.GetExtension(UDAS_END);
                }
            }

            //file
            uint FileCount = DAT_AMOUNT;
            if (isUdas && UDAS_SOUNDFLAG > 0 && FileCount > 0)
            {
                FileCount += 1;
            }

            idx_.Write("FileCount = " + FileCount);
            Console.WriteLine("FileCount = " + FileCount);
            if (isUdas)
            {
                idx_.Write(Environment.NewLine + "SoundFlag = " + UDAS_SOUNDFLAG);
                Console.WriteLine("SoundFlag = " + UDAS_SOUNDFLAG);
            }

            for (int i = 0; i < datGroup.Length; i++)
            {
                string line = "File_" + datGroup[i].FileID + " = " + datGroup[i].FileName;
                idx_.Write(Environment.NewLine + line);
                Console.WriteLine(line);
            }

            if (isUdas && UDAS_SOUNDFLAG > 0 && FileCount > 0)
            {
                string line = "File_" + (FileCount - 1) + " = " + Snd.FileName;
                idx_.Write(Environment.NewLine + line);
                Console.WriteLine(line);
            }

            idx_.Close();

        }

        private class Line
        {
            public int FileID { get; set; } = -1;
            public string FileName { get; set; } = "";
            public string Extension { get; set; } = "";
        }

    }
    
}
