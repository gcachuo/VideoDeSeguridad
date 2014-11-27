using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Kinect;

namespace SeguridadPorVideo
{
    static class Kinect
    {
        private static KinectSensor _sensor;
        private static Skeleton[] _skeletons;

        public static void InicializaSensor()
        {
            _sensor = KinectSensor.KinectSensors[0];
            _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            _sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            _sensor.SkeletonStream.Enable();
            _sensor.AllFramesReady += FramesReady;
            _sensor.Start();
        }

        static Bitmap imageToBitmap(ColorImageFrame image)
        {
            var pixeldata = new byte[image.PixelDataLength];
            image.CopyPixelDataTo(pixeldata);
            var bmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppRgb);
            var bmapdata = bmap.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly,
                bmap.PixelFormat);
            var ptr = bmapdata.Scan0;
            Marshal.Copy(pixeldata, 0, ptr, image.PixelDataLength);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }

        static void FramesReady(object sender, AllFramesReadyEventArgs e)
        {
            var vFrame = e.OpenColorImageFrame();
            if (vFrame == null) return;
            var bmap = imageToBitmap(vFrame);
            Form1.imgVideo.Image = bmap;
            var sFrame = e.OpenSkeletonFrame();
            if (sFrame == null) return;
            _skeletons = new Skeleton[sFrame.SkeletonArrayLength];
            sFrame.CopySkeletonDataTo(_skeletons);
            foreach (var unSkeleton in _skeletons)
            {
                if (unSkeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    var sloc = unSkeleton.Joints[JointType.HandRight].Position;
                    // ReSharper disable once CSharpWarnings::CS0618
                    var cloc = _sensor.MapSkeletonPointToColor(sloc,
                        ColorImageFormat.RgbResolution640x480Fps30);
                    //markAtPoint(cloc, bmap);
                    sloc = unSkeleton.Joints[JointType.HandLeft].Position;
                    // ReSharper disable once CSharpWarnings::CS0618
                    cloc = _sensor.MapSkeletonPointToColor(sloc, ColorImageFormat.RgbResolution640x480Fps30);
                    //markAtPoint(cloc, bmap);
                    Form1.imgVideo.Image = bmap;
                }
                if (unSkeleton.TrackingState != SkeletonTrackingState.PositionOnly) continue;
                if (unSkeleton.Joints[JointType.ShoulderRight].TrackingState == JointTrackingState.Tracked) ;

            }

        }
       
    }
}
