using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace LibPicasso.Handlers
{
    public interface IImageHandler
    {
        /// <summary>
        /// Tests whether the image handler handles this format hint.
        /// </summary>
        /// <param name="ext">The format hint.</param>
        /// <returns>Whether or not this image handler handles this format hint.</returns>
        bool HasExtension(string ext);

        /// <summary>
        /// Tests whether a Stream contains this image format.
        /// </summary>
        /// <param name="file">The Stream containing the file to test.</param>
        /// <returns>Whether the Stream contains this format or not.</returns>
        bool Test(Stream file);

        /// <summary>
        /// Loads the frame or frames from a Stream.
        /// </summary>
        /// <param name="file">The Stream containing the file to load.</param>
        /// <returns>An array containing the frame or frames of this image.</returns>
        Image[] Load(Stream file);
    }
}
