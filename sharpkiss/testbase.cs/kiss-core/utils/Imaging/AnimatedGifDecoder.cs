using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Kiss.Utils.Imaging
{
    public class AnimatedGifDecoder
    {
        private Image image;
        private List<Bitmap> nodes;
        private FrameDimension imageDimension;
        private bool canPlay;
        private int frameCount;
        private int currenTime;
        private ulong timeSum;
        private int frameFrequency;
        private int allFrameTime;
        private Size size;
        private byte[] time;
        private int cycleCount;
        private int currentCycleCount;

        /// <summary>
        /// 获取或者设置当前的Image对象
        /// </summary>
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }
        /// <summary>
        /// 当前的图片
        /// </summary>
        public Image CurrentImage
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }

        /// <summary>
        /// 当前的Image需要播放的循环数
        /// </summary>
        public int CycleCount
        {
            get
            {
                return cycleCount;
            }
            set
            {
                cycleCount = value;
            }
        }
        /// <summary>
        /// 当前是否可以播放这个,有可能是一个静态图片，也有可能是一个动态图片，但是现在不允许播放了
        /// </summary>
        public bool CanPlay
        {
            get
            {
                if (!canPlay)
                    return false;

                if (cycleCount == 0 || currentCycleCount < cycleCount)
                {
                    return true;
                }
                if (currentCycleCount >= cycleCount)
                {
                    timeSum = 0;
                    return false;
                }
                return canPlay;
            }
        }
        /// <summary>
        /// 开始播放动画，不改变动画原先所在的位置
        /// </summary>
        public void Start()
        {
            currentCycleCount = 0;
            if (frameCount > 1)
                canPlay = true;
        }

        /// <summary>
        /// 立即停止,动画停止在当前的位置
        /// </summary>
        public void Stop()
        {
            if (frameCount > 1)
                canPlay = false;
            timeSum = 0;
        }

        /// <summary>
        /// 当前这个图片的桢
        /// </summary>
        public int FrameCount
        {
            get
            {
                return frameCount;
            }

        }


        /// <summary>
        /// 是否是合法的图片
        /// </summary>
        /// <returns></returns>
        public bool IsValidImage()
        {
            if (image != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 当前的图片是否是动画图片
        /// </summary>
        public bool IsAnimated
        {
            get
            {
                if (frameCount > 1)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 从图片文件中获取数据
        /// </summary>
        /// <param name="FilePath"></param>
        public void FromFile(string FilePath)
        {
            try // Check If invalid horision 2006-3-23
            {
                image = Image.FromFile(FilePath);
            }
            catch
            {
                image = null;
                return;
            }

            ParseImage();

        }
        /// <summary>
        /// 解析Image对象
        /// </summary>
        private void ParseImage()
        {
            if (image.FrameDimensionsList.GetLength(0) != 0)
            {
                Debug.Assert(image.FrameDimensionsList.GetLength(0) != 0);
            }

            imageDimension = new System.Drawing.Imaging.FrameDimension(image.FrameDimensionsList[0]);
            frameCount = image.GetFrameCount(imageDimension);
            if (FrameCount > 1)
            {
                canPlay = true;
                frameFrequency = image.PropertyItems[0].Value.GetLength(0) / FrameCount;
                time = image.PropertyItems[0].Value;
            }
            else
            {
                canPlay = false;

            }

            for (int i = 0; i < FrameCount; i++)
            {
                allFrameTime = allFrameTime + GetFrameTime(i);
            }
            currenTime = 0;
            size = image.Size;
            for (int i = 0; i < FrameCount; i++)
            {
                MemoryStream s = new MemoryStream();
                image.SelectActiveFrame(imageDimension, i);

                Bitmap bitmap = new Bitmap(image);
                nodes.Add(bitmap);
                s.Dispose();
            }
        }

        /// <summary>
        /// 根据传送的Image 对象构造AmigoImage
        /// </summary>
        /// <param name="image"></param>
        public AnimatedGifDecoder(Image image)
        {
            ClassInitCollection();
            this.image = image;
            if (this.image == null)
            {
                return;
            }
            ParseImage();

        }

        /// <summary>
        /// AmigoImage 的构造函数
        /// </summary>
        public AnimatedGifDecoder()
        {
            ClassInitCollection();
        }
        /// <summary>
        /// AmigoImage的构造函数，可以直接根据文件获取实例
        /// </summary>
        /// <param name="FilePath"></param>
        public AnimatedGifDecoder(string FilePath)
        {
            ClassInitCollection();
            FromFile(FilePath);
        }


        private void ClassInitCollection()
        {
            nodes = new List<Bitmap>();
            frameFrequency = 0;

        }
        /// <summary>
        /// 获得指定桢的事件长度
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        protected int GetFrameTime(int frame)
        {
            Debug.Assert(frame <= FrameCount);
            if (frame > FrameCount)
            {
                return -1;
            }
            if (FrameCount == 1)
                return 0;
            byte[] Time = time;

            int iTime = Time[frame * frameFrequency + 3] * (int)Math.Pow(10, 4) +
                Time[frame * frameFrequency + 2] * (int)Math.Pow(10, 3) +
                Time[frame * frameFrequency + 1] * (int)Math.Pow(10, 2) +
                Time[frame * frameFrequency + 0] * (int)Math.Pow(10, 1);
            return iTime;

        }

        /// <summary>
        /// 图片的Size 长*宽
        /// </summary>
        public Size Size
        {
            get
            {
                return size;
            }
        }
        /// <summary>
        /// 图片的高度
        /// </summary>
        public int Height
        {
            get
            {
                return size.Height;
            }
        }
        /// <summary>
        /// 图片的宽度
        /// </summary>
        public int Width
        {
            get
            {
                return size.Width;
            }
        }

        /// <summary>
        /// 获得下一个事件段的图片，并将CurrentImage 指到当前时间片应该定位到的图片
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public virtual Image NextFrame(int interval)
        {

            if (!CanPlay) return image;
            timeSum += (ulong)interval;
            currentCycleCount = (int)(timeSum / (ulong)allFrameTime);
            currenTime = (currenTime + interval) % allFrameTime;


            int iTemptime = 0;
            int frame = 0;
            for (int i = 0; i < FrameCount; i++)
            {
                iTemptime = iTemptime + GetFrameTime(i);
                if (iTemptime >= currenTime)
                {
                    frame = i;
                    break;
                }
            }
            // _image.SelectActiveFrame(_ImageDimension, frame);
            image = nodes[frame];
            return image;
        }

        public GifFrame GetFrame(int frame)
        {
            Image frameImage = nodes[frame];
            int delay = GetFrameTime(frame);
            return new GifFrame(frameImage, delay);
        }
    }
}
