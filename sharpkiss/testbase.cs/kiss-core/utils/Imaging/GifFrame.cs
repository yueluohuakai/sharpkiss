using System.Drawing;

namespace Kiss.Utils.Imaging
{
    public class GifFrame
    {
        private Image image;
        private int delay;

        public GifFrame(Image im, int del)
        {
            image = im;
            delay = del;
        }

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

        public int Delay
        {
            get
            {
                return delay;
            }
            set
            {
                delay = value;
            }
        }
    }
}
