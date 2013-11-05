using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibPicasso.Handlers
{
    //todo: fix
    public class TIFFImageHandler : IImageHandler
    {
        /// <summary>
        /// Tests whether the image handler handles this format hint.
        /// </summary>
        /// <param name="ext">The format hint.</param>
        /// <returns>Whether or not this image handler handles this format hint.</returns>
        public bool HasExtension(string ext)
        {
            return ext == "tif" || ext == "tiff";
        }

        /// <summary>
        /// Tests whether a Stream contains a TIFF image.
        /// </summary>
        /// <param name="file">The Stream containing the file to test.</param>
        /// <returns>Whether the Stream contains a TIFF image or not.</returns>
        public bool Test(Stream file)
        {
            byte byteorder1 = (byte)file.ReadByte();
            byte byteorder2 = (byte)file.ReadByte();
            return (((byteorder1 == 0x49 && byteorder2 == 0x49) || (byteorder1 == 0x4D && byteorder2 == 0x4d)) && file.ReadByte() == 0x2a);
        }

        /// <summary>
        /// Loads the frame or frames from a Stream.
        /// </summary>
        /// <param name="file">The Stream containing the TIFF image to load.</param>
        /// <returns>An array containing the frame or frames of this image.</returns>
        public Image[] Load(Stream file)
        {
            long pos = file.Position;
            byte byteorder1 = (byte)file.ReadByte();
            byte byteorder2 = (byte)file.ReadByte();
            if (!((byteorder1 == 0x49 && byteorder2 == 0x49) || (byteorder1 == 0x4D && byteorder2 == 0x4d)) && file.ReadByte() == 0x2a)
                throw new Exception("Provided file is not a TIFF image!");
            file.Position = pos;
            Image frame = new Bitmap(Image.FromStream(file)); // see JPEG handler
            return new Image[1] { frame };
        }
    }
}
