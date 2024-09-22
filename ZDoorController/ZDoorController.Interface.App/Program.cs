using Iot.Device.Media;

#if DEBUG
Console.WriteLine("Press any key for debug");
Console.ReadKey();
#endif
Console.WriteLine("Hello, World!");

VideoConnectionSettings settings = new VideoConnectionSettings(busId: 0, captureSize: (2592, 1944), pixelFormat: VideoPixelFormat.JPEG);

using VideoDevice device = VideoDevice.Create(settings);
device.Capture("/home/pi/Pictures/capture.jpg");

//IEnumerable<VideoPixelFormat> formats = device.GetSupportedPixelFormats();

//foreach (var format in formats)
//{
//    Console.WriteLine($"Pixel Format {format}");
//    IEnumerable<Resolution> resolutions = device.GetPixelFormatResolutions(format);
//    if (resolutions is not null)
//    {
//        foreach (var res in resolutions)
//        {
//            Console.WriteLine($"   min res: {res.MinWidth} x {res.MinHeight} ");
//            Console.WriteLine($"   max res: {res.MaxWidth} x {res.MaxHeight} ");
//        }
//    }
//}