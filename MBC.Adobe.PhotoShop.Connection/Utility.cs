/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MBC.Adobe.PhotoShop.Connection
{
    /// <summary>
    /// Collection of utility functions.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Test given string is PhotoShop acceptable, 
        /// ie. composed of ascii visible characters ([\x20-\x7E])
        /// </summary>
        /// <param name="strTest">string to be tested</param>
        /// <returns>PhotoShop acceptable or not</returns>
        public static bool IsPhotoShopPBKDF2AcceptableString(
            this string strTest)
        {
            var match = Regex.Match(strTest, "[\x20-\x7E]+");
            return match.Success;
        }

        /// <summary>
        /// generates RFC2898 PBKDF2 secure key used for PhotoShop Communication
        /// </summary>
        /// <param name="password">
        /// password to encode, 
        /// should be composed of ascii visible characters ([\x20-\x7E])
        /// </param>
        /// <returns>
        /// byte array containing generated RFC2898 PBKDF2 secure key.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// thrown when password argument contains characters 
        /// except ascii visible characters ([\x20-\x7E]) 
        /// </exception>
        public static byte[] DerivePBKDF2Key(
            this string password)
        {
            // password should be ascii visible characters ([\x20-\x7E])
            if (false == password.IsPhotoShopPBKDF2AcceptableString())
            {
                throw
                    new ArgumentOutOfRangeException(
                        "password",
                        "password should be composed of " + 
                            "ascii visible characters ([\x20-\x7E]).");
            }
            var passwordBytes = Encoding.ASCII.GetBytes(password);

            // salt value assumes ascii encoding
            var saltBytes = Encoding.ASCII.GetBytes(PhotoShopConstants.SALT);

            // generate pbkdf2 key
            var pbkdf2KeyGenerator =
                new System.Security.Cryptography.Rfc2898DeriveBytes(
                    passwordBytes,
                    saltBytes,
                    PhotoShopConstants.ITERATION_COUNT);

            // get key byte arrays with given key-length
            return
                pbkdf2KeyGenerator.GetBytes(PhotoShopConstants.KEY_LENGTH);
        }

        /// <summary>
        /// Utility function to convert byte array to hexa display format
        /// </summary>
        /// <param name="data">byte array to convert to string</param>
        /// <param name="padding">padding string for formating</param>
        /// <returns>
        /// hexa display string converted from <paramref name="data"/>
        /// </returns>
        public static string GetHexaDisplayString(
            this byte[] data,
            string padding = "")
        {
            if (null != data &&
                data.Length > 0)
            {
                var build = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    if (false == string.IsNullOrEmpty(padding) &&
                        0 == (i %8))
                        build.Append(padding);
                    
                    build.Append(data[i].ToString("x2") + " ");
                    if (i > 0 && (((i + 1) % 8) == 0) && ((i+1) != data.Length))
                        build.AppendLine();
                }

                return build.ToString();
            }

            return "NULL";
        }

        /// <summary>
        /// convert file path to PhotoShop acceptable path representation in Windows.
        /// </summary>
        /// <param name="strPath">path to convert</param>
        /// <returns>
        /// converted PhotoShop acceptable path representation in Windows.
        /// </returns>
        public static string ToPhotoShopPathString(
            this string strPath)
        {
            var isValidPathName = false;
            var absolutePath = string.Empty;
            try
            {
                var fileInfo = new FileInfo(strPath);
                absolutePath = fileInfo.FullName;
                isValidPathName = true;
            }
            catch { }


            return
                (isValidPathName) ?
                    "/" + absolutePath.Replace('\\', '/').Replace(":", "") :
                    string.Empty;
        }

        /// <summary>
        /// decode byte array to bitmap.
        /// It's assumed that we request PIXMAP type to PS 
        /// to get current thumbnail.
        /// So <paramref name="byteArray"/> must be of format PIXMAP.
        /// Also, if given param is null or less than expected, 
        /// null image is returned.
        /// This function doesn't care about Exception!!!
        /// Be sure to enclose this function call with exception handling
        /// </summary>
        /// <param name="byteArray">
        /// byte array containing PIXMAP
        /// </param>
        /// <returns>
        /// Image decoded from <paramref name="byteArray"/> 
        /// </returns>
        public static Image DecodeArray(
            this byte[] byteArray)
        {
            if (null == byteArray ||
                byteArray.Length < 16)
                return null;

            using (var memStream = new MemoryStream(byteArray))
            {
                using (var brReader = new BinaryReader(memStream))
                {
                    var pixmapIndicator = brReader.ReadByte();
                    var width = IPAddress.NetworkToHostOrder(brReader.ReadInt32());
                    var height = IPAddress.NetworkToHostOrder(brReader.ReadInt32());
                    var rowBytes = IPAddress.NetworkToHostOrder(brReader.ReadInt32());
                    byte colorMode = brReader.ReadByte();
                    if (1 != colorMode)
                        return null;
                    var channelCount = brReader.ReadByte();
                    if (1 != channelCount && 3 != channelCount)
                        return null;
                    var bitsPerChannel = brReader.ReadByte();
                    if (8 != bitsPerChannel)
                        return null;

                    var extra = rowBytes - width * channelCount;

                    var retBmp = new Bitmap(width, height);
                    Func<int, int, int, int, int> pack8888 =
                        (r, g, b, a) =>
                            (r << 0) |
                            (g << 8) |
                            (b << 16) |
                            (a << 24);

                    for (int y = 0; y < height; y++, memStream.Position += extra)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int color = 0;
                            // 1 or 3 for now
                            switch (channelCount)
                            {
                                case 1:
                                    var rgb = brReader.ReadByte();
                                    color = pack8888(rgb, rgb, rgb, 255);
                                    break;
                                case 3:
                                    var b = brReader.ReadByte();
                                    var g = brReader.ReadByte();
                                    var r = brReader.ReadByte();
                                    color = pack8888(r, g, b, 255);
                                    break;
                                default:
                                    break;
                            }
                            retBmp.SetPixel(x, y, Color.FromArgb(color));
                        }
                    }

                    return retBmp;
                }
            }
        }

        /// <summary>
        /// convert string value to <see cref="Color"/>
        /// notice that given string is passed from PhotoShop as of format
        /// "RRGGBB" hex values
        /// </summary>
        /// <param name="toConvert">string to convert to Color</param>
        /// <returns>
        /// converted color.
        /// if given string is not convertable
        /// (as such, null string, string is not of format "RRGGBB", etc)
        /// then <see cref="Color.Empty"/> is returned.
        /// </returns>
        public static Color convertToColor(
            this string toConvert)
        {
            if (string.IsNullOrEmpty(toConvert))
                return Color.Empty;
            if (toConvert.Length < 6)
                return Color.Empty;

            try
            {
                var red   = int.Parse(toConvert.Substring(0, 2), NumberStyles.HexNumber);
                var green = int.Parse(toConvert.Substring(2, 2), NumberStyles.HexNumber);
                var blue  = int.Parse(toConvert.Substring(4, 2), NumberStyles.HexNumber);
                return Color.FromArgb(red, green, blue);
            }
            catch
            {
                return Color.Empty;
            }
        }
    }
}
