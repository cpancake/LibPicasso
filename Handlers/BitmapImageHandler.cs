using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibPicasso.Handlers
{
    public class BitmapImageHandler : IImageHandler
    {
        /// <summary>
        /// Tests whether the image handler handles this format hint.
        /// </summary>
        /// <param name="ext">The format hint.</param>
        /// <returns>Whether or not this image handler handles this format hint.</returns>
        public bool HasExtension(string ext)
        {
            return ext == "bmp" || ext == "rle" || ext == "dib";
        }

        /// <summary>
        /// Tests whether a Stream contains a bitmap image.
        /// </summary>
        /// <param name="file">The Stream containing the file to test.</param>
        /// <returns>Whether the Stream contains a bitmap image or not.</returns>
        public bool Test(Stream file)
        {
            return (file.ReadByte() == 0x42 && file.ReadByte() == 0x4d);
        }

        /// <summary>
        /// Loads the frame or frames from a Stream.
        /// </summary>
        /// <param name="file">The Stream containing the bitmap image to load.</param>
        /// <returns>An array containing the frame or frames of this image.</returns>
        public Image[] Load(Stream file)
        {
            long pos = file.Position;
            if (!(file.ReadByte() == 0x42 && file.ReadByte() == 0x4d))
                throw new Exception("Provided file is not a bitmap image!");
            file.Position = pos;
            Image frame = Image.FromStream(file); //can load bmp
            return new Image[1] { frame };
        }
    }
}
