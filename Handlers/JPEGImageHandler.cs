using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibPicasso.Handlers
{
    public class JPEGImageHandler : IImageHandler
    {
        /// <summary>
        /// Tests whether the image handler handles this format hint.
        /// </summary>
        /// <param name="ext">The format hint.</param>
        /// <returns>Whether or not this image handler handles this format hint.</returns>
        public bool HasExtension(string ext)
        {
            return ext == "jpeg" || ext == "jpg" || ext == "jpe";
        }

        /// <summary>
        /// Tests whether a Stream contains a JPEG image.
        /// </summary>
        /// <param name="file">The Stream containing the file to test.</param>
        /// <returns>Whether the Stream contains a JPEG image or not.</returns>
        public bool Test(Stream file)
        {
            return (file.ReadByte() == 0xff && file.ReadByte() == 0xd8);
        }

        /// <summary>
        /// Loads the frame or frames from a Stream.
        /// </summary>
        /// <param name="file">The Stream containing the JPEG image to load.</param>
        /// <returns>An array containing the frame or frames of this image.</returns>
        public Image[] Load(Stream file)
        {
            long pos = file.Position;
            if (!(file.ReadByte() == 0xff && file.ReadByte() == 0xd8))
                throw new Exception("Provided file is not a JPEG image!");
            file.Position = 0;
            //if the stream closes, the image object can't be saved - workaround
            //this works, somehow
            Image frame = new Bitmap(Image.FromStream(file));
            return new Image[1] { frame };
        }
    }
}
