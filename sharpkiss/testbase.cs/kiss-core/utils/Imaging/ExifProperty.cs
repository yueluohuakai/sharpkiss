
namespace Kiss.Utils.Imaging
{
    public enum ExifProperty
    {
        #region TIFF Rev. 6.0 Attribute Information
        // A. Tags relating to image data structure

        /// <summary>
        /// ͼ����
        /// </summary>
        ImageWidth = 256,

        /// <summary>
        /// ͼ��߶�
        /// </summary>
        ImageLength = 257,
        BitsPerSample = 258,

        /// <summary>
        /// ѹ���ȡ�
        /// </summary>
        Compression = 259,
        PhotometricInterpretation = 262,

        /// <summary>
        /// ���� �е����֧�֣��еĲ�֧��
        /// </summary>
        Orientation = 274,

        /// <summary>
        /// 
        /// </summary>
        SamplesPerPixel = 277,
        PlanarConfiguration = 284,
        YCbCrSubSampling = 530,

        /// <summary>
        /// ɫ�ඨλ
        /// </summary>
        YCbCrPositioning = 531,

        /// <summary>
        /// X����ֱ���
        /// </summary>
        XResolution = 282,

        /// <summary>
        /// Y����ֱ���
        /// </summary>
        YResolution = 283,

        /// <summary>
        /// �ֱ��ʵ�λ һ��ΪPPI
        /// </summary>
        ResolutionUnit = 296,

        // B. Tags relating to recording offset
        /// <summary>
        /// Exif��Ϣλ�ã�����Exif����Ϣ���ļ��е�д�룬��Щ�������ʾ�� 
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
        /// ���ں�ʱ��
        /// </summary>
        DateTime = 306,

        /// <summary>
        /// ͼ����������Դ. ָ����ͼ��Ĺ���
        /// </summary>
        ImageDescription = 270,

        /// <summary>
        /// ������ ָ��Ʒ��������
        /// </summary>
        Make = 271,

        /// <summary>
        /// �ͺ� ָ�豸�ͺ�
        /// </summary>
        Model = 272,

        /// <summary>
        /// ��� ��ʾ�̼�Firmware�汾
        /// </summary>
        Software = 305,
        /// <summary>
        /// ����
        /// </summary>
        Artist = 315,
        Copyright = 33432,
        #endregion

        #region Exif IFD Attribute Information
        // A. Tags Relating to Version

        /// <summary>
        /// Exif�汾
        /// </summary>
        ExifVersion = 36864,

        /// <summary>
        /// FlashPix�汾 ���������֧�֣� 
        /// </summary>
        FlashPixVersion = 40960,

        // B. Tag Relating to Image Data Characteristics
        /// <summary>
        /// ɫ��ɫ�ʿռ�
        /// </summary>
        ColorSpace = 40961,

        // C. Tags Relating to Image Configuration
        /// <summary>
        /// ͼ���죨��ָɫ����Ϸ�����
        /// </summary>
        ComponentsConfiguration = 37121,

        /// <summary>
        /// (BPP)ѹ��ʱÿ����ɫ��λ ָѹ���̶� 
        /// </summary>
        CompressedBitsPerPixel = 37122,

        /// <summary>
        /// ����������
        /// </summary>
        PixelXDimension = 40962,

        /// <summary>
        /// ����������
        /// </summary>
        PixelYDimension = 40963,

        // D. Tags Relating to User Information
        /// <summary>
        /// ���߱�ǡ�˵��
        /// </summary>
        MakerNote = 37500,
        /// <summary>
        /// ���߼�¼
        /// </summary>
        UserComment = 37510,

        // E. Tag Relating to Related File Information
        RelatedSoundFile = 40964,

        // F. Tags Relating to Date and Time

        /// <summary>
        /// ����ʱ��
        /// </summary>
        DateTimeOriginal = 36867,

        /// <summary>
        /// ���ֻ�ʱ��
        /// </summary>
        DateTimeDigitized = 36868,
        SubsecTime = 37520,
        SubsecTimeOriginal = 37521,
        SubsecTimeDigitized = 37522,

        // G. Tags Relating to Picture-Taking Conditions

        /// <summary>
        /// �ع�ʱ�� �������ٶ�
        /// </summary>
        ExposureTime = 33434,

        /// <summary>
        /// ��Ȧϵ��
        /// </summary>
        FNumber = 33437,

        /// <summary>
        /// �ع���� ָ����ʽ�Զ��ع�����ã��������ͬ,������Sutter Priority���������ȣ���Aperture Priority���������ȣ��ȵȡ�
        /// </summary>
        ExposureProgram = 34850,

        /// <summary>
        /// 
        /// </summary>
        SpectralSensitivity = 34852,

        /// <summary>
        /// �й��
        /// </summary>
        ISOSpeedRatings = 34855,
        OECF = 34856,
        ShutterSpeedValue = 37377,
        ApertureValue = 37378,
        BrightnessValue = 37379,

        /// <summary>
        /// �عⲹ����
        /// </summary>
        ExposureBiasValue = 37380,

        /// <summary>
        /// ����Ȧ
        /// </summary>
        MaxApertureValue = 37381,
        SubjectDistance = 37382,

        /// <summary>
        /// ��ⷽʽ�� ƽ��ʽ��⡢�����ص��⡢����ȡ�
        /// </summary>
        MeteringMode = 37383,

        /// <summary>
        /// ��Դ ָ��ƽ������
        /// </summary>
        LightSource = 37384,

        /// <summary>
        /// �Ƿ�ʹ������ơ�
        /// </summary>
        Flash = 37385,

        /// <summary>
        /// ���࣬һ����ʾ��ͷ�����࣬��Щ������Զ���һ��ϵ�����Ӷ���ʾ�൱��35mm����Ľ���
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
        /// Դ�ļ�
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
        /// IFDͨ������չ���ָ�� ��TIFF�ļ���أ����庬�岻��
        /// </summary>
        InteroperabilityIndex = 20545,

        /// <summary>
        /// IFDͨ������չ���ָ�� ��TIFF�ļ���أ����庬�岻��
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
