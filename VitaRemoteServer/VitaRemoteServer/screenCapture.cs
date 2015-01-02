using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace VitaRemoteServer
{
    static class screenCapture
    {
        #region "Win32 Api"

        public const int SRCCOPY = 13369376;

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern IntPtr DeleteObject(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int abc);

        [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
        public static extern IntPtr GetWindowDC(Int32 ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsCallbackHandler lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsCallbackHandler(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);

        [DllImport("user32")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        //Public Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Long

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern uint SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("gdi32.dll")]
        static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
            int nWidthDest, int nHeightDest,
            IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
            CopyPixelOperation dwRop);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        private const int WM_PAINT = 0x000F;

        private enum StretchMode
        {
            STRETCH_ANDSCANS = 1,
            STRETCH_ORSCANS = 2,
            STRETCH_DELETESCANS = 3,
            STRETCH_HALFTONE = 4,
        }

        [StructLayout(LayoutKind.Sequential)]

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }


#endregion

        public static int _x;
        public static int _y;
        private static int _area;
        private static int _resolution;
        private static int _resWidth;
        private static int _resHeight;
        private static int _areaWidth;
        private static int _areaHeight;
        private static int _quality;
        //private static Bitmap _prevBmp;
        //private static int qSwitch = 0;
        private static bool _showCursor = true;
        #region "Properties"
        public static int Quality
        {
            get { return _quality; }
            set { _quality = value; }
        }

        public static bool ShowCursor
        {
            get { return _showCursor; }
            set { _showCursor = value; }
        }
        public static void init()
        {
            X = 0;
            Y = 0;
            Area = 100;
            Resolution  = 100;
            Quality = 100;
        }
        public static int ResWidth
        {
            get {
                //_resWidth = (int)((Screen.PrimaryScreen.Bounds.Width * _resolution) / 100);
                return _resWidth; 
            }
        }

        public static int ResHeight
        {
            get {
                //_resHeight = (int)((Screen.PrimaryScreen.Bounds.Height * _resolution) / 100);
                return _resHeight; 
            }
        }

        public static int Resolution
        {
            get { return _resolution; }
            set {
                    //_resolution = value;
                    //int width = 940;
                    //int height = 544;
                    //(int)((width * _resolution) / 100);
                    //(int)((height * _resolution) / 100);
                if (value == 2)
                {
                    _resWidth = 240; _resHeight = 270;
                }
                else
                {
                    _resWidth = 200; _resHeight = 200;
                }
                _resolution = value;
            }
        }
        public static int X
        {
            get { return _x; }
            set{ //jonathan comented this lines to test drag screen peformance not working well must find better way
               if ((value + _areaWidth) <= System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width && value >= 0)
                {
					
                    _x = value;
                }
            }
        }

        public static int Y
        {
            get { return _y; }
            set {
               if ((value + _areaHeight) <= System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height && value >= 0)
                {
                    _y = value; 
               }
            }
        }

        public static int Area
        {
            get { return _area; }
            set {
                //if (value <= 100 && value >= 10)
                //{
                    switch (value)
                    {
                        case 1:
                            _areaWidth = 400; _areaHeight = 200;
                            break;
                        case 2:
                            _areaWidth = 480; _areaHeight = 270;
                            break;
                        case 3:
                            _areaWidth = 640; _areaHeight = 480;
                            break;
                        case 4:
                            _areaWidth = 720; _areaHeight = 576;
                            break;
                        case 5:
                            _areaWidth = 800; _areaHeight = 600;
                            break;
						case 6:
                            _areaWidth = 1200; _areaHeight = 800; //jonathan added new resolution
                            break;
                    }

                    _area = value;
                    X = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2) - (_areaWidth / 2);
                    Y = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2) - (_areaHeight / 2);
                    //_areaWidth = (int)((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * _area) / 100);
                    //_areaHeight = (int)((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * _area) / 100);

                    
                //}
            }
        }

        public static int AreaWidth
        { get { return _areaWidth; } }
        public static int AreaHeight
        { get { return _areaHeight; } }

        #endregion

       
        public static MemoryStream GetDesktopImage(IntPtr hwnd)
        {
            IntPtr hBitmap;
            IntPtr hDC = GetDC(hwnd);
            IntPtr hMemDC = CreateCompatibleDC(hDC);
            
            RECT windowRect = new RECT();

            GetWindowRect(hwnd,ref windowRect);

            int width = windowRect.right - windowRect.left;

            int height = windowRect.bottom - windowRect.top;

            hBitmap = CreateCompatibleBitmap(hDC,_resWidth, _resHeight);

            //As hBitmap is IntPtr we can not check it against null. For this purspose IntPtr.Zero is used.
            if (hBitmap != IntPtr.Zero)
            {
                IntPtr hOld = (IntPtr)SelectObject(hMemDC, hBitmap);
                StretchBlt(hMemDC, 0, 0, _resWidth, _resHeight, hDC, _x, _y, _areaWidth, _areaHeight, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
                //BitBlt(hMemDC, 0, 0, _areaWidth, _areaHeight, hDC, _x, _y, SRCCOPY);

                

                SelectObject(hMemDC, hOld);
                DeleteDC(hMemDC);
                ReleaseDC(hwnd, hDC);

                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
				


                DeleteObject(hBitmap);
                return bmp.Compress(_quality);
				
            }

            return null;
        }


        public static int ScreenShot(ref MemoryStream ms1, ref MemoryStream ms2)
        {
            Size size = new Size(_areaWidth , _areaHeight);

            Bitmap srcImage = new Bitmap(size.Width, size.Height,System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
			
            Graphics srcGraphics = Graphics.FromImage(srcImage);

            srcGraphics.CopyFromScreen(_x, _y, 0, 0, size);


            if (_showCursor)
            {
                Size curSize = new Size(32, 32);
                Cursors.Default.Draw(srcGraphics, new Rectangle(Cursor.Position.X - _x, Cursor.Position.Y - _y, 32, 32));
            }

            //if (srcImage.CompareMemCmp(_prevBmp))
                //return null;
            //_prevBmp = srcImage;

            Bitmap scaledScr1 = new Bitmap(_resWidth , _resHeight);
            Bitmap scaledScr2 = new Bitmap(_resWidth, _resHeight);
            
            srcGraphics = Graphics.FromImage(scaledScr1);
            Rectangle src1 = new Rectangle(0, 0, size.Width / 2, size.Height);
            Rectangle src2 = new Rectangle(size.Width / 2, 0, size.Width / 2, size.Height);
            Rectangle dst = new Rectangle(0, 0, _resWidth, _resHeight);
            srcGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            srcGraphics.DrawImage(srcImage, dst, src1, GraphicsUnit.Pixel);

            srcGraphics = Graphics.FromImage(scaledScr2);
            srcGraphics.DrawImage(srcImage, dst, src2, GraphicsUnit.Pixel);
			
			
                ms1 = scaledScr1.Compress(_quality);
                ms2 = scaledScr2.Compress(_quality);
			//
			//Console.WriteLine("ms1.lengh = "+ms1.Length);
			//Console.WriteLine("ms2.lengh = "+ms2.Length);
                

            return 0;
        }


        public static bool CompareMemCmp(this Bitmap b1, Bitmap b2)
        {
            if (b2 == null) return false;
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format16bppRgb555);//jonathan changed image frtom 32bits to 16 bits for an improved performance of around 10% and no noticable image downgrade
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format16bppRgb555);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }

        public static MemoryStream Compress(this Image image, Int32 quality) //jonathan compress image
        {
            if (image == null)
                return null;
			
		
		
				// jonathan resise image it works but image is already at 200 pixel per size
		/*	
			Console.WriteLine("image width = "+image.Width.ToString());
			Console.WriteLine("image height = "+image.Height.ToString());
			Int32 scaledWidth = Convert.ToInt32(image.Width * 0.4f);
  			Int32 scaledHeight = Convert.ToInt32(image.Height * 0.4f);
			
			Size size=new Size(scaledWidth , scaledHeight);
  			var resizedImage = new Bitmap(size.Width, size.Height, image.PixelFormat);
 			 resizedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

 			 using (var g = Graphics.FromImage(resizedImage))
 			 {
 				   var location = new Point(0, 0);
 				   g.InterpolationMode = InterpolationMode.HighQualityBicubic;
  				  g.DrawImage(image, new Rectangle(location, size), new Rectangle(location, image.Size), GraphicsUnit.Pixel);
			  }
				
				image=resizedImage;
			//
			
		*/	
			
			
			
            quality = Math.Max(0, Math.Min(100, quality));

            using (var encoderParameters = new EncoderParameters(1))
            {
                var imageCodecInfo = ImageCodecInfo.GetImageEncoders().First(encoder => String.Compare(encoder.MimeType, "image/jpeg", StringComparison.OrdinalIgnoreCase) == 0);
                var memoryStream = new MemoryStream();

                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Convert.ToInt64(quality));
				
				
                image.Save(memoryStream, imageCodecInfo, encoderParameters);
                image.Save(memoryStream, ImageFormat.Jpeg);
				
				
                return memoryStream;
            }
        }


        public static int GetAllWindows(bool minimized, bool specialCapturring)
        {
            EnumWindowsCallbackHandler callback = new EnumWindowsCallbackHandler(EnumWindowsCallback);
            EnumWindows(callback, IntPtr.Zero);

            return 0;
        }

        private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            return false;
        //    bool specialCapturing = false;

        //    if (hWnd == IntPtr.Zero) return false;

        //    if (!IsWindowVisible(hWnd)) return true;

        //    if (!countMinimizedWindows)
        //    {
        //        if (IsIconic(hWnd)) return true;
        //    }
        //    else if (IsIconic(hWnd) && useSpecialCapturing) specialCapturing = true;

        //    if (GetWindowText(hWnd) == PROGRAMMANAGER) return true;

        //    windowSnaps.Add(new WindowSnap(hWnd, specialCapturing));

        //    return true;
        }

    }
}
