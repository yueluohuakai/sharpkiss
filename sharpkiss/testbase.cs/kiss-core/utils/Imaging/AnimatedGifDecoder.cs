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
        /// ��ȡ�������õ�ǰ��Image����
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
        /// ��ǰ��ͼƬ
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
        /// ��ǰ��Image��Ҫ���ŵ�ѭ����
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
        /// ��ǰ�Ƿ���Բ������,�п�����һ����̬ͼƬ��Ҳ�п�����һ����̬ͼƬ���������ڲ���������
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
        /// ��ʼ���Ŷ��������ı䶯��ԭ�����ڵ�λ��
        /// </summary>
        public void Start()
        {
            currentCycleCount = 0;
            if (frameCount > 1)
                canPlay = true;
        }

        /// <summary>
        /// ����ֹͣ,����ֹͣ�ڵ�ǰ��λ��
        /// </summary>
        public void Stop()
        {
            if (frameCount > 1)
                canPlay = false;
            timeSum = 0;
        }

        /// <summary>
        /// ��ǰ���ͼƬ����
        /// </summary>
        public int FrameCount
        {
            get
            {
                return frameCount;
            }

        }


        /// <summary>
        /// �Ƿ��ǺϷ���ͼƬ
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
        /// ��ǰ��ͼƬ�Ƿ��Ƕ���ͼƬ
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
        /// ��ͼƬ�ļ��л�ȡ����
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
        /// ����Image����
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
        /// ���ݴ��͵�Image ������AmigoImage
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
        /// AmigoImage �Ĺ��캯��
        /// </summary>
        public AnimatedGifDecoder()
        {
            ClassInitCollection();
        }
        /// <summary>
        /// AmigoImage�Ĺ��캯��������ֱ�Ӹ����ļ���ȡʵ��
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
        /// ���ָ������¼�����
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
        /// ͼƬ��Size ��*��
        /// </summary>
        public Size Size
        {
            get
            {
                return size;
            }
        }
        /// <summary>
        /// ͼƬ�ĸ߶�
        /// </summary>
        public int Height
        {
            get
            {
                return size.Height;
            }
        }
        /// <summary>
        /// ͼƬ�Ŀ��
        /// </summary>
        public int Width
        {
            get
            {
                return size.Width;
            }
        }

        /// <summary>
        /// �����һ���¼��ε�ͼƬ������CurrentImage ָ����ǰʱ��ƬӦ�ö�λ����ͼƬ
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
