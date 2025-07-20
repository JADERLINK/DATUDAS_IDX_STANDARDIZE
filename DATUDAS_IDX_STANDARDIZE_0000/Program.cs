using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DATUDAS_IDX_STANDARDIZE_0000
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine("# DATUDAS_IDX_STANDARDIZE_0000");
            Console.WriteLine("# By: JADERLINK");
            Console.WriteLine("# youtube.com/@JADERLINK");
            Console.WriteLine("# github.com/JADERLINK");
            Console.WriteLine("# VERSION 1.0.3 (2025-07-20)");
            Console.WriteLine("");

            if (args.Length == 0)
            {
                Console.WriteLine("How to use: drag the file to the executable.");
                Console.WriteLine("Or pass an .idx or .idxj file as the first parameter.");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (File.Exists(args[i]))
                    {
                        try
                        {
                            Continue(args[i]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + args[i]);
                            Console.WriteLine(ex);
                        }
                    }
                    else
                    {
                        Console.WriteLine("File specified does not exist: " + args[i]);
                    }

                }

                Console.WriteLine("Finished!!!");
            }
        }

        private static void Continue(string file)
        {
            FileInfo info = new FileInfo(file);
            Console.WriteLine("File: " + info.Name);

            if (info.Extension.ToUpperInvariant() == ".IDX")
            {
                _ = new StandardizeIdx_0000(info, StandardizeIdx_0000.IdxType.Idx);
            }
            else if (info.Extension.ToUpperInvariant() == ".IDXJ")
            {
                _ = new StandardizeIdx_0000(info, StandardizeIdx_0000.IdxType.IdxJ);
            }
            else if (info.Extension.ToUpperInvariant() == ".IDXBIG")
            {
                _ = new StandardizeIdx_0000(info, StandardizeIdx_0000.IdxType.IdxJ);
            }
            else
            {
                Console.WriteLine("The extension is not valid: " + info.Extension);
            }
        }
    }
}
