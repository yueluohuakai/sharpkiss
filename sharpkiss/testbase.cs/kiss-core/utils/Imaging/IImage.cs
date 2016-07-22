#region File Comment
//+-------------------------------------------------------------------+
//+ FileName:		IImage.cs
//+ File Created:   2008/03/11
//+-------------------------------------------------------------------+
//+ Purpose:        IImage定义了图片信息的接口
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
    /// 定义图片信息接口
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