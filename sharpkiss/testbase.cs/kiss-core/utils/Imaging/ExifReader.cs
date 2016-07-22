using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Kiss.Utils.Imaging
{
    public class ExifReader
    {
        //
        private System.Drawing.Bitmap bmp;
        //
        private Dictionary<ExifProperty, string> properties = new Dictionary<ExifProperty, string>();

        /// <summary>
        ///  Ù–‘ºØ∫œ
        /// </summary>
        public Dictionary<ExifProperty, string> ExifProperties
        {
            get { return properties; }
        }

        //
        internal int Count
        {
            get
            {
                return this.properties.Count;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="sp"></param>
        public ExifReader(ref System.Drawing.Bitmap bmp)
        {
            //
            this.bmp = bmp;
            //
            buildDB(this.bmp.PropertyItems);
        }

        public ExifReader(string file)
        {
            //				
            this.buildDB(GetExifProperties(file));

        }


        public ExifReader(Stream stream)
        {
            //				
            this.buildDB(GetExifProperties(stream));

        }

        public static PropertyItem[] GetExifProperties(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            return GetExifProperties(stream);
        }
        
        public static PropertyItem[] GetExifProperties(Stream stream)
        { 
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream,
                /* useEmbeddedColorManagement = */ true,
                /* validateImageData = */ false);
            return image.PropertyItems;            
        }

        string getString(byte[] buffer)
        {
            Encoding encoding = Encoding.UTF8;
            return getString(encoding, buffer);
        }

        string getString(Encoding encoding, byte[] buffer)
        {
            return cleanZeroCharacter(encoding.GetString(buffer));
        }

        /// <summary>
        /// 
        /// </summary>
        private void buildDB(System.Drawing.Imaging.PropertyItem[] parr)
        {
            properties.Clear();
            //
            //
            //
            foreach (System.Drawing.Imaging.PropertyItem p in parr)
            {
                if (!Enum.IsDefined(typeof(ExifProperty), p.Id)) continue;
                string v = "";

                // tag not found. skip it
                //
                ExifProperty name = (ExifProperty)p.Id; ;

                //1 = BYTE An 8-bit unsigned integer.,
                if (p.Type == 0x1)
                {
                    if (p.Id >= 0x9C9B && p.Id <= 0x9C9F)
                        v = cleanZeroCharacter(Encoding.Unicode.GetString(p.Value));
                    else
                        v = cleanZeroCharacter(p.Value[0].ToString());
                }
                //2 = ASCII An 8-bit byte containing one 7-bit ASCII code. The final byte is terminated with NULL.,
                else if (p.Type == 0x2)
                {
                    // string					
                    v = getString(p.Value);
                }
                //3 = SHORT A 16-bit (2 -byte) unsigned integer,
                else if (p.Type == 0x3)
                {
                    // orientation // lookup table					
                    switch (p.Id)
                    {
                        case 0x8827: // ISO
                            v = "ISO-" + convertToInt16U(p.Value).ToString();
                            break;
                        case 0xA217: // sensing method
                            {
                                switch (convertToInt16U(p.Value))
                                {
                                    case 1: v = "Not defined"; break;
                                    case 2: v = "One-chip color area sensor"; break;
                                    case 3: v = "Two-chip color area sensor"; break;
                                    case 4: v = "Three-chip color area sensor"; break;
                                    case 5: v = "Color sequential area sensor"; break;
                                    case 7: v = "Trilinear sensor"; break;
                                    case 8: v = "Color sequential linear sensor"; break;
                                    default: v = " reserved"; break;
                                }
                            }
                            break;
                        case 0x8822: // aperture 
                            switch (convertToInt16U(p.Value))
                            {
                                case 0: v = "Not defined"; break;
                                case 1: v = "Manual"; break;
                                case 2: v = "Normal program"; break;
                                case 3: v = "Aperture priority"; break;
                                case 4: v = "Shutter priority"; break;
                                case 5: v = "Creative program (biased toward depth of field)"; break;
                                case 6: v = "Action program (biased toward fast shutter speed)"; break;
                                case 7: v = "Portrait mode (for closeup photos with the background out of focus)"; break;
                                case 8: v = "Landscape mode (for landscape photos with the background in focus)"; break;
                                default: v = "reserved"; break;
                            }
                            break;
                        case 0x9207: // metering mode
                            switch (convertToInt16U(p.Value))
                            {
                                case 0: v = "unknown"; break;
                                case 1: v = "Average"; break;
                                case 2: v = "CenterWeightedAverage"; break;
                                case 3: v = "Spot"; break;
                                case 4: v = "MultiSpot"; break;
                                case 5: v = "Pattern"; break;
                                case 6: v = "Partial"; break;
                                case 255: v = "Other"; break;
                                default: v = "reserved"; break;
                            }
                            break;
                        case 0x9208: // light source
                            {
                                switch (convertToInt16U(p.Value))
                                {
                                    case 0: v = "unknown"; break;
                                    case 1: v = "Daylight"; break;
                                    case 2: v = "Fluorescent"; break;
                                    case 3: v = "Tungsten"; break;
                                    case 17: v = "Standard light A"; break;
                                    case 18: v = "Standard light B"; break;
                                    case 19: v = "Standard light C"; break;
                                    case 20: v = "D55"; break;
                                    case 21: v = "D65"; break;
                                    case 22: v = "D75"; break;
                                    case 255: v = "other"; break;
                                    default: v = "reserved"; break;
                                }
                            }
                            break;
                        case 0x9209:
                            {
                                switch (convertToInt16U(p.Value))
                                {
                                    case 0: v = "Flash did not fire"; break;
                                    case 1: v = "Flash fired"; break;
                                    case 5: v = "Strobe return light not detected"; break;
                                    case 7: v = "Strobe return light detected"; break;
                                    default: v = "reserved"; break;
                                }
                            }
                            break;
                        default:
                            v = convertToInt16U(p.Value).ToString();
                            break;
                    }
                }
                //4 = LONG A 32-bit (4 -byte) unsigned integer,
                else if (p.Type == 0x4)
                {
                    // orientation // lookup table					
                    v = convertToInt32U(p.Value).ToString();
                }
                //5 = RATIONAL Two LONGs. The first LONG is the numerator and the second LONG expresses the//denominator.,
                else if (p.Type == 0x5)
                {
                    // rational
                    byte[] n = new byte[p.Len / 2];
                    byte[] d = new byte[p.Len / 2];
                    Array.Copy(p.Value, 0, n, 0, p.Len / 2);
                    Array.Copy(p.Value, p.Len / 2, d, 0, p.Len / 2);
                    uint a = convertToInt32U(n);
                    uint b = convertToInt32U(d);
                    Rational r = new Rational(a, b);
                    //
                    //convert here
                    //
                    switch (p.Id)
                    {
                        case 0x9202: // aperture
                            v = "F/" + Math.Round(Math.Pow(Math.Sqrt(2), r.ToDouble()), 2).ToString();
                            break;
                        case 0x920A:
                            v = r.ToDouble().ToString();
                            break;
                        case 0x829A:
                            v = r.ToDouble().ToString();
                            break;
                        case 0x829D: // F-number
                            v = "F/" + r.ToDouble().ToString();
                            break;
                        default:
                            v = r.ToString("/");
                            break;
                    }

                }
                //7 = UNDEFINED An 8-bit byte that can take any value depending on the field definition,
                else if (p.Type == 0x7)
                {
                    switch (p.Id)
                    {
                        case 0xA000:
                            v = getString(p.Value);
                            break;
                        case 0xA300:
                            {
                                if (p.Value[0] == 3)
                                {
                                    v = "DSC";
                                }
                                else
                                {
                                    v = "reserved";
                                }
                                break;
                            }
                        case 0xA301:
                            if (p.Value[0] == 1)
                                v = "A directly photographed image";
                            else
                                v = "Not a directly photographed image";
                            break;
                        case 0x9286:
                            if (Encoding.UTF8.GetString(p.Value, 0, 8) == "UNICODE\0")
                                v = cleanZeroCharacter(Encoding.Unicode.GetString(p.Value, 8, p.Len - 8));
                            else
                                v = getString(p.Value);
                            break;
                        case 36864:
                            v = getString(p.Value);
                            break;
                        default:
                            v = "-";
                            break;
                    }
                }
                //9 = SLONG A 32-bit (4 -byte) signed integer (2's complement notation),
                else if (p.Type == 0x9)
                {
                    v = convertToInt32(p.Value).ToString();
                }
                //10 = SRATIONAL Two SLONGs. The first SLONG is the numerator and the second SLONG is the
                //denominator.
                else if (p.Type == 0xA)
                {

                    // rational
                    byte[] n = new byte[p.Len / 2];
                    byte[] d = new byte[p.Len / 2];
                    Array.Copy(p.Value, 0, n, 0, p.Len / 2);
                    Array.Copy(p.Value, p.Len / 2, d, 0, p.Len / 2);
                    int a = convertToInt32(n);
                    int b = convertToInt32(d);
                    Rational r = new Rational(a, b);
                    //
                    // convert here
                    //
                    switch (p.Id)
                    {
                        case 0x9201: // shutter speed
                            v = "1/" + Math.Round(Math.Pow(2, r.ToDouble()), 2).ToString();
                            break;
                        case 0x9203:
                            v = Math.Round(r.ToDouble(), 4).ToString();
                            break;
                        default:
                            v = r.ToString("/");
                            break;
                    }
                }
                // add it to the list
                properties[name] = v;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        int convertToInt32(byte[] arr)
        {
            if (arr.Length != 4)
                return 0;
            else
                return arr[3] << 24 | arr[2] << 16 | arr[1] << 8 | arr[0];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        int convertToInt16(byte[] arr)
        {
            if (arr.Length != 2)
                return 0;
            else
                return arr[1] << 8 | arr[0];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        uint convertToInt32U(byte[] arr)
        {
            if (arr.Length != 4)
                return 0;
            else
                return Convert.ToUInt32(arr[3] << 24 | arr[2] << 16 | arr[1] << 8 | arr[0]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        uint convertToInt16U(byte[] arr)
        {
            if (arr.Length != 2)
                return 0;
            else
                return Convert.ToUInt16(arr[1] << 8 | arr[0]);
        }



        private string cleanZeroCharacter(string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
                return rawText;

            return rawText.Replace("\0", "");
        }
    }

    /// <summary>
    /// private class
    /// </summary>
    internal class Rational
    {
        private int n;
        private int d;
        public Rational(int n, int d)
        {
            this.n = n;
            this.d = d;
            simplify(ref this.n, ref this.d);
        }
        public Rational(uint n, uint d)
        {
            this.n = Convert.ToInt32(n);
            this.d = Convert.ToInt32(d);

            simplify(ref this.n, ref this.d);
        }
        public Rational()
        {
            this.n = this.d = 0;
        }
        public string ToString(string sp)
        {
            if (sp == null) sp = "/";
            return n.ToString() + sp + d.ToString();
        }
        public double ToDouble()
        {
            if (d == 0)
                return 0.0;

            return Math.Round(Convert.ToDouble(n) / Convert.ToDouble(d), 2);
        }
        private void simplify(ref int a, ref int b)
        {
            if (a == 0 || b == 0)
                return;

            int gcd = euclid(a, b);
            a /= gcd;
            b /= gcd;
        }
        private int euclid(int a, int b)
        {
            if (b == 0)
                return a;
            else
                return euclid(b, a % b);
        }
    }
}
