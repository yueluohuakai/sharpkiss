using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Kiss.Utils.Imaging
{
    /// <summary>
    /// ����Ϊ������ͼƬ��ʵ�֣�������ͷ����
    /// </summary>
    public sealed class ImpSquareScaleTransform : ImpScaleTransform
    {

        #region constructor

        public ImpSquareScaleTransform(int w):base(w,w,true)
        {
            
        }

        #endregion

        public override IImage Transform(IImage image)
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

                using (System.Drawing.Image innerImage = image.Image, scaled = new Bitmap(Width, Height, innerImage.PixelFormat))
                {
                    using (Graphics graphics = Graphics.FromImage(scaled))
                    {

                        float widthF = image.Width;
                        float heightF = image.Height;

                        float srcX = 0f;
                        float srcY = 0f;

                        if (widthF > heightF)
                        {
                            //��ȴ��ڸ߶�
                            srcX = (widthF - heightF) / 2;
                            widthF = heightF;
                        }
                        else if (heightF > widthF)
                        {
                            srcY = (heightF - widthF) / 2;
                            heightF = widthF;
                        }            

                        //����ͼƬ�����ŵ�ʱ����ø����������ŷ�ʽ
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        graphics.DrawImage(innerImage,
                            new Rectangle(destinationX, destinationY, destinationWidth, destinationHeight),
                            new Rectangle(Convert.ToInt32( srcX), Convert.ToInt32(srcY),Convert.ToInt32(widthF),Convert.ToInt32( heightF)),
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
    }
}
