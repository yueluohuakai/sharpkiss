using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Kiss.Utils.Imaging
{
	public static class ImageUtil
	{
		private static Dictionary<string, ImageFormat> _mimeMap;

		#region ctor

		static ImageUtil()
		{
			_mimeMap = new Dictionary<string, ImageFormat>(4);

			_mimeMap.Add("image/gif", ImageFormat.Gif);
			_mimeMap.Add("image/jpeg", ImageFormat.Jpeg);
			_mimeMap.Add("image/jpg", ImageFormat.Jpeg);
			_mimeMap.Add("image/png", ImageFormat.Png);
		}

		#endregion

		#region Transform

		/// <summary>
		/// 图片转换
		/// </summary>
		/// <param name="image">图片对象</param>
		/// <param name="tf">转换实现</param>
		public static Image Transform(Image image, IImageTransform tf)
		{
			return Transform(new Bitmap(image), tf);
		}

		/// <summary>
		/// 图片转换
		/// </summary>
		/// <param name="bitmap">位图</param>
		/// <param name="tf">转换实现</param>
		public static Image Transform(Bitmap bitmap, IImageTransform tf)
		{
			BaseImage baseImage = new BaseImage(bitmap);
			return Transform(baseImage, tf);
		}

		/// <summary>
		/// 图片转换
		/// </summary>
		/// <param name="image">IImage对象</param>
		/// <param name="tf">转换实现</param>
		public static Image Transform(IImage image, IImageTransform tf)
		{
			Image img = tf.Transform(image).Image;

			return img;
		}

		#endregion

		#region Thumbnail Image

		/// <summary>
		/// 获取正方形的缩略图
		/// </summary>
		/// <param name="path"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Image GetThumbnailImage(Image image, int size)
		{
			if (image == null)
			{
				return null;
			}

			ImpSquareScaleTransform isst = new ImpSquareScaleTransform(size);

			return Transform(image, isst);
		}

		/// <summary>
		/// 获取缩略图(保持图片比例)
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static Image GetThumbnailImage(Image image, int width, int height)
		{
			return GetThumbnailImage(image, width, height, true);
		}

		/// <summary>
		/// 获取缩略图
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="keepRatio"></param>
		/// <returns></returns>
		public static Image GetThumbnailImage(Image image, int width, int height, bool keepRatio)
		{
			if (image == null)
			{
				return null;
			}

			ImpScaleTransform ist = new ImpScaleTransform(width, height, keepRatio);
			return Transform(image, ist);
		}

		/// <summary>
		/// 返回固定宽度的缩略图
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public static Image GetThumbnailImageWithWidth(Image image, int width)
		{
			if (image == null)
				return null;

			int height = image.Height * width / image.Width;
			return GetThumbnailImage(image, width, height, true);
		}

		/// <summary>
		/// 返回固定高度的缩略图
		/// </summary>
		/// <param name="image"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static Image GetThumbnailImageWithHeight(Image image, int height)
		{
			if (image == null)
				return null;

			int width = image.Width * height / image.Height;
			return GetThumbnailImage(image, width, height, true);
		}

		/// <summary>
		/// 返回固定长度和宽度的缩略图，超出部分裁剪
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static Image GetThumbnailImageWithCutOut(Image image, int width, int height)
		{
			if (image == null)
				return null;

			double imageHeightRatio = (double)image.Height / image.Width;
			double heightRatio = (double)height / width;

			Image newImage;
			Rectangle srcRectangle;
			if (heightRatio == imageHeightRatio) // 比例一样，不需要裁
			{
				newImage = GetThumbnailImageWithHeight(image, height);
				return newImage;
			}
			else if (heightRatio > imageHeightRatio) // 裁左右 
			{
				newImage = GetThumbnailImageWithHeight(image, height);
				srcRectangle = new Rectangle((newImage.Width - width) / 2, 0, width, height);
			}
			else
			{ // 裁尾
				newImage = GetThumbnailImageWithWidth(image, width);
				srcRectangle = new Rectangle(0, 0, width, height);
			}
			Image timg = GetThumbnailImageEx(newImage, width, height, srcRectangle);
			newImage.Dispose();
			return timg;

		}

		/// <summary>
		/// 获取缩略图尺寸
		/// 注：假设Width >= Height
		/// 如果缩略图尺寸比原图大，那么保持原图大小
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static Size GetThumbnailImageSize(int imageWidth, int imageHeight, int width, int height)
		{
			if ((imageWidth == width && imageHeight == height) || (imageWidth == height && imageHeight == width))
				return new Size(imageWidth, imageHeight);

			int desiredWidth, desiredHeight;
			double heightRatio = (double)imageHeight / imageWidth;
			double widthRatio = (double)imageWidth / imageHeight;

			// 如果是竖向的图片
			//
			if (widthRatio < 1)
			{
				desiredHeight = width;
				desiredWidth = height;
			}
			else
			{
				desiredHeight = height;
				desiredWidth = width;
			}

			if (imageWidth < desiredWidth && imageHeight < desiredHeight)
				return new Size(imageWidth, imageHeight);


			height = desiredHeight;

			if (widthRatio > 0)
				width = Convert.ToInt32(height * widthRatio);

			if (width > desiredWidth)
			{
				width = desiredWidth;
				height = Convert.ToInt32(width * heightRatio);
			}

			return new Size(width, height);
		}

		#endregion

		#region Thumbnail Image Ex
		/// <summary>
		/// 获取正方形的缩略图
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public static Image GetThumbnailImageEx(Image image, int width)
		{
			if (image == null || width < 1)
				return null;
			Rectangle srcRectangle;
			int x, y, w;

			if (image.Width > image.Height)
			{
				x = (image.Width - image.Height) / 2;
				y = 0;
				w = image.Height;
			}
			else
			{
				x = 0;
				y = (image.Height - image.Width) / 2;
				w = image.Width;
			}
			srcRectangle = new Rectangle(x, y, w, w);

			return GetThumbnailImageEx(image, width, width, srcRectangle);
		}

		/// <summary>
		/// 获取缩略图(保持图片比例)
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static Image GetThumbnailImageEx(Image image, int width, int height)
		{
			return GetThumbnailImageEx(image, width, height, true);
		}

		/// <summary>
		/// 缩略图
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="maintainRatio">是否保持图片比例</param>
		/// <returns></returns>
		public static Image GetThumbnailImageEx(Image image, int width, int height, bool maintainRatio)
		{
			if (image == null || width < 1 || height < 1)
				return null;

			// 保持比例
			//
			if (maintainRatio)
			{
				double heightRatio = (double)image.Height / image.Width;
				double widthRatio = (double)image.Width / image.Height;

				int desiredHeight = height;
				int desiredWidth = width;


				height = desiredHeight;
				if (widthRatio > 0)
					width = Convert.ToInt32(height * widthRatio);

				if (width > desiredWidth)
				{
					width = desiredWidth;
					height = Convert.ToInt32(width * heightRatio);
				}
			}

			Rectangle srcRectangle = new Rectangle(0, 0, image.Width, image.Height);

			return GetThumbnailImageEx(image, width, height, srcRectangle);
		}

		private static Image GetThumbnailImageEx(Image image, int width, int height, Rectangle srcRectangle)
		{
			if (image == null || width < 1 || height < 1)
				return null;

			// 新建一个bmp图片
			//
			Image bitmap = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb);

			// 新建一个画板
			//
			using (Graphics g = System.Drawing.Graphics.FromImage(bitmap))
			{

				// 设置高质量插值法
				//
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;

				// 设置高质量,低速度呈现平滑程度
				//
				g.SmoothingMode = SmoothingMode.HighQuality; // SmoothingMode.AntiAlias;

				// 高质量、低速度复合
				//
				g.CompositingQuality = CompositingQuality.HighQuality;

				g.PixelOffsetMode = PixelOffsetMode.HighQuality;


				// 清空画布并以透明背景色填充
				//
				g.Clear(Color.Transparent);

				// 在指定位置并且按指定大小绘制原图片的指定部分
				//
				g.DrawImage(image, new Rectangle(0, 0, width, height), srcRectangle, GraphicsUnit.Pixel);

				return bitmap;
			}
		}


		/// <summary>
		/// 获取图像编码解码器的所有相关信息
		/// </summary>
		/// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
		/// <returns>返回图像编码解码器的所有相关信息</returns>
		public static ImageCodecInfo GetImageCodecInfo(string mimeType)
		{
			ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
			foreach (ImageCodecInfo ici in CodecInfo)
			{
				if (ici.MimeType == mimeType)
					return ici;
			}
			return null;
		}

		public static ImageCodecInfo GetImageCodecInfo(ImageFormat format)
		{
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

			foreach (ImageCodecInfo icf in encoders)
			{
				if (icf.FormatID == format.Guid)
				{
					return icf;
				}
			}

			return null;
		}

		public static void SaveImage(Image image, string savePath)
		{
			SaveImage(image, savePath, GetImageFormat(savePath));
		}

		public static void SaveImage(Image image, string savePath, ImageFormat format)
		{
			SaveImage(image, savePath, GetImageCodecInfo(format));
		}

		/// <summary>
		/// 保存图片
		/// </summary>
		/// <param name="image"></param>
		/// <param name="savePath"></param>
		/// <param name="ici"></param>
		public static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
		{
			string dir = Path.GetDirectoryName(savePath);
			if (!Directory.Exists(dir))
				FileUtil.CreateDirectory(dir);

			image.Save(savePath, ici, null);
		}
		#endregion

		#region WaterMark

		/// <summary>
		/// 添加水印文字到图片中
		/// </summary>
		/// <param name="picture"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="watermarkText"></param>
		/// <param name="position"></param>
		public static void AddWatermarkText(ref Graphics picture, int width, int height, string watermarkText, WatermarkPosition position)
		{
			int alpha = 80;
			AddWatermarkText(ref picture, width, height, watermarkText, position, alpha);
		}

		/// <summary>
		/// 添加水印文字到图片中
		/// </summary>
		/// <param name="picture"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="watermarkText"></param>
		/// <param name="position"></param>
		/// <param name="alpha"></param>
		public static void AddWatermarkText(ref Graphics g, int width, int height, string watermarkText, WatermarkPosition position, int alpha)
		{
			int[] sizes = new int[] { 22, 20, 18, 16, 14, 12, 10, 8, 6 };
			Font crFont = null;
			SizeF crSize = new SizeF();
			for (int i = 0; i < 9; i++)
			{
				crFont = new Font("黑体", sizes[i], FontStyle.Bold);
				crSize = g.MeasureString(watermarkText, crFont);

				if ((ushort)crSize.Width < (ushort)width)
					break;
			}

			float xpos = 0;
			float ypos = 0;

			switch (position)
			{
				case WatermarkPosition.TOP_LEFT:
					xpos = ((float)width * (float).01) + (crSize.Width / 2);
					ypos = (float)height * (float).01;
					break;
				case WatermarkPosition.TOP_CENTER:
					xpos = ((float)width - crSize.Width) / 2;
					ypos = (float)height * (float).01;
					break;
				case WatermarkPosition.TOP_RIGHT:
					xpos = ((float)width * (float).99) - (crSize.Width / 2);
					ypos = (float)height * (float).01;
					break;
				case WatermarkPosition.CENTER_LEFT:
					xpos = ((float)width * (float).01) + (crSize.Width / 2);
					ypos = ((float)height - crSize.Height) / 2;
					break;
				case WatermarkPosition.CENTER_CENTER:
					xpos = ((float)width - crSize.Width) / 2;
					ypos = ((float)height - crSize.Height) / 2;
					break;
				case WatermarkPosition.CENTER_RIGHT:
					xpos = ((float)width * (float).99) - (crSize.Width / 2);
					ypos = ((float)height - crSize.Height) / 2;
					break;
				case WatermarkPosition.BOTTOM_RIGHT:
					xpos = ((float)width * (float).99) - (crSize.Width / 2);
					ypos = ((float)height * (float).99) - crSize.Height;
					break;
				case WatermarkPosition.BOTTOM_CENTER:
					xpos = ((float)width - crSize.Width) / 2;
					ypos = ((float)height * (float).99) - crSize.Height;
					break;
				case WatermarkPosition.BOTTOM_LEFT:
					xpos = ((float)width * (float).01) + (crSize.Width / 2);
					ypos = ((float)height * (float).99) - crSize.Height;
					break;
			}

			//StringFormat StrFormat = new StringFormat();
			//StrFormat.Alignment = StringAlignment.Center;

			SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
			//g.DrawString(watermarkText, crFont, semiTransBrush2, xpos + 1, ypos + 1, StrFormat);
			g.DrawString(watermarkText, crFont, semiTransBrush2, xpos + 1, ypos + 1);

			SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255));
			//g.DrawString(watermarkText, crFont, semiTransBrush, xpos, ypos, StrFormat);
			g.DrawString(watermarkText, crFont, semiTransBrush, xpos, ypos);


			semiTransBrush2.Dispose();
			semiTransBrush.Dispose();
		}

		/// <summary>
		/// 添加水印图片到图片中
		/// </summary>
		/// <param name="picture"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="watermark"></param>
		/// <param name="position"></param>
		public static void AddWatermarkImage(ref Graphics picture, int width, int height, Image watermark, WatermarkPosition position)
		{

			using (ImageAttributes imageAttributes = new ImageAttributes())
			{
				ColorMap colorMap = new ColorMap();

				colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
				colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
				ColorMap[] remapTable = { colorMap };

				imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

				float[][] colorMatrixElements = {
												new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
												new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
												new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
												new float[] {0.0f,  0.0f,  0.0f,  1.0f, 0.0f},
												new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
											};

				ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

				imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

				int xpos = 0;
				int ypos = 0;

				switch (position)
				{
					case WatermarkPosition.TOP_LEFT:
						xpos = 10;
						ypos = 10;
						break;
					case WatermarkPosition.TOP_CENTER:
						xpos = (width - watermark.Width) / 2;
						ypos = 10;
						break;
					case WatermarkPosition.TOP_RIGHT:
						xpos = ((width - watermark.Width) - 10);
						ypos = 10;
						break;
					case WatermarkPosition.CENTER_RIGHT:
						xpos = ((width - watermark.Width) - 10);
						ypos = (height - watermark.Height) / 2;
						break;
					case WatermarkPosition.CENTER_CENTER:
						xpos = (width - watermark.Width) / 2;
						ypos = (height - watermark.Height) / 2;
						break;
					case WatermarkPosition.CENTER_LEFT:
						xpos = 10;
						ypos = (height - watermark.Height) / 2;
						break;
					case WatermarkPosition.BOTTOM_RIGHT:
						xpos = ((width - watermark.Width) - 10);
						ypos = height - watermark.Height - 10;
						break;
					case WatermarkPosition.BOTTOM_CENTER:
						xpos = (width - watermark.Width) / 2;
						ypos = height - watermark.Height - 10;
						break;
					case WatermarkPosition.BOTTOM_LEFT:
						xpos = 10;
						ypos = height - watermark.Height - 10;
						break;
				}

				picture.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);


				watermark.Dispose();
				imageAttributes.Dispose();
			}
		}

		#endregion

		public static string GetContentType(string filename)
		{
			if (filename.IndexOf('.') == -1)
				return string.Empty;
			return string.Format("image/{0}", Path.GetExtension(filename).Substring(1));
		}

		public static ImageFormat GetImageFormat(string filename)
		{
			string contentType = GetContentType(filename);
			if (_mimeMap.ContainsKey(contentType))
				return _mimeMap[contentType];

			return ImageFormat.Jpeg;
		}

		public static void SaveCutPic(string pPath, string pSavedPath, int pPartStartPointX, int pPartStartPointY, int pPartWidth, int pPartHeight, int pOrigStartPointX, int pOrigStartPointY, int imageWidth, int imageHeight)
		{
			using (Image originalImg = Image.FromFile(pPath))
			{
				if (originalImg.Width == imageWidth && originalImg.Height == imageHeight)
				{
					SaveCutPic(pPath, pSavedPath, pPartStartPointX, pPartStartPointY, pPartWidth, pPartHeight,
							pOrigStartPointX, pOrigStartPointY);
				}

				Bitmap thumimg = MakeThumbnail(originalImg, imageWidth, imageHeight);

				Bitmap partImg = new Bitmap(pPartWidth, pPartHeight);

				Graphics graphics = Graphics.FromImage(partImg);
				Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY), new Size(pPartWidth, pPartHeight));//目标位置
				Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY), new Size(pPartWidth, pPartHeight));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）

				///文字水印  
				Graphics G = Graphics.FromImage(partImg);
				G.Clear(Color.White);
				// 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
				G.InterpolationMode = InterpolationMode.HighQualityBicubic;
				// 指定高质量、低速度呈现。 
				G.SmoothingMode = SmoothingMode.HighQuality;

				graphics.DrawImage(thumimg, destRect, origRect, GraphicsUnit.Pixel);
				G.Dispose();

				originalImg.Dispose();
				if (File.Exists(pSavedPath))
				{
					File.SetAttributes(pSavedPath, FileAttributes.Normal);
					File.Delete(pSavedPath);
				}
				partImg.Save(pSavedPath, GetImageFormat(pSavedPath));

				partImg.Dispose();
				thumimg.Dispose();
			}
		}

		public static Bitmap MakeThumbnail(Image fromImg, int width, int height)
		{
			Bitmap bmp = new Bitmap(width, height);
			int ow = fromImg.Width;
			int oh = fromImg.Height;

			//新建一个画板
			Graphics g = Graphics.FromImage(bmp);

			//设置高质量插值法
			g.InterpolationMode = InterpolationMode.High;
			//设置高质量,低速度呈现平滑程度
			g.SmoothingMode = SmoothingMode.HighQuality;
			//清空画布并以透明背景色填充
			g.Clear(Color.Transparent);

			g.DrawImage(fromImg, new Rectangle(0, 0, width, height),
				new Rectangle(0, 0, ow, oh),
				GraphicsUnit.Pixel);

			return bmp;

		}

		public static void SaveCutPic(string pPath, string pSavedPath, int pPartStartPointX, int pPartStartPointY, int pPartWidth, int pPartHeight, int pOrigStartPointX, int pOrigStartPointY)
		{
			using (Image originalImg = Image.FromFile(pPath))
			{
				Bitmap partImg = new Bitmap(pPartWidth, pPartHeight, PixelFormat.Format24bppRgb);
				Graphics graphics = Graphics.FromImage(partImg);
				Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY), new Size(pPartWidth, pPartHeight));//目标位置
				Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY), new Size(pPartWidth, pPartHeight));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）

				Graphics G = Graphics.FromImage(partImg);
				//G.Clear ( Color.White );
				// 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
				G.InterpolationMode = InterpolationMode.HighQualityBicubic;
				// 指定高质量、低速度呈现。 
				G.SmoothingMode = SmoothingMode.HighQuality;

				graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);
				G.Dispose();

				originalImg.Dispose();
				if (File.Exists(pSavedPath))
				{
					File.SetAttributes(pSavedPath, FileAttributes.Normal);
					File.Delete(pSavedPath);
				}
				partImg.Save(pSavedPath, GetImageFormat(pSavedPath));
				partImg.Dispose();
			}
		}

	}
}
