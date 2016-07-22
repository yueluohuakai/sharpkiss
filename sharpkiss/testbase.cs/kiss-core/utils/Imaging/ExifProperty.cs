
namespace Kiss.Utils.Imaging
{
    public enum ExifProperty
    {
        #region TIFF Rev. 6.0 Attribute Information
        // A. Tags relating to image data structure

        /// <summary>
        /// 图像宽度
        /// </summary>
        ImageWidth = 256,

        /// <summary>
        /// 图像高度
        /// </summary>
        ImageLength = 257,
        BitsPerSample = 258,

        /// <summary>
        /// 压缩比。
        /// </summary>
        Compression = 259,
        PhotometricInterpretation = 262,

        /// <summary>
        /// 方向 有的相机支持，有的不支持
        /// </summary>
        Orientation = 274,

        /// <summary>
        /// 
        /// </summary>
        SamplesPerPixel = 277,
        PlanarConfiguration = 284,
        YCbCrSubSampling = 530,

        /// <summary>
        /// 色相定位
        /// </summary>
        YCbCrPositioning = 531,

        /// <summary>
        /// X方向分辨率
        /// </summary>
        XResolution = 282,

        /// <summary>
        /// Y方向分辨率
        /// </summary>
        YResolution = 283,

        /// <summary>
        /// 分辨率单位 一般为PPI
        /// </summary>
        ResolutionUnit = 296,

        // B. Tags relating to recording offset
        /// <summary>
        /// Exif信息位置，定义Exif在信息在文件中的写入，有些软件不显示。 
        /// </summary>
        StripOffsets = 273,
        RowsPerStrip = 278,
        StripByteCounts = 279,
        JPEGInterchangeFormat = 513,
        JPEGInterchangeFormatLength = 514,

        // C. Tags relating to image data characteristics
        TransferFunction = 301,
        WhitePoint = 318,
        PrimaryChromaticities = 319,
        YCbCrCoefficients = 529,
        ReferenceBlackWhite = 532,

        /// <summary>
        /// 日期和时间
        /// </summary>
        DateTime = 306,

        /// <summary>
        /// 图像描述、来源. 指生成图像的工具
        /// </summary>
        ImageDescription = 270,

        /// <summary>
        /// 生产者 指产品生产厂家
        /// </summary>
        Make = 271,

        /// <summary>
        /// 型号 指设备型号
        /// </summary>
        Model = 272,

        /// <summary>
        /// 软件 显示固件Firmware版本
        /// </summary>
        Software = 305,
        /// <summary>
        /// 作者
        /// </summary>
        Artist = 315,
        Copyright = 33432,
        #endregion

        #region Exif IFD Attribute Information
        // A. Tags Relating to Version

        /// <summary>
        /// Exif版本
        /// </summary>
        ExifVersion = 36864,

        /// <summary>
        /// FlashPix版本 （个别机型支持） 
        /// </summary>
        FlashPixVersion = 40960,

        // B. Tag Relating to Image Data Characteristics
        /// <summary>
        /// 色域、色彩空间
        /// </summary>
        ColorSpace = 40961,

        // C. Tags Relating to Image Configuration
        /// <summary>
        /// 图像构造（多指色彩组合方案）
        /// </summary>
        ComponentsConfiguration = 37121,

        /// <summary>
        /// (BPP)压缩时每像素色彩位 指压缩程度 
        /// </summary>
        CompressedBitsPerPixel = 37122,

        /// <summary>
        /// 横向像素数
        /// </summary>
        PixelXDimension = 40962,

        /// <summary>
        /// 纵向像素数
        /// </summary>
        PixelYDimension = 40963,

        // D. Tags Relating to User Information
        /// <summary>
        /// 作者标记、说明
        /// </summary>
        MakerNote = 37500,
        /// <summary>
        /// 作者记录
        /// </summary>
        UserComment = 37510,

        // E. Tag Relating to Related File Information
        RelatedSoundFile = 40964,

        // F. Tags Relating to Date and Time

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTimeOriginal = 36867,

        /// <summary>
        /// 数字化时间
        /// </summary>
        DateTimeDigitized = 36868,
        SubsecTime = 37520,
        SubsecTimeOriginal = 37521,
        SubsecTimeDigitized = 37522,

        // G. Tags Relating to Picture-Taking Conditions

        /// <summary>
        /// 曝光时间 即快门速度
        /// </summary>
        ExposureTime = 33434,

        /// <summary>
        /// 光圈系数
        /// </summary>
        FNumber = 33437,

        /// <summary>
        /// 曝光程序 指程序式自动曝光的设置，各相机不同,可能是Sutter Priority（快门优先）、Aperture Priority（快门优先）等等。
        /// </summary>
        ExposureProgram = 34850,

        /// <summary>
        /// 
        /// </summary>
        SpectralSensitivity = 34852,

        /// <summary>
        /// 感光度
        /// </summary>
        ISOSpeedRatings = 34855,
        OECF = 34856,
        ShutterSpeedValue = 37377,
        ApertureValue = 37378,
        BrightnessValue = 37379,

        /// <summary>
        /// 曝光补偿。
        /// </summary>
        ExposureBiasValue = 37380,

        /// <summary>
        /// 最大光圈
        /// </summary>
        MaxApertureValue = 37381,
        SubjectDistance = 37382,

        /// <summary>
        /// 测光方式， 平均式测光、中央重点测光、点测光等。
        /// </summary>
        MeteringMode = 37383,

        /// <summary>
        /// 光源 指白平衡设置
        /// </summary>
        LightSource = 37384,

        /// <summary>
        /// 是否使用闪光灯。
        /// </summary>
        Flash = 37385,

        /// <summary>
        /// 焦距，一般显示镜头物理焦距，有些软件可以定义一个系数，从而显示相当于35mm相机的焦距
        /// </summary>
        FocalLength = 37386,
        SubjectArea = 37396,
        FlashEnergy = 41483,
        SpatialFrequencyResponse = 41484,
        FocalPlaneXResolution = 41486,
        FocalPlaneYResolution = 41487,
        FocalPlaneResolutionUnit = 41488,
        SubjectLocation = 41492,
        ExposureIndex = 41493,
        SensingMethod = 41495,

        /// <summary>
        /// 源文件
        /// </summary>
        FileSource = 41728,
        SceneType = 41729,
        CFAPattern = 41730,
        CustomRendered = 41985,
        ExposureMode = 41986,
        WhiteBalance = 41987,
        DigitalZoomRatio = 41988,
        FocalLengthIn35mmFilm = 41989,
        SceneCaptureType = 41990,
        GainControl = 41991,
        Contrast = 41992,
        Saturation = 41993,
        Sharpness = 41994,
        DeviceSettingDescription = 41995,
        SubjectDistanceRange = 41996,

        // H. Other Tags
        ImageUniqueID = 42016,

        // H.Interoperability IFD Attribute Information
        /// <summary>
        /// IFD通用性扩展项定义指针 和TIFF文件相关，具体含义不详
        /// </summary>
        InteroperabilityIndex = 20545,

        /// <summary>
        /// IFD通用性扩展项定义指针 和TIFF文件相关，具体含义不详
        /// </summary>
        InteroperabilityVersion = 20546,

        RelatedImageWidth = 4097,
        RelatedImageLength = 4098,
        #endregion

        #region Windows Attributes

        WindowsTitle = 0x9c9b,
        WindowsComments = 0x9C9C,
        WindowsAuthor = 0x9C9D,
        WindowsKeywords = 0x9C9E,
        WindowsSubject = 0x9C9F,

        #endregion

    }
}
