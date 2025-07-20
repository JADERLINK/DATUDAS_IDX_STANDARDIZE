using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DATUDAS_IDX_STANDARDIZE
{
    public class StandardizeIdx
    {
        public enum IdxType
        {
            Idx,
            IdxJ
        }

        public StandardizeIdx(FileInfo info, IdxType type)
        {
            string entry_file = "file_";
            char entry_separator = '=';
            string output_file = "File_";
            string output_separator = " = ";
            string output_FileID_ToString = "D";

            if (type == IdxType.IdxJ)
            {
                entry_file = "dat_";
                entry_separator = ':';
                output_file = "DAT_";
                output_separator = ":";
                output_FileID_ToString = "D3";
            }

            string directory = info.DirectoryName;
            string baseName = Path.GetFileNameWithoutExtension(info.Name);
            string idxFormat = Path.GetExtension(info.Name);

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

            if (idx != null)
            {
                List<Line> lines = new List<Line>(); // lista de linhas no arquivo .idx

                // codigo responsavel por ler as linhas
                string endLine = "";
                while (endLine != null)
                {
                    endLine = idx.ReadLine();
                    if (endLine != null)
                    {
                        Line l = new Line();
                        l.SLine = endLine.Trim();
                        lines.Add(l);
                    }
                }
                idx.Close();
                //----

                //Código responsável por verificar quais linhas são arquivos.
                foreach (var item in lines)
                {
                    string trim = item.SLine.Trim();

                    if (trim.ToLowerInvariant().StartsWith(entry_file))
                    {
                        var split = trim.Split(new char[] { entry_separator });
                        if (split.Length >= 2)
                        {
                            int ikey = -1;
                            string key = split[0].ToLowerInvariant().Replace(entry_file, "").Trim();
                            if (int.TryParse(key, out ikey))
                            {
                                string vfile = split[1].Trim().Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                                string Extension = "";

                                var vfileSplit = vfile.Split('.');
                                if (vfileSplit.Length > 1)
                                {
                                    Extension = vfileSplit.LastOrDefault()?.ToUpperInvariant() ?? "";
                                }

                                if (Extension == "DAS")
                                {
                                    Extension = "SND";
                                }

                                string newName = baseName + Path.DirectorySeparatorChar + baseName + "_" + ikey.ToString("D3");
                                if (Extension.Length > 0)
                                {
                                    newName += "." + Extension;
                                }
                                
                                if (Extension == "SND")
                                {
                                    newName = baseName + Path.DirectorySeparatorChar + baseName + "_END." + Extension;
                                }

                                item.FileID = ikey;
                                item.OldFileName = vfile;
                                item.NewFileName = newName;
                                item.Extension = Extension;
                                item.IsFile = true;
                            }
                            
                        }

                    }
                    else if (type == IdxType.IdxJ && trim.ToLowerInvariant().StartsWith("udas_end"))
                    {
                        var split = trim.Split(new char[] { ':' });
                        if (split.Length >= 2)
                        {
                            string vfile = split[1].Trim().Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                            string Extension = "EMPTY";

                            var vfileSplit = vfile.Split('.');
                            if (vfileSplit.Length > 1)
                            {
                                Extension = vfileSplit.LastOrDefault()?.ToUpperInvariant() ?? "EMPTY";
                            }

                            if (Extension == "DAS")
                            {
                                Extension = "SND";
                            }

                            string newName = baseName + Path.DirectorySeparatorChar + baseName + "_END." + Extension;

                            item.FileID = -1;
                            item.IsSND = true;
                            item.OldFileName = vfile;
                            item.NewFileName = newName;
                            item.Extension = Extension;
                            item.IsFile = true;

                        }
                    }

                }
                //----

                Console.WriteLine("Renaming and moving files.");

                //codigo por renomear os arquivos, no sistema de arquivo.
                string newDirectoy = directory + Path.DirectorySeparatorChar + baseName;
                try
                {
                    Directory.CreateDirectory(newDirectoy);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error when creating new directory:");
                    Console.WriteLine(newDirectoy);
                    Console.WriteLine("Message: " + ex.Message);
                    return;
                }
               
                foreach (var item in lines)
                {
                    if (item.IsFile)
                    {
                        string oldf = directory + Path.DirectorySeparatorChar + item.OldFileName;
                        string newf = directory + Path.DirectorySeparatorChar + item.NewFileName;

                        try
                        {
                            File.Move(oldf, newf);
                            item.FileHasBeenRenamed = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error when renaming file:");
                            Console.WriteLine(item.OldFileName);
                            Console.WriteLine("Message: " + ex.Message);
                        }
                    }
                }

                Console.WriteLine($"Creating new {idxFormat} file:");
                Console.WriteLine();

                StreamWriter idxW = null;
                // codigo responsavel por criar o novo .idx
                try
                {
                    idxW = info.CreateText();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }

                if (idxW != null)
                {
                    foreach (var item in lines)
                    {
                        if (item.IsFile && item.FileHasBeenRenamed)
                        {
                            string newLine;

                            if (type == IdxType.IdxJ && item.IsSND)
                            {
                                newLine = "UDAS_END:" + item.NewFileName;
                            }
                            else 
                            {
                                newLine = output_file + item.FileID.ToString(output_FileID_ToString) + output_separator + item.NewFileName;
                            }
                            idxW.WriteLine(newLine);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(item.SLine);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" --> ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(newLine);
                            Console.WriteLine();
                        }
                        else if (item.SLine.Length > 0)
                        {
                            idxW.WriteLine(item.SLine);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(item.SLine);
                        }
                    }
                    idxW.Close();
                }
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        }

        private class Line 
        {
            public string SLine { get; set; } = "";
            public bool IsFile { get; set; } = false;
            public bool IsSND { get; set; } = false;
            public int FileID { get; set; } = -1;
            public string OldFileName { get; set; } = "";
            public string NewFileName { get; set; } = "";
            public string Extension { get; set; } = "";

            public bool FileHasBeenRenamed { get; set; } = false;
        }
    }
}
