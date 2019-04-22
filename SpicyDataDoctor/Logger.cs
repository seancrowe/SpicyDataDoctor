using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpicyDataDoctor
{
    public class Logger
    {
        private string logDirectory;

        public Logger(string writeDirectory)
        {
            logDirectory = writeDirectory;

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        private bool headerSet;

        public void WriteLog(string message, string sender = "log")
        {
            FileStream fs = File.OpenWrite(logDirectory + "/"+sender+".txt");
            fs.Position = fs.Length;

            message = Environment.NewLine + message;

            if (headerSet == false)
            {
                headerSet = true;

                message = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " @ " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + Environment.NewLine + message;
                message = "New Log-------------- " + message;

                if (fs.Length > 0)
                {
                    message = Environment.NewLine + Environment.NewLine + "From " + sender + ": " + message;
                }
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            fs.Write(messageBytes, 0, messageBytes.Length);
            fs.Close();
        }

        public void WriteLog(Exception exception, string sender = "log")
        {
            string message = exception.Message;

            WriteLog(message, sender);
        }

        public void WriteFile(string message, string sender = "log")
        {
            if (File.Exists(logDirectory + "/" + sender + ".txt"))
            {
                File.Delete(logDirectory + "/" + sender + ".txt");
            }

            FileStream fs = File.Create(logDirectory + "/" + sender + ".txt");

            StreamWriter streamWriter = new StreamWriter(fs);

            streamWriter.WriteLine(message);

            streamWriter.Close();
        }

    }
}
