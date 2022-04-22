using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace MaschineImageMaker.Helpers;

/// <summary>
///     Helpers for working with bitmaps.
/// </summary>
/// <remarks>Some of the code came from https://stackoverflow.com/a/6484754/3797962</remarks>
public static class BitmapHelpers
{

    /// <summary>
    ///     A BitmapImage extension method that converts a <see cref="BitmapImage"/> to a <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="bitmapImage">The bitmapImage to act on.</param>
    /// <returns>
    ///     BitmapImage as a Bitmap.
    /// </returns>
    public static Bitmap ToBitmap(this BitmapImage bitmapImage)
    {
        var enc = new BmpBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
        using var stream = new MemoryStream();
        enc.Save(stream);
        return new Bitmap(stream);
    }


    /// <summary>
    ///     A Bitmap extension method that converts a <see cref="Bitmap"/> to a <see cref="BitmapImage"/>.
    /// </summary>
    /// <param name="bitmap">The bitmap to act on.</param>
    /// <returns>
    ///     Bitmap as a BitmapImage.
    /// </returns>
    public static BitmapImage ToBitmapImage(this Bitmap bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }


    /// <summary>
    ///     An Image extension method that resizes an <see cref="Image"/> and returns a <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="image">The image to act on.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>
    ///     A Bitmap.
    /// </returns>
    public static Bitmap Resize(this Image image, int width, int height)
    {
        var destImage = new Bitmap(width, height);
        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        using var graphics = Graphics.FromImage(destImage);
        graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        using var wrapMode = new ImageAttributes();
        wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
        graphics.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, image.Width, image.Height,
            GraphicsUnit.Pixel, wrapMode);
        return destImage;
    }
}