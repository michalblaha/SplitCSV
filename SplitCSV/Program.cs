using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Help(); return;
            }

            var ConsoleFColor = Console.ForegroundColor;

            string fn = System.IO.Path.GetFullPath(args[0]);
            string fnExt = System.IO.Path.GetExtension(fn);
            string fnNoExt = fn.Substring(0,fn.Length-fnExt.Length);

            if (!System.IO.File.Exists(fn))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File {fn} not found.");
                Console.ForegroundColor = ConsoleFColor;
                return;
            }
            long chunkSize = 0;
            if (!long.TryParse(args[1], out chunkSize))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chunk size {args[1]} is not recongnized as number.");
                Console.ForegroundColor = ConsoleFColor;
                return;
            }

            long currSize = 0;
            long actChunk = 1;

            using (var reader = new StreamReader(fn))
            {
                using (var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration() { HasHeaderRecord = false, Delimiter = "," }))
                {
                    while (csv.Read())
                    {

                        var rec = csv.Context.Record;
                        var recRaw = rec.Select(m =>
                                    {
                                        if (m.Contains((char)13) || m.Contains((char)10) || m.Contains("\t"))
                                            return "\"" + m + "\"";
                                        else
                                            return m;
                                    }
                        ).Aggregate((f, s) => f + "," + s)
                            + "\n";
                        var recSize = Encoding.UTF8.GetByteCount(recRaw);
                        if (currSize+recSize > chunkSize)
                        {
                            actChunk++;
                            currSize = 0;
                        }

                        if (currSize == 0)
                        {
                            System.IO.File.Delete(chunkFn(fnNoExt,fnExt,actChunk));
                            Console.WriteLine("Writing into " + chunkFn(fnNoExt, fnExt, actChunk));
                        }

                        currSize = currSize + recSize;

                        System.IO.File.AppendAllText(chunkFn(fnNoExt, fnExt, actChunk), recRaw);
                    }
                }
            }
        }

        static string chunkFn(string fnNoExt, string fnExt, long part)
        {
            return fnNoExt + "_" + part.ToString("0000") + fnExt;
        }

        static void Help()
        {
            Console.WriteLine(@"SplitCSV - split CSV to chunks
SplitCSV filename maxChunkSizeInBytes


");
        }
    }
}
