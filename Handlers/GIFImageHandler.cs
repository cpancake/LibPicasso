using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibPicasso.Handlers
{
    public class GIFImageHandler : IImageHandler
    {
        /// <summary>
        /// Tests whether the image handler handles this format hint.
        /// </summary>
        /// <param name="ext">The format hint.</param>
        /// <returns>Whether or not this image handler handles this format hint.</returns>
        public bool HasExtension(string ext)
        {
            return ext == "png";
        }

        /// <summary>
        /// Tests whether a Stream contains a GIF image.
        /// </summary>
        /// <param name="file">The Stream containing the file to test.</param>
        /// <returns>Whether the Stream contains a GIF image or not.</returns>
        public bool Test(Stream file)
        {
            return (file.ReadByte() == 0x47 && file.ReadByte() == 0x49 && file.ReadByte() == 0x46);
        }

        /// <summary>
        /// Loads the frame or frames from a Stream.
        /// </summary>
        /// <param name="file">The Stream containing the GIF image to load.</param>
        /// <returns>An array containing the frame or frames of this image.</returns>
        public Image[] Load(Stream file)
        {
            //todo: sacrifice to the sun gods and hope for moving pictures
            long pos = file.Position;
            if (!(file.ReadByte() == 0x47 && file.ReadByte() == 0x49 && file.ReadByte() == 0x46))
                throw new Exception("Provided file is not a GIF image!");
            file.Position = pos;
            Image frame = new Bitmap(Image.FromStream(file)); //again, see JPEG image handler
            return new Image[1] { frame };
        }
    }
}
