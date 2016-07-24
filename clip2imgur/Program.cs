using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ClipboardMonitor;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;

namespace clip2imgur
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            ClipboardNotifications.ClipboardUpdate += ClipboardChanged;
            while (true) Thread.Sleep(1000);
        }

        private static async void ClipboardChanged(object sender, EventArgs eventArgs)
        {
            if (Clipboard.ContainsImage())
            {
                Console.WriteLine("Clipboard image event occurred, capturing and rewriting...");

                try
                {
                    var client = new ImgurClient(Secrets.ClientId, Secrets.ClientSecret);
                    var ep = new ImageEndpoint(client);
                    using (var mem = new MemoryStream())
                    {
                        using (var i = Clipboard.GetImage())
                        {
                            i.Save(mem, ImageFormat.Png);
                        }
                        // var res = ep.UploadImageStreamAsync(mem);
                        // var img = res.Result;
                        var img = await ep.UploadImageBinaryAsync(mem.GetBuffer());
                        Console.WriteLine(img.Link);
                        Clipboard.SetText(img.Link);
                    }
                }
                catch (ImgurException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}