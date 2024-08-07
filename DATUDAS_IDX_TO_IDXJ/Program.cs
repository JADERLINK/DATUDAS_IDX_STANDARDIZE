﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DATUDAS_IDX_TO_IDXJ
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine("# DATUDAS_IDX_TO_IDXJ");
            Console.WriteLine("# By: JADERLINK");
            Console.WriteLine("# VERSION 1.0.2 (2024-08-03)");
            Console.WriteLine("# youtube.com/@JADERLINK");
            Console.WriteLine("");

            if (args.Length == 0)
            {
                Console.WriteLine("Pass an .idx or .idxj file as the first parameter.");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else if (args.Length > 0 && File.Exists(args[0]))
            {
                string file = args[0];
                FileInfo info = null;

                try
                {
                    info = new FileInfo(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in the path: " + Environment.NewLine + ex);
                }

                if (info != null)
                {
                    Console.WriteLine("File: " + info.Name);

                    if (info.Extension.ToUpperInvariant() == ".IDX")
                    {
                        try
                        {
                            _ = new Convert(info, Convert.IdxType.Idx);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex);
                        }
                    }
                    else if (info.Extension.ToUpperInvariant() == ".IDXJ")
                    {
                        try
                        {
                            _ = new Convert(info, Convert.IdxType.IdxJ);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex);
                        }
                    }
                    else
                    {
                        Console.WriteLine("The extension is not valid: " + info.Extension);
                    }

                }

            }
            else
            {
                Console.WriteLine("File specified does not exist.");
            }

            Console.WriteLine("Finished!!!");
            Console.WriteLine("");
        }
    }
}
