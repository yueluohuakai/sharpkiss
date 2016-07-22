#region File Comment
//+-------------------------------------------------------------------+
//+ FileName:		IImage.cs
//+ File Created:   2008/03/11
//+-------------------------------------------------------------------+
//+ Purpose:        IImage������ͼƬ��Ϣ�Ľӿ�
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 2008/03/11		zhli Comment Created
//+-------------------------------------------------------------------+
#endregion

using System;
using System.Drawing;

namespace Kiss.Utils.Imaging
{
    /// <summary>
    /// ����ͼƬ��Ϣ�ӿ�
    /// </summary>
    public interface IImage : ICloneable, IDisposable
    {
        int Height { get; }

        int Width{ get; }

        Image Image
        {
            get;
            set;
        }

        Color GetPixel(int x, int y);

        void SetPixel(int x, int y, Color color);
    }
}