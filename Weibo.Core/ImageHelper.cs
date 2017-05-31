using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;
namespace Weibo.Core
{
    public class ImageHelper
    {
        /// <summary>
        ///  Resize图片 
        /// </summary>
        /// <param name="bmp">原始Bitmap </param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        /// <param name="Mode">保留着，暂时未用</param>
        /// <returns>处理以后的图片</returns>

        public static Bitmap ResizeImage(Bitmap bmp, int newW, int newH, int Mode)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量 
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 剪裁 -- 用GDI+ 
        /// </summary>
        /// <param name="b">原始Bitmap</param>
        /// <param name="StartX">开始坐标X</param>
        /// <param name="StartY">开始坐标Y</param>
        /// <param name="iWidth">宽度</param>
        /// <param name="iHeight">高度</param>
        /// <returns>剪裁后的Bitmap</returns>
        public static Bitmap Cut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }
            int w = b.Width;
            int h = b.Height;
            if (StartX >= w || StartY >= h)
            {
                return null;
            }
            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }
            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }
            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();
                return bmpOut;
            }
            catch
            {
                return null;
            }
        }

        public static Image WaterMark(Image image, string text, Font font, Brush backgroundColor, Brush fontColor, ImagePosition position = ImagePosition.RigthBottom)
        {
            Bitmap bit = null;
            Graphics g = null;
            if (image == null)
            {
                return null;
            }
            try
            {
                bit = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
                bit.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                g = Graphics.FromImage(bit);
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                SizeF size = g.MeasureString(text, font);
                float waterWidth = size.Width + 10;
                float waterHeight = size.Height + 8;
                float xPosOfWaterMark;
                float yPosOfWaterMark;
                switch (position)
                {
                    case ImagePosition.BottomMiddle:
                        xPosOfWaterMark = bit.Width / 2;
                        yPosOfWaterMark = bit.Height - waterHeight - 10;
                        break;
                    case ImagePosition.Center:
                        xPosOfWaterMark = bit.Width / 2;
                        yPosOfWaterMark = bit.Height / 2;
                        break;
                    case ImagePosition.LeftBottom:
                        xPosOfWaterMark = waterWidth;
                        yPosOfWaterMark = bit.Height - waterHeight - 10;
                        break;
                    case ImagePosition.LeftTop:
                        xPosOfWaterMark = waterWidth / 2;
                        yPosOfWaterMark = waterHeight / 2;
                        break;
                    case ImagePosition.RightTop:
                        xPosOfWaterMark = bit.Width - waterWidth - 10;
                        yPosOfWaterMark = waterHeight;
                        break;
                    case ImagePosition.RigthBottom:
                        xPosOfWaterMark = bit.Width - waterWidth - 10;
                        yPosOfWaterMark = bit.Height - waterHeight - 10;
                        break;
                    case ImagePosition.TopMiddle:
                        xPosOfWaterMark = bit.Width / 2;
                        yPosOfWaterMark = waterWidth;
                        break;
                    default:
                        xPosOfWaterMark = waterWidth;
                        yPosOfWaterMark = bit.Height - waterHeight - 10;
                        break;
                }
                g.FillRoundedRectangle(backgroundColor, new Rectangle((int)xPosOfWaterMark, (int)yPosOfWaterMark, (int)waterWidth, (int)waterHeight), (int)waterHeight / 2);
                g.DrawString(text, font, fontColor, xPosOfWaterMark + 5, yPosOfWaterMark + 4);
                return bit;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }
        }
    }
    public enum ImagePosition
    {
        LeftTop,
        LeftBottom,
        RightTop,
        RigthBottom,
        TopMiddle,
        BottomMiddle,
        Center
    }
    public static class RoundedRectangle
    {
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

        internal static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            // top left arc  
            path.AddArc(arc, 180, 90);
            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
