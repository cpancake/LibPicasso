using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using LibPicasso.Handlers;

namespace LibPicasso
{
    public class ImageFile
    {
        private bool _animated;
        private Image _image;
        private Image[] _frames;
        private float[] _frameduration;

        private static IImageHandler[] handlers;

        /// <summary>
        /// Creates an empty ImageFile object.
        /// </summary>
        public ImageFile() { }

        /// <summary>
        /// Creates an ImageFile object from a file path.
        /// </summary>
        /// <param name="path">The path to the image to load.</param>
        public ImageFile(string path)
        {
            Stream file = File.Open(path, FileMode.Open);
            this.Load(file, Path.GetExtension(path));
            file.Close();
        }

        /// <summary>
        /// Creates an ImageFile object from a Stream and a format hint.
        /// </summary>
        /// <param name="file">The stream objet to load the image from.</param>
        /// <param name="hint">The file format hint - usually the file extension.</param>
        public ImageFile(Stream file, string hint)
        {
            this.Load(file, hint);
        }

        /// <summary>
        /// Creates an ImageFile object from a Stream, and attempts to figure out its format.
        /// </summary>
        /// <param name="file">The stream object to load the image from.</param>
        public ImageFile(Stream file)
        {
            this.Load(file, null);
        }

        /// <summary>
        /// Loads an image into an ImageFile object.
        /// </summary>
        /// <param name="file">The stream object to load the image from.</param>
        /// <param name="hint">The file format hint - usually the file extension.</param>
        public void Load(Stream file, string hint)
        {
            if (handlers == null)
                this.initializeHandlers();
            if (hint.StartsWith("."))
                hint = hint.TrimStart('.');
            IImageHandler handler = null;
            long pos = file.Position;
            foreach (IImageHandler h in handlers)
            {
                file.Position = pos;
                if (h.HasExtension(hint) && h.Test(file))
                {
                    handler = h;
                    break;
                }
                file.Position = pos;
                if (h.Test(file))
                {
                    handler = h;
                    break;
                }
            }
            file.Position = pos;
            if (handler == null)
                throw new Exception("Cannot process input file");
            this._frames = handler.Load(file);
            this._image = this._frames[0];
            this._animated = this._frames.Length > 1;
        }

        //better way to do this? probably there is
        private void initializeHandlers()
        {
            handlers = new IImageHandler[] { 
                new JPEGImageHandler(),
                new PNGImageHandler(),
                new GIFImageHandler(),
                new BitmapImageHandler(),
                new TIFFImageHandler(),
                new PNMImageHandler()
            };
        }

        /// <summary>
        /// Get the first frame of this image.
        /// </summary>
        public Image Image
        {
            get { return this._image; }
        }

        /// <summary>
        /// Returns whether or not the image is animated.
        /// </summary>
        public bool Animated
        {
            get { return this._animated; }
        }

        /// <summary>
        /// Returns a list of frames in this image.
        /// </summary>
        public Image[] Frames
        {
            get { return this._frames; }
        }

        /// <summary>
        /// Returns the duration of each frame.
        /// </summary>
        public float[] FrameDuration
        {
            get { return (this._animated ? this._frameduration : new float[] { 0f }); }
        }
    }
}
