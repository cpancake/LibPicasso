using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibPicasso.Handlers
{
    public class PNGImageHandler : IImageHandler
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
        /// Tests whether a Stream contains a PNG image.
        /// </summary>
        /// <param name="file">The Stream containing the file to test.</param>
        /// <returns>Whether the Stream contains a PNG image or not.</returns>
        public bool Test(Stream file)
        {
            return (file.ReadByte() == 0x89 && file.ReadByte() == 0x50 && file.ReadByte() == 0x4e && file.ReadByte() == 0x47);
        }

        /// <summary>
        /// Loads the frame or frames from a Stream.
        /// </summary>
        /// <param name="file">The Stream containing the PNG image to load.</param>
        /// <returns>An array containing the frame or frames of this image.</returns>
        public Image[] Load(Stream file)
        {
            //todo: the miracle of animation
            long pos = file.Position;
            if (!(file.ReadByte() == 0x89 && file.ReadByte() == 0x50 && file.ReadByte() == 0x4e && file.ReadByte() == 0x47))
                throw new Exception("Provided file is not a PNG image!");
            file.Position = pos;
            Image frame = Image.FromStream(file);
            return new Image[1] { frame };
        }
    }
}
