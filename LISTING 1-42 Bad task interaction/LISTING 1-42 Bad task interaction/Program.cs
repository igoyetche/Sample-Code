using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LISTING_1_42_Bad_task_interaction
{

    class Program
    {
        static long sharedTotal;

        static object thisIstheKey = new object();

        // make an array that holds the values 0 to 5000000
        static int[] items = Enumerable.Range(0, 500001).ToArray();

        static void addRangeOfValues(int start, int end, int threadNumber, bool verbose = false)
        {
            while (start < end)
            {
                if (verbose)
                    Console.Write(" {0} ", threadNumber);
                sharedTotal = sharedTotal + items[start];

                start++;
            }
        }

        static void addRangeOfValues_interlocked(int start, int end, int threadNumber, bool verbose = false)
        {
            while (start < end)
            {
                if (verbose)
                    Console.Write(" {0} ", threadNumber);

                Interlocked.Add(ref sharedTotal, items[start]);

                start++;
            }
        }



        private static void RunCalcs()
        {
            DateTime startTime = DateTime.Now;
            List<Task> tasks = new List<Task>();

            int rangeSize = 1000;
            int rangeStart = 0;
            int threadNumber = 1;
            sharedTotal = 0;

            while (rangeStart < items.Length)
            {
                int rangeEnd = rangeStart + rangeSize;

                if (rangeEnd > items.Length)
                    rangeEnd = items.Length;

                // create local copies of the parameters
                int rs = rangeStart;
                int re = rangeEnd;

                tasks.Add(Task.Run(() => addRangeOfValues_interlocked(rs, re, threadNumber++, false)));
                rangeStart = rangeEnd;
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine("The total is: {0}", sharedTotal);
            Console.WriteLine("It took {0} to run, and the pc used {1} tasks to run it",
                DateTime.Now - startTime, threadNumber - 1);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome");
            Console.WriteLine("Press Escape to exit");
            Console.WriteLine("Any other key to sum numbers from 0 to 5000000");
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                Program.RunCalcs();
            }
        }
    }
}
