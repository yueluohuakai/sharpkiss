
namespace Kiss.Utils.Imaging
{
    /// <summary>
    /// 图片转换的接口定义
    /// </summary>
    public interface IImageTransform
    {
        IImage Transform(IImage image);
    }
}