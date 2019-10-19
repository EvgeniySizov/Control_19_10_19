using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Control_19_10_19
{
    public class FileWatcher
    {
        DateTime changeTime;
        private readonly string _filePath;

        public Action<DateTime, string> Change;

        public FileWatcher(string path)
        {
            _filePath = path;
        }

        public void Start()
        {
            Thread th = new Thread(() =>
            {
                changeTime = File.GetLastWriteTime(_filePath);
                while (true)
                {
                    if (changeTime != File.GetLastWriteTime(_filePath))
                    {
                        changeTime = File.GetLastWriteTime(_filePath);
                        Change?.Invoke(changeTime, _filePath);
                    }
                }
            });
            th.Start();
        }
    }

    class Program
    {
        private static readonly object obj = new object();

        static void Main(string[] args)
        {
            try
            {
                var path = "TestDocs.txt";
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }

                var w = new FileWatcher(path);
                w.Change += W_Change;
                w.Start();

                while (true)
                {
                    Console.WriteLine("Do you whant change to 1?(y/n)");
                    string ans = Console.ReadLine().ToLower();

                    if (ans == "y")
                    {
                        File.WriteAllText(path, "1");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception a)
            {
                Console.WriteLine(a.Message);
            }
        }
        private static void W_Change(DateTime changeTime, string path)
        {

            Thread thread = new Thread(() =>
            {
                lock (obj)
                {
                    Console.WriteLine("Changed " + changeTime);
                    string contains = File.ReadAllText(path);
                    if (contains == "1")
                    {
                        File.WriteAllText(path, "0");
                        Thread.Sleep(100000);
                        Console.WriteLine("File changet to '0' ten second ago");
                    }
                }
            });
            thread.Start();
        }
    }
}
