using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DATUDAS_IDX_TO_IDXJ
{
    public class Convert
    {
        public enum IdxType
        {
            Idx,
            IdxJ
        }

      
        public Convert(FileInfo info, IdxType type)
        {
            if (type == IdxType.IdxJ)
            {
                FromIdxjToIdx(info);
            }
            else if (type == IdxType.Idx) 
            {
                FromIdxToIdxJ(info);
            }
        }

        private void FromIdxToIdxJ(FileInfo info)
        {
            StreamReader idx = null;

            try
            {
                idx = info.OpenText();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return;
            }

            if (idx != null)
            {

                Dictionary<string, string> pair = new Dictionary<string, string>();

                string endLine = "";
                while (endLine != null)
                {
                    endLine = idx.ReadLine();

                    if (endLine != null)
                    {
                        endLine = endLine.Trim();

                        if (!(endLine.Length == 0
                            || endLine.StartsWith("#")
                            || endLine.StartsWith("\\")
                            || endLine.StartsWith("/")
                            || endLine.StartsWith(":")
                            || endLine.StartsWith("!")
                            ))
                        {
                            var split = endLine.Split(new char[] { '=' });
                            if (split.Length >= 2)
                            {
                                string key = split[0].ToUpperInvariant().Trim();
                                if (!pair.ContainsKey(key))
                                {
                                    pair.Add(key, split[1].Trim());
                                }
                            }
                        }

                    }

                }

                idx.Close();

                //SoundFlag

                bool isUdas = false;
                string FileFormat = "DAT";
                int SoundFlag = -1;
                int FileCount = 0;

                if (pair.ContainsKey("SOUNDFLAG"))
                {
                    isUdas = true;
                    FileFormat = "UDAS";
                    try
                    {
                        SoundFlag = int.Parse(pair["SOUNDFLAG"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                        if (SoundFlag > 0xFF)
                        {
                            SoundFlag = 0xFF;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SoundFlag convert error: " + ex);
                        return;
                    }
                }

                //FileCount
                if (pair.ContainsKey("FILECOUNT"))
                {
                    try
                    {
                        FileCount = int.Parse(pair["FILECOUNT"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("FileCount convert error: " + ex);
                        return;
                    }

                }
                else
                {
                    Console.WriteLine("FileCount does not exist.");
                    return;
                }


                StreamWriter idxj = null;

                try
                {
                    string directory = info.DirectoryName;
                    string baseName = Path.GetFileNameWithoutExtension(info.Name);
                    string EndFileName = directory + "\\" + baseName + ".idxJ";
                    FileInfo EndFileInfo = new FileInfo(EndFileName);
                    idxj = EndFileInfo.CreateText();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    return;
                }

                if (idxj != null)
                {

                    int datAmount = FileCount;
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
                        if (pair.ContainsKey(key))
                        {
                            dat.FileName = pair[key];
                            dat.Extension = Path.GetExtension(pair[key]);
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
                        if (pair.ContainsKey(key))
                        {
                            Snd.FileName = pair[key];
                            Snd.Extension = Path.GetExtension(pair[key]);
                        }
                    }

                    // cria novo arquivo dados

                    idxj.WriteLine("# github.com/JADERLINK/JADERLINK_DATUDAS_TOOL");
                    idxj.WriteLine("# youtube.com/@JADERLINK");
                    idxj.WriteLine("# JADERLINK DATUDAS TOOL By JADERLINK");
                    idxj.WriteLine("TOOL_VERSION:V02");
                    idxj.WriteLine("FILE_FORMAT:" + FileFormat);
                    idxj.Write("DAT_AMOUNT:" + datAmount);
                    for (int i = 0; i < datGroup.Length; i++)
                    {
                        string Line = "DAT_" + datGroup[i].FileID.ToString("D3") + ":" + datGroup[i].FileName;
                        idxj.Write(Environment.NewLine + Line);
                    }
                    if (isUdas && SoundFlag > 0)
                    {
                        idxj.Write(Environment.NewLine + "UDAS_SOUNDFLAG:" + SoundFlag);
                        idxj.Write(Environment.NewLine + "UDAS_END:" + Snd.FileName);
                    }
                    idxj.Close();
                }


            }

        }

        private void FromIdxjToIdx(FileInfo info) 
        {
            StreamReader idxj = null;

            try
            {
                idxj = info.OpenText();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return;
            }

            if (idxj != null)
            {
                Dictionary<string, string> pair = new Dictionary<string, string>();

                string endLine = "";
                while (endLine != null)
                {
                    endLine = idxj.ReadLine();

                    if (endLine != null)
                    {
                        endLine = endLine.Trim();

                        if (!(endLine.Length == 0
                            || endLine.StartsWith("#")
                            || endLine.StartsWith("\\")
                            || endLine.StartsWith("/")
                            || endLine.StartsWith(":")
                            || endLine.StartsWith("!")
                            ))
                        {
                            var split = endLine.Split(new char[] { ':' });
                            if (split.Length >= 2)
                            {
                                string key = split[0].ToUpperInvariant().Trim();
                                if (!pair.ContainsKey(key))
                                {
                                    pair.Add(key, split[1].Trim());
                                }
                            }
                        }
                    }

                }

                idxj.Close();


                if (pair.ContainsKey("FILE_FORMAT") && pair.ContainsKey("DAT_AMOUNT"))
                {
                    string FileFormat = pair["FILE_FORMAT"].ToUpperInvariant().Trim();

                    if (FileFormat == "UDAS" || FileFormat == "DAT")
                    {
                        bool isUdas = false;
                        int SoundFlag = -1;
                        bool asSoundFlag = false;

                        int datAmount = 0;
                        try
                        {
                            datAmount = int.Parse(pair["DAT_AMOUNT"].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("DAT_AMOUNT convert error: " + ex);
                            return;
                        }

                        if (FileFormat == "UDAS" && pair.ContainsKey("UDAS_SOUNDFLAG"))
                        {
                            try
                            {
                                SoundFlag = int.Parse(pair["UDAS_SOUNDFLAG"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                                if (SoundFlag > 0xFF)
                                {
                                    SoundFlag = 0xFF;
                                }
                                asSoundFlag = true;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("UDAS_SOUNDFLAG convert error: " + ex);
                                return;
                            }
                        }

                        StreamWriter idx_ = null;

                        try
                        {
                            string directory = info.DirectoryName;
                            string baseName = Path.GetFileNameWithoutExtension(info.Name);
                            string EndFileName = directory + "\\" + baseName + ".idx";
                            FileInfo EndFileInfo = new FileInfo(EndFileName);
                            idx_ = EndFileInfo.CreateText();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex);
                            return;
                        }


                        if (idx_ != null)
                        {
                            Line[] datGroup = new Line[datAmount];

                            // get files
                            for (int i = 0; i < datAmount; i++)
                            {
                                Line dat = new Line();
                                dat.FileID = i;
                                dat.FileName = "";
                                dat.Extension = "";

                                string key = "DAT_" + i.ToString("D3");
                                if (pair.ContainsKey(key))
                                {
                                    dat.FileName = pair[key];
                                    dat.Extension = Path.GetExtension(pair[key]);
                                }

                                datGroup[i] = dat;
                            }

                            Line Snd = new Line();
                            Snd.FileID = -1;
                            Snd.FileName = "";
                            Snd.Extension = "";

                            if (FileFormat == "UDAS")
                            {
                                isUdas = true;

                                if (pair.ContainsKey("UDAS_END"))
                                {
                                    Snd.FileName = pair["UDAS_END"];
                                    Snd.Extension = Path.GetExtension(pair["UDAS_END"]);
                                }
                            }

                            //file
                            int FileCount = datAmount;
                            if (isUdas && asSoundFlag && SoundFlag > 0 && FileCount > 0)
                            {
                                FileCount += 1;
                            }

                            idx_.Write("FileCount = " + FileCount);
                            if (isUdas)
                            {
                                idx_.Write(Environment.NewLine + "SoundFlag = " + SoundFlag);
                            }
                           
                            for (int i = 0; i < datGroup.Length; i++)
                            {
                                idx_.Write(Environment.NewLine + "File_" + datGroup[i].FileID + " = " + datGroup[i].FileName);
                            }

                            if (isUdas && asSoundFlag && SoundFlag > 0 && FileCount > 0)
                            {
                                idx_.Write(Environment.NewLine + "File_" + (FileCount - 1) + " = " + Snd.FileName);
                            }

                            idx_.Close();
                        }

                    }
                    else
                    {
                        Console.WriteLine("Invalid FILE_FORMAT: " + FileFormat);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Not found FILE_FORMAT or DAT_AMOUNT tag.");
                    return;
                }

            }


        }

        private class Line
        {
            public int FileID { get; set; } = -1;
            public string FileName { get; set; } = "";
            public string Extension { get; set; } = "NULL";
        }

    }
    
}
