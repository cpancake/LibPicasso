using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibPicasso.Handlers
{
    public class PNMImageHandler : IImageHandler
    {
        /// <summary>
        /// Tests whether the image handler handles this format hint.
        /// </summary>
        /// <param name="ext">The format hint.</param>
        /// <returns>Whether or not this image handler handles this format hint.</returns>
        public bool HasExtension(string ext)
        {
            return ext == "ppm";
        }

        /// <summary>
        /// Tests whether a Stream contains a PNM image.
        /// </summary>
        /// <param name="file">The Stream containing the file to test.</param>
        /// <returns>Whether the Stream contains a PNM image or not.</returns>
        public bool Test(Stream file)
        {
            byte[] magic = new byte[2];
            file.Read(magic, 0, 2);
            return (magic[0] == 0x50 && (magic[1] >= 0x31 || magic[1] <= 0x36));

        }

        /// <summary>
        /// Loads the frame or frames from a Stream.
        /// </summary>
        /// <param name="file">The Stream containing the PNM image to load.</param>
        /// <returns>An array containing the frame or frames of this image.</returns>
        public Image[] Load(Stream file)
        {
            byte[] buffer = new byte[file.Length];
            file.Read(buffer, 0, buffer.Length);
            Image frame = null;

            if (buffer[0] == 0x50 && (buffer[1] >= 0x31 && buffer[1] <= 0x33)) //P1 - P3 = ASCII
            {
                //todo: optimization
                file.Read(buffer, 0, buffer.Length);
                string data = Encoding.ASCII.GetString(buffer);
                StringReader reader = new StringReader(data);
                reader.ReadLine(); //magic number
                string line;
                int max = -1;
                if ((char)reader.Peek() == '#')
                    while (reader.Peek() == '#') reader.ReadLine(); //skip comments

                line = reader.ReadLine().Trim();
                string[] size = line.Split(' ');
                Bitmap frameBmp = new Bitmap(Convert.ToInt32(size[0]), Convert.ToInt32(size[1]));

                if (buffer[1] != 0x31) //PBM has no max value
                {
                    line = reader.ReadLine().Trim();
                    max = Convert.ToInt32(line);
                }

                int w = 0, h = 0;
                int[] rgb = new int[3];
                int count = 1;
                if (buffer[1] == 0x33) count = 3;
                while (h < frameBmp.Height)
                {
                    for (int c = 0; c < count; c++)
                    {
                        string color = "";
                        int ch = 0;
                        while ((ch = reader.Read()) != -1)
                        {
                            if (Utils.IsCharWhitespace((char)ch)) break;
                            if (ch == 0x0a) continue; //newline
                            color += (char)ch;
                        }
                        int col = Convert.ToInt32(color);
                        if (buffer[1] == 0x33)
                        {
                            if (max == 255)
                                rgb[c] = col;
                            else // i'm not sure if PPM images ever contain maxes other than 255, but just to be safe
                                rgb[c] = (int)Math.Floor(((double)col / (double)max) * 255);
                        }
                        else if (buffer[1] == 0x32 || buffer[1] == 0x35)
                            rgb[0] = rgb[1] = rgb[2] = (int)Math.Floor(((double)col / (double)max) * 255);
                        else
                            rgb[0] = rgb[1] = rgb[2] = (col == 1 ? 255 : 0);
                    }
                    frameBmp.SetPixel(w, h, Color.FromArgb(rgb[0], rgb[1], rgb[2]));
                    w++;
                    if (w >= frameBmp.Width)
                    {
                        w = 0;
                        h++;
                    }
                }
                frame = (Image)frameBmp;
            }
            else if (buffer[0] == 0x50 && (buffer[1] >= 0x34 && buffer[1] <= 0x36)) //P4 - P6 = Binary
            {
                //todo: PBM DOES NOT WORK. I don't know why.
                //adapted from http://rosettacode.org/wiki/Bitmap/Read_a_PPM_file#C.23
                BinaryReader reader = new BinaryReader(new MemoryStream(buffer));
                reader.ReadChars(3); //magic number + newline
                string widthStr = "", heightStr = "", maxStr = "0";
                char ch;
                if (reader.PeekChar() == '#') //who the hell thought putting ascii in binary files was a good idea
                    while (true)
                    {
                        while (reader.PeekChar() != (char)0x0a) 
                            reader.ReadChar();
                        if (reader.PeekChar() == '#')
                            continue;
                        else
                            break;
                    }
                while (reader.PeekChar() == (char)0x0a) reader.ReadChar(); //left over newlines aaaa
                while ((ch = reader.ReadChar()) != ' ')
                    widthStr += ch;
                while ((ch = reader.ReadChar()) >= '0' && ch <= '9')
                    heightStr += ch;
                if (buffer[1] != 0x34)
                {
                    while ((ch = reader.ReadChar()) >= '0' && ch <= '9')
                        maxStr += ch;
                }
                if (reader.PeekChar() == (char)0x0a) reader.ReadChar(); 
                int width = Convert.ToInt32(widthStr);
                int height = Convert.ToInt32(heightStr);
                int max = Convert.ToInt32(maxStr);
                Bitmap frameBmp = new Bitmap(width, height);
                for (int h = 0; h < height; h++)
                    for (int w = 0; w < width; w++)
                    {
                        if (buffer[1] == 0x36)
                        {
                            if (max == 255)
                                frameBmp.SetPixel(w, h, Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte()));
                            else
                            {
                                int r = reader.ReadByte();
                                int g = reader.ReadByte();
                                int b = reader.ReadByte();
                                Color c = Color.FromArgb((int)Math.Floor(((double)r / max) * 255), (int)Math.Floor(((double)g / max) * 255), (int)Math.Floor(((double)b / max) * 255));
                                frameBmp.SetPixel(w, h, c);
                            }
                        }
                        else if(buffer[1] == 0x35)
                        {
                            int col = (int)Math.Floor(((double)reader.ReadByte() / max) * 255);
                            frameBmp.SetPixel(w, h, Color.FromArgb(col, col, col));
                        }
                        else if (buffer[1] == 0x34)
                        {
                            int col = (reader.ReadByte() == 1 ? 255 : 0);
                            frameBmp.SetPixel(w, h, Color.FromArgb(col, col, col));
                        }
                    }
                frame = (Image)frameBmp;
                reader.Close();
            }
            else
                throw new Exception("Provided file is not a PNM image!");
            return new Image[1] { frame };
        }
    }
}
