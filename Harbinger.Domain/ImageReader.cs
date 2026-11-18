using System;
using System.Collections.Generic;
using System.Text;

using ImageMagick;

namespace Harbinger.Domain;

public static class ImageReader
{
    private static IReadOnlyCollection<string> _fileNames = [SampleImages.Jpg, SampleImages.Png, SampleImages.WebP, SampleImages.Svg];


    public static void ReadImage(this string imageFile)
    {
        foreach(var fileName in _fileNames)
        {
            try
            {
                using var image = new MagickImage(fileName);
                var (width, height) = (image.Width, image.Height);
                var pixels = image.GetPixels();
                foreach(var pix in pixels)
                {
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }



    }


}
