using System;
using System.Drawing;

namespace Kiss.Utils.Imaging
{
    public class BaseImage : IImage
    {

        private Bitmap _bitmap;
        private Size _size;

        public BaseImage(Bitmap bitmap)
        {
            this._bitmap = bitmap;

            if (this._bitmap != null)
            {
                this._size = _bitmap.Size;
            }
        }

        #region IImage Members

        public int Height
        {
            get { return _size.Height; }
        }

        public int Width
        {
            get { return _size.Width; }
        }

        public Image Image
        {
            get
            {
                return _bitmap;
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException();
                }

                using (Bitmap old = _bitmap)
                {
                    try
                    {
                        _bitmap = new Bitmap(value);
                        _size = _bitmap.Size;
                    }
                    catch { }                    
                }
            }
        }

        public Color GetPixel(int x, int y)
        {
            try
            {
                return _bitmap.GetPixel(x, y);
            }
            catch (ArgumentException)
            {
                if (x < 0 || x >= Width)
                {
                    throw new ArgumentOutOfRangeException("x", x, "x ×ø±ê³¬³ö·¶Î§£¡");
                }
                if (y < 0 || y >= Height)
                {
                    throw new ArgumentOutOfRangeException("y", y, "y ×ø±ê³¬³ö·¶Î§£¡");
                }

                throw;
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            try
            {
                _bitmap.SetPixel(x, y, color);
            }
            catch (ArgumentException)
            {
                if (x < 0 || x >= Width)
                {
                    throw new ArgumentOutOfRangeException("x", x, "x ×ø±ê³¬³ö·¶Î§£¡");
                }
                if (y < 0 || y >= Height)
                {
                    throw new ArgumentOutOfRangeException("y", y, "y ×ø±ê³¬³ö·¶Î§£¡");
                }

                throw;
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new BaseImage(_bitmap);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }

        #endregion
    }
}
