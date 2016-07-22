using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Kiss.Utils.Imaging
{
    /// <summary>
    /// ͼƬת��������ʵ��
    /// </summary>
    public class ImpScaleTransform : IImageTransform
    {
        #region constructor

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public ImpScaleTransform(int width, int height)
            : this(width, height, false)
        {

        }

        public ImpScaleTransform(int width, int height, bool keepRatio)
        {
            Width = width;
            Height = height;
            KeepRatio = keepRatio;
        }

        #endregion

        #region public properties

        /// <summary>
        /// �Ƿ񱣳ֳ����
        /// </summary>
        public bool KeepRatio { get; set; }

        /// <summary>
        /// �����Ժ�Ŀ�ȣ���λ�����أ�
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// �����Ժ�ĸ߶ȣ���λ�����أ�
        /// </summary>
        public int Height { get; set; }

        #endregion

        #region IImageTransform Members

        /// <summary>
        /// ����ʵ��
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public virtual IImage Transform(IImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            int destinationWidth = Width;
            int destinationHeight = Height;
            int destinationX = 0;
            int destinationY = 0;

            IImage newImage = null;

            try
            {
                newImage = (IImage)image.Clone();

                using (Image innerImage = image.Image, scaled = new Bitmap(Width, Height, innerImage.PixelFormat))
                {
                    using (Graphics graphics = Graphics.FromImage(scaled))
                    {
                        float widthF = image.Width;
                        float heightF = image.Height;

                        float aspect = widthF / heightF;
                        float targetAspect = (float)Width / (float)Height;

                        if (KeepRatio && widthF != heightF)
                        {
                            float scale;

                            if (aspect < targetAspect)
                            {
                                // �仯�Ժ�߶ȿ�Ƚ�խ
                                scale = (float)Height / (float)heightF;
                            }
                            else
                            {
                                // �任�Ժ�Ŀ��ĸ߶Ƚ�С
                                scale = (float)Width / (float)widthF;
                            }

                            destinationHeight = System.Convert.ToInt32(heightF * scale);
                            destinationWidth = System.Convert.ToInt32(widthF * scale);
                            destinationX = (Width - destinationWidth) / 2;
                            destinationY = (Height - destinationHeight) / 2;
                        }

                        //����ͼƬ�����ŵ�ʱ����ø����������ŷ�ʽ
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        graphics.DrawImage(innerImage,
                            new Rectangle(destinationX, destinationY, destinationWidth, destinationHeight),
                            new Rectangle(0, 0, (int)widthF, (int)heightF),
                            GraphicsUnit.Pixel);
                    }

                    newImage.Image = scaled;
                }

                return newImage;
            }
            catch (Exception ex)
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }
                throw ex;
            }
        }

        #endregion

    }
}
