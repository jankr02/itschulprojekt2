using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MesseauftrittDatenerfassung
{

    namespace MesseauftrittDatenerfassung
    {
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public class CameraAPI
        {
            private VideoCaptureDevice videoSource;
            private FilterInfoCollection videoDevices;
            private Bitmap currentFrame;

            public CameraAPI() // Constructor
            {
                try
                {
                    // Enumerate video devices
                    videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                    if (videoDevices.Count == 0)
                        throw new Exception("No video devices found");

                    // Create video source
                    videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

                    // Start the video source
                    videoSource.Start();
                }
                catch (Exception ex)
                {
                    Task.Run(() => MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly));
                }
            }

            private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
            {
                // Get new frame
                currentFrame?.Dispose(); // Dispose previous frame if it exists
                currentFrame = (Bitmap)eventArgs.Frame.Clone();
            }

            public byte[] CaptureImage()
            {
                try
                {
                    if (currentFrame != null)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            // Save to memory stream in JPEG format
                            currentFrame.Save(memoryStream, ImageFormat.Jpeg);

                            // Get bytes from stream
                            return memoryStream.ToArray();
                        }
                    }
                }
                catch (Exception)
                {
                }
                return null;
            }

            public void StopCamera()
            {
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                    videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
                    currentFrame?.Dispose(); // Dispose the frame when stopping the camera
                }
            }
        }
    }
}
