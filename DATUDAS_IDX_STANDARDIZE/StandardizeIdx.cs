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
        public StandardizeIdx(FileInfo info)
        {
            string directory = info.DirectoryName;
            string baseName = info.Name.Substring(0, info.Name.Length - info.Extension.Length);

            StreamReader idx = null;

            try
            {
                idx = info.OpenText();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
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
                        l.SLine = endLine;
                        lines.Add(l);
                    }
                }
                idx.Close();
                //----

                //codigo responsavel por verificar quais linhas são arquivos
                foreach (var item in lines)
                {
                    string trim = item.SLine.Trim();

                    if (trim.ToLowerInvariant().StartsWith("file_"))
                    {
                        var split = trim.Split(new char[] { '=' });
                        if (split.Length >= 2)
                        {
                            int ikey = -1;
                            string key = split[0].ToLowerInvariant().Replace("file_", "").Trim();
                            if (int.TryParse(key, out ikey))
                            {
                                string vfile = split[1].Trim();
                                string Extension = "NULL";

                                var vfileSplit = vfile.Split('.');
                                if (vfileSplit.Length > 1)
                                {
                                    Extension = vfileSplit.LastOrDefault().ToUpperInvariant();
                                }

                                if (Extension == "DAS")
                                {
                                    Extension = "SND";
                                }

                                string newName = baseName + "\\" + baseName + "_" + ikey.ToString("D3") + "." + Extension;
                                if (Extension == "SND")
                                {
                                    newName = baseName + "\\" + baseName + "_END." + Extension;
                                }

                                item.FileID = ikey;
                                item.OldFileName = vfile;
                                item.NewFileName = newName;
                                item.IsFile = true;
                            }
                            
                        }

                    }

                }
                //----

                Console.WriteLine("Renaming and moving files.");

                //codigo por renomear os arquivos, no sistema de arquivo.
                string newDirectoy = directory + "\\" + baseName;
                try
                {
                    Directory.CreateDirectory(newDirectoy);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error when creating new directory:");
                    Console.WriteLine(newDirectoy);
                    Console.WriteLine("Message: " + ex.Message);
                }
               
                foreach (var item in lines)
                {
                    if (item.IsFile)
                    {
                        string oldf = directory + "\\" + item.OldFileName;
                        string newf = directory + "\\" + item.NewFileName;

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

                Console.WriteLine("Creating new .idx file:");
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
                            string newLine = "File_" + item.FileID.ToString() + " = " + item.NewFileName;
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
                Console.ForegroundColor = ConsoleColor.White;
            }

        }

        class Line 
        {
            public string SLine { get; set; } = "";
            public bool IsFile { get; set; } = false;
            public int FileID { get; set; } = -1;
            public string OldFileName { get; set; } = "";
            public string NewFileName { get; set; } = "";
            public bool FileHasBeenRenamed { get; set; } = false;
        }
    }
}
