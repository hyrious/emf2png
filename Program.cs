using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
class EMF2PNG
{
    static readonly string Help = @"
Usage:
  > emf2png.exe file.emf                  Get image size in pixels
  450,40

  > emf2png.exe file.emf 2x               Resize and convert to PNG
  > emf2png.exe file.emf 900:80
  > emf2png.exe file.emf 900:-1 out.png   -1 means keeping aspect ratio

Note:
  The default output filename is the input with extension changed to '.png',
  if it already ends with '.png', then an extra '.png' is appended.
";

    static void Main(string[] args)
    {
        if (args.Length == 1)
        {
            GetSize(args[0]);
        }
        else if (args.Length == 2)
        {
            Resize(args[0], args[1]);
        }
        else if (args.Length == 3)
        {
            Resize(args[0], args[1], args[2]);
        }
        else
        {
            Console.WriteLine(Help);
        }
    }

    private static Size GetSize(Metafile source, bool isWmf)
    {
        if (isWmf)
        {
            var header = source.GetMetafileHeader();
            var width = (int)(source.Width / header.DpiX * 100);
            var height = (int)(source.Height / header.DpiY * 100);
            return new Size(width, height);
        }
        else
        {
            return new Size(source.Width, source.Height);
        }
    }

    static void GetSize(string filename)
    {
        var source = new Metafile(filename);
        var size = GetSize(source, filename.EndsWith("wmf"));
        Console.WriteLine("{0},{1}", size.Width, size.Height);
    }

    static void Resize(string filename, string config)
    {
        var outfile = Path.ChangeExtension(filename, ".png");
        if (filename == outfile)
        {
            outfile = filename += ".png";
        }
        Resize(filename, config, outfile);
    }

    static void Resize(string filename, string config, string outfile)
    {
        var image = new Metafile(filename);
        int width, height;

        if (config.EndsWith("x"))
        {
            if (double.TryParse(config[..^1], out double scale) && 0 < scale && scale < 100)
            {
                var size = GetSize(image, filename.EndsWith("wmf"));
                width = (int)Math.Round(size.Width * scale);
                height = (int)Math.Round(size.Height * scale);
            }
            else
            {
                Console.WriteLine("Failed to parse scale, expecting things like '2x', got {0}", config);
                return;
            }
        }
        else
        {
            char[] sep = { ':' };
            string[] size = config.Split(sep, 2);
            if (int.TryParse(size[0], out width) && int.TryParse(size[1], out height) && (width > 0 || height > 0))
            {
                if (width <= 0 || height <= 0)
                {
                    double ratio = (double)image.Width / image.Height;
                    if (width <= 0)
                    {
                        width = (int)Math.Round(height * ratio);
                    }
                    else if (height <= 0)
                    {
                        height = (int)Math.Round(width / ratio);
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to parse config, expecting things like '100:50', got {0}", config);
                return;
            }
        }

        // scale to 4x then scale down: https://stackoverflow.com/questions/12098924/how-do-i-resize-shrink-an-emf-metafile-in-net/37585204#37585204
        using var bitmap = new Bitmap(width * 4, height * 4);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.Transparent);
            g.DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);
        }

        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);

        stream.Position = 0;
        using var png = Image.FromStream(stream);
        using var target = new Bitmap(width, height);
        using (var g = Graphics.FromImage(target))
        {
            g.Clear(Color.Transparent);
            g.DrawImage(png, 0, 0, target.Width, target.Height);
        }

        target.Save(outfile, ImageFormat.Png);
    }
}