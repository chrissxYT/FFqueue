using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using static System.Environment;
using static System.GC;
using static System.Console;
using static System.IO.Path;
using static System.Diagnostics.Process;
using static System.Threading.Thread;
using static System.Diagnostics.Stopwatch;

namespace FFqueue
{
    class Program
    {
        volatile static bool run = true;
        volatile static string ffmpeg, i, o, tmp;
        volatile static Random rand = new Random((int)(GetTimestamp() % int.MaxValue));
        volatile static string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
        volatile static bool encoding = false;

        static void Main(string[] args)
        {
            Write("ffmpeg: ");
            ffmpeg = ReadLine().Replace("\"", "");
            Write("in: ");
            i = ReadLine().Replace("\"", "");
            Write("out: ");
            o = ReadLine().Replace("\"", "");
            Write("src: ");
            tmp = ReadLine().Replace("\"", "");
            new Thread(() =>
            {
                while (run)
                {
                    string[] f = Directory.GetFiles(i);
                    if (f.Length > 0)
                    {
                        encoding = true;
                        string s = f[rand.Next(f.Length)];
                        string p = Combine(tmp, GetFileName(s));
                        while (File.Exists(p))
                            p += "_";
                        File.Move(s, p);
                        string fn = ChangeExtension(GetFileName(p), "mp4");
                        string d = Combine(o, fn);
                        s1 = s;
                        s2 = p;
                        s3 = fn;
                        s4 = d;
                        s5 = "";
                        foreach (string t in f)
                            s5 += " " + GetFileName(t);
                        s5 = s5.Substring(1);
                        Collect();
                        Start(new ProcessStartInfo(ffmpeg, $"-i \"{p}\" -c:v libx265 -c:a aac -b:a 92k \"{d}\"")).WaitForExit();
                        s1 = s2 = s3 = s4 = s5 = "";
                        encoding = false;
                    }
                    else
                        Sleep(rand.Next(60 * 1000));
                    Collect();
                }
            }).Start();

            while (run)
            {
                string cmd = ReadLine();
                if (cmd == "quit")
                {
                    run = false;
                    if (!encoding)
                        Exit(0);
                    else
                        WriteLine("will shut down once ffmpeg is done.");
                }
                else if(cmd == "debug")
                {
                    WriteLine("ffmpeg: " + ffmpeg);
                    WriteLine("i: " + i);
                    WriteLine("o: " + o);
                    WriteLine("tmp: " + tmp);
                    WriteLine("s1: " + s1);
                    WriteLine("s2: " + s2);
                    WriteLine("s3: " + s3);
                    WriteLine("s4: " + s4);
                    WriteLine("s5: " + s5);
                }
                else
                {
                    WriteLine("cmds: quit, debug");
                }
            }
        }
    }
}
