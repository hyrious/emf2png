using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

class EMF2PNG {
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

  static void Main(string[] args) {
    if (args.Length == 0 || args.Length > 3) {
      Console.WriteLine(Help);
    }
    else if (args.Length == 1) {
      GetSize(args[0]);
    }
    else if (args.Length == 2) {
      Resize(args[0], args[1]);
    }
    else if (args.Length == 3) {
      Resize(args[0], args[1], args[2]);
    }
  }

  static void GetSize(string filename) {
    var image = Image.FromFile(filename);
    Console.WriteLine("{0},{1}", image.Width, image.Height);
  }

  static void Resize(string filename, string config) {
    var outfile = Path.ChangeExtension(filename, ".png");
    if (filename == outfile) {
      outfile = filename += ".png";
    }
    Resize(filename, config, outfile);
  }

  static void Resize(string filename, string config, string outfile) {
    var image = Image.FromFile(filename);
    int width, height;

    if (config.EndsWith("x")) {
      double scale;
      if (Double.TryParse(config.Substring(0, config.Length - 1), out scale) && 0 < scale && scale < 100) {
        width  = (int) Math.Round(image.Width  * scale);
        height = (int) Math.Round(image.Height * scale);
      } else {
        Console.WriteLine("Failed to parse scale, expecting things like '2x', got {0}", config);
        return;
      }
    } else {
      char[] sep = { ':' };
      string[] size = config.Split(sep, 2);
      if (Int32.TryParse(size[0], out width) && Int32.TryParse(size[1], out height) && (width > 0 || height > 0)) {
        if (width <= 0 || height <= 0) {
          double ratio = (double)image.Width / image.Height;
          if (width <= 0) {
            width = (int) Math.Round(height * ratio);
          }
          else if (height <= 0) {
            height = (int) Math.Round(width / ratio);
          }
        }
      } else {
        Console.WriteLine("Failed to parse config, expecting things like '100:50', got {0}", config);
        return;
      }
    }

    // scale to 4x then scale down: https://stackoverflow.com/questions/12098924/how-do-i-resize-shrink-an-emf-metafile-in-net/37585204#37585204
    using (var bitmap = new Bitmap(width * 4, height * 4)) {
      using (var g = Graphics.FromImage(bitmap)) {
        g.Clear(Color.Transparent);
        g.DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);
      }

      using (var stream = new MemoryStream()) {
        bitmap.Save(stream, ImageFormat.Png);

        stream.Position = 0;
        using (var png = Image.FromStream(stream)) {
          using (var target = new Bitmap(width, height)) {
            using (var g = Graphics.FromImage(target)) {
              g.Clear(Color.Transparent);
              g.DrawImage(png, 0, 0, target.Width, target.Height);
            }

            target.Save(outfile, ImageFormat.Png);
          }
        }
      }
    }
  }
}
