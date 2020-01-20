using System;
using System.IO;

namespace MITP
{
    class Logger
    {
        private StreamWriter writer;
        private string path;

        public Logger()
        {
            path = "Log " + DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss") + ".txt";
            Create();
        }

        private void Create()
        {
            try
            {
                if (!File.Exists(path))
                {
                    writer = File.CreateText(path);
                    writer.Close();
                }
            }
            catch
            {
                Console.WriteLine("Can't create file");
            }
        }

        public void WriteMessage(string message)
        {
            string date = DateTime.Now.ToString("HH:mm:ss");

            try
            {
                writer = File.AppendText(path);
                writer.WriteLine("{0} {1}", date, message);
                writer.Close();
            }
            catch
            {
                Console.WriteLine("Not write");
            }
        }
    }
}


