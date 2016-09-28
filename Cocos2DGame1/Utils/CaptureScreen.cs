using Microsoft.Xna.Framework;
using SharpDX.WIC;
using System;
using System.Drawing;
using System.Runtime.InteropServices;


namespace CaptureScreen
{
	/// <summary>
	/// This class shall keep all the functionality for capturing 
	/// the desktop.
	/// </summary>
	public static class CaptureScreen
	{

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

        [DllImport("user32.dll", EntryPoint = "GetClientRect")]
        public static extern bool  GetClientRect(IntPtr hWnd,  System.Drawing.Point lpRect);

		//#region Constructor
		//public CaptureScreen()
		//{
			// 
			// TODO: Add constructor logic here
			//
		//}

		//#endregion

		//#region Class Variable Declaration
			//public static
		//#endregion
        public struct SIZE
	{
		public int cx;
		public int cy;
	}
        public static System.Drawing.Point p; 
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;
        public const int SRCCOPY = 13369376;
		//#region Public Class Functions
        public static System.Drawing.Bitmap GetDesktopImage(IntPtr ip)
		{
			//In size variable we shall keep the size of the screen.
			SIZE size;
			IntPtr m_HBitmap;
			//Here we get the handle to the desktop device context.
			//IntPtr 	hDC = PlatformInvokeUSER32.GetDC(PlatformInvokeUSER32.GetDesktopWindow()); 
            IntPtr hDC = GetDC(ip); 
			//Here we make a compatible device context in memory for screen device context.
            IntPtr hMemDC = CreateCompatibleDC(hDC);
			
			//We pass SM_CXSCREEN constant to GetSystemMetrics to get the X coordinates of screen.
			size.cx = GetSystemMetrics(SM_CXSCREEN);

			//We pass SM_CYSCREEN constant to GetSystemMetrics to get the Y coordinates of screen.
			size.cy = GetSystemMetrics(SM_CYSCREEN);
			
			//We create a compatible bitmap of screen size and using screen device context.
            
            p.X = 100;
            p.Y = 100;
            //GetClientRect(hMemDC, p);

			m_HBitmap = CreateCompatibleBitmap(hDC, p.X, p.Y);

			//As m_HBitmap is IntPtr we can not check it against null. For this purspose IntPtr.Zero is used.
			if (m_HBitmap!=IntPtr.Zero)
			{
				//Here we select the compatible bitmap in memeory device context and keeps the refrence to Old bitmap.
				IntPtr hOld = (IntPtr) SelectObject(hMemDC, m_HBitmap);
				//We copy the Bitmap to the memory device context.
				BitBlt(hMemDC, 0, 0,size.cx,size.cy, hDC, 0, 0, SRCCOPY);
				//We select the old bitmap back to the memory device context.
				SelectObject(hMemDC, hOld);
				//We delete the memory device context.
				DeleteDC(hMemDC);
				//We release the screen device context.
				ReleaseDC(GetDesktopWindow(), hDC);
				//Image is created by Image bitmap handle and returned.
				return System.Drawing.Image.FromHbitmap(m_HBitmap); 
			}
			//If m_HBitmap is null retunrn null.
			return null;
		}
		//#endregion
	}
}
