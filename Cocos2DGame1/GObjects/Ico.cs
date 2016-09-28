using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;

namespace VenLight
{
    public class Ico
    {
        public  int    centrX = 0;                                                        //середина по иксу
        public  int    centrY = 0;                                                        //середина по игреку  
        public  int    mindev;                                                            //отклонение в пикселях
        public  int    maxdev;                                                            //отклонение в пикселях 
        public  string textstr = "";                                                      //надпись
        public  int    timeFrame = 50;                                                    //интервал между обработкой в милисекундах
        public  int    eltime = 0;                                                        //количество милисекунд с момента последнего обнуления
        public  int    count = 0;                                                         //количество кадров
        public  int    tekcount = 0;                                                      //текущий кадр
        protected int  DXY=5;                                                             //переменная приращения для девиации
        public  bool   enabledAnimation = true;                                           //разрешает смену кадров
        public  bool   visible = true;                                                    //разрешает отображение
        public  Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;//Цвет текстуры
        public  SpriteEffects effect = SpriteEffects.None;                                //эффекты для спрайтов
        public  Texture2D[] sprite;                                                       //массив кадров       
        protected Microsoft.Xna.Framework.Rectangle Rect;                                 //текущие координаты иконки
        protected Microsoft.Xna.Framework.Rectangle RectOrig;                             //оригинальные координаты иконки
        public float Rot = 0;                                                             //угол поворота     
        protected int AlX = 0;
        protected int AlY = 0;

        public Ico(string path, GraphicsDevice GD)
        {
            Rect = new Microsoft.Xna.Framework.Rectangle(50,50,80,80);
            RectOrig = Rect;
            if (path != null)
            {
                int a=LoadFile(path, GD);
                if (a <= 0) MessageBox.Show("Ошибка загрузки файла "+path+" код ошибки - "+a.ToString());
            }
        }

        public void SetRect(Microsoft.Xna.Framework.Rectangle r)
        {
            Rect = r;
            RectOrig = r;
            mindev = (int)(0.1*RectOrig.Width);
            maxdev = (int)(2 * RectOrig.Width);
        }

        public void SetX(int x)
        {
            Rect.X = x;
            RectOrig.X = x;
        }
        public void SetY(int y)
        {
            Rect.Y = y;
            RectOrig.Y = y;
        }
        public Microsoft.Xna.Framework.Rectangle GetRect()
        {
            return RectOrig;
        }

        public void SetAlXY(int x, int y)
        {
            AlX = x;
            AlY = y;
        }

        public void Draw(SpriteBatch SP)
        {
            if (!visible) return;
            Microsoft.Xna.Framework.Rectangle R = Rect;
            R.X = R.X + R.Width / 2;
            R.Y = R.Y + R.Height / 2;
            SP.Begin();
            SP.Draw(sprite[tekcount], R, new Microsoft.Xna.Framework.Rectangle(0, 0, centrX * 2, centrY * 2), color, /*povorot*/ Rot, new Vector2(centrX, centrY), effect, 0);           
            SP.End();
        }

        public void SetAlpha(byte a)
        {
            if (a == 0) visible = false; 
            color.A = a;
            color.B = a;
            color.G = a;
            color.R = a;
        }

        public void GoEffect(GameTime gameTime)
        {
            eltime = eltime + gameTime.ElapsedGameTime.Milliseconds;
            if (eltime >= timeFrame)
            {
                if (enabledAnimation == true) { tekcount++; if (tekcount >= count) tekcount = 0; }
                eltime = 0;
            }
            //if ((Rect.Width <= RectOrig.Width - deviation) || (Rect.Width >= RectOrig.Width + deviation))  DXY = -DXY; 
            if (Rect.Width <= mindev)  DXY = -DXY;
            if (Rect.Width >= maxdev)  return; 
            Rect.Width = Rect.Width + 2 * DXY;
            Rect.Height = Rect.Height + 2 * DXY;
            Rect.X = Rect.X - DXY;
            Rect.Y = Rect.Y - DXY;
            if (AlX < 0) Rect.X -= DXY;
            if (AlX > 0) Rect.X += DXY;
            if (AlY < 0) Rect.Y -= DXY;
            if (AlY > 0) Rect.Y += DXY;
        }
        public void GoDefolt(GameTime gameTime)
        {
            eltime = eltime + gameTime.ElapsedGameTime.Milliseconds;
            if (eltime >= timeFrame)
            {
                if (enabledAnimation == true) { tekcount++; if (tekcount >= count) tekcount = 0; } 
                eltime = 0;
            }
            if (Rect.Width != RectOrig.Width) // возвращаемся к текущему размеру
                {
                    if (Rect.Width < RectOrig.Width)  DXY = -DXY;
                    Rect.Width = Rect.Width - 2 * DXY;
                    Rect.Height = Rect.Height - 2 * DXY;
                    Rect.X = Rect.X + DXY;
                    Rect.Y = Rect.Y + DXY;
                    if (AlX > 0) Rect.X -= DXY;
                    if (AlX < 0) Rect.X += DXY;
                    if (AlY > 0) Rect.Y -= DXY;
                    if (AlY < 0) Rect.Y += DXY;
                }
        }
        public int AuditXY(int x, int y, GameTime gameTime)
        {
            if (!visible) return 0;
            int a = onClickXY(x, y);
            if (a==1) GoEffect(gameTime); else GoDefolt(gameTime);
            return a;
        }
        public int onClickXY(int x, int y)
        {            
            if ((x >= RectOrig.X) && (x <= RectOrig.Right) && (y >= Rect.Y) && (y <= Rect.Bottom)) return 1;
            return 0;
        }

        public int LoadFile(string path, GraphicsDevice GD1)
        {
            if (!File.Exists(path)) return -1;
            if (string.Compare(Path.GetExtension(path), ".exe") == 0)  return LoadExe(path, GD1); 
            try
            {
                Image image = Image.FromFile(path);
                //imageFormat = "GIF";
                if (System.Drawing.Imaging.ImageFormat.Gif.Equals(image.RawFormat)) return LoadGif(path, GD1);
                //imageFormat = "ICON";
                if (System.Drawing.Imaging.ImageFormat.Icon.Equals(image.RawFormat)) return LoadIco(path, GD1);
                //imageFormat = "DEFOULT";
                return LoadDefoult(path, GD1);
                //MessageBox.Show("Требуется ввести имя");                              
            }
            catch
            {
               return -2;
            }                    
        }

        public int LoadExe(string path, GraphicsDevice GD1)
        {
            Stream s = new MemoryStream();
            Icon icon = Icon.ExtractAssociatedIcon(path);
            Bitmap b = icon.ToBitmap();
            b.Save(s,ImageFormat.Png);
            sprite = new Texture2D[1];
            sprite[0] = Texture2D.FromStream(GD1, s);
            centrX = sprite[0].Width / 2;
            centrY = sprite[0].Height / 2;
            this.count = 1;
            s.Close();
            return 1;
        }

      public int LoadDefoult(string path, GraphicsDevice GD1)
        {
            sprite = new Texture2D[1];
            sprite[0] = Texture2D.FromStream(GD1, File.OpenRead(path));
            centrX = sprite[0].Width / 2;
            centrY = sprite[0].Height / 2;
            this.count = 1;
            return 1;
        }

      private int LoadGif(string path, GraphicsDevice GD1)
      {
          System.Drawing.Image myImage = System.Drawing.Image.FromFile(path);
          System.Drawing.Imaging.FrameDimension d = new System.Drawing.Imaging.FrameDimension(myImage.FrameDimensionsList[0]);
          count = myImage.GetFrameCount(d);// 'количество кадров
          Stream s;
          sprite = new Texture2D[count];
          for (int i = 0; i < count; i++)
          {
              myImage.SelectActiveFrame(d, i);
              s = new MemoryStream();
              myImage.Save(s, ImageFormat.Png);
              sprite[i] = Texture2D.FromStream(GD1, s);
              s.Close();
          }
          centrX = sprite[0].Width / 2;
          centrY = sprite[0].Height / 2;
          tekcount = 0;
          return count;
      }
        private int LoadIco(string path, GraphicsDevice GD1)
        {
            Stream imageStreamSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            IconBitmapDecoder decoder = new IconBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            PngBitmapEncoder encoder;
            Stream s;
            int c = decoder.Frames.Count;
            if (c < 1)
            {
                imageStreamSource.Close();
                return -2;
            }
            double maxW = 0;
            int tk = 0;
            for (int a = 0; a < c; a++) //найдём самый широкий кадр наверное же он будет и самым качественным
            {
                if (decoder.Frames[a].Width > maxW)
                {
                    maxW = decoder.Frames[a].Width;
                    tk = a;
                }
            }
            sprite = new Texture2D[1];
            encoder = new PngBitmapEncoder();
            s = new MemoryStream();
            encoder.Frames.Add(decoder.Frames[tk]);
            encoder.Save(s);
            sprite[0] = Texture2D.FromStream(GD1, s);
            centrX = sprite[0].Width / 2;
            centrY = sprite[0].Height / 2;
            s.Close();           
            imageStreamSource.Close();
            count = 1;
            tekcount = 0;
            return count;
        }
             
    }
    //------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------
    public class IcoM : Ico
    {
        //Vector3 p3d;                                              //координаты в 3д
        public byte typeofico = 255;                              //тип иконки - внутрянняя служебная информация 
        public SpriteFont font;                                   //шрифт надписи
        public Vector2 V2 = new Vector2(0, 0);
        public string shortstr=null;                              //та строка которая будет отображаться 
        public Microsoft.Xna.Framework.Point V;
        //public bool wievstr = true;

        public IcoM(string path, SpriteFont fnt, string str, GraphicsDevice GD) : base(path, GD)//описание конструктора!!!!!
        {
            font = fnt;
            textstr = str;
            shortstr = str;
            V.X = -1;
            V.Y = -1;
        }

        public void SetTextPos()
        {
            Vector2 size = font.MeasureString(shortstr);
            V2.X = -(GetRect().Width - size.X) / 2;
        }
        public void GoToVect()
        {
            if (V.Y < 0) return;
            if (RectOrig.X < V.X) { RectOrig.X+=DXY; Rect.X+=DXY; }
            if (RectOrig.X > V.X) { RectOrig.X-=DXY; Rect.X-=DXY; }
        }

        public new void Draw(SpriteBatch SP)
        {
            if (!visible) return;
            base.Draw(SP);
            if ((shortstr!= null))
            SP.Begin();            
                SP.DrawString(font, shortstr, new Vector2(GetRect().Left, GetRect().Bottom + 20), Microsoft.Xna.Framework.Color.Azure, (float)(0), V2, 1, SpriteEffects.None, 0);
            SP.End();
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------
    class SystemIco : Ico
    {
        IntPtr deskr; // дескриптор приёмника
        //--------------------------------------------------------------------------------------------------------------------------
        public SystemIco(IntPtr e, string str, GraphicsDevice GD) : base(null, GD)//описание конструктора!!!!!
        {
            deskr = e;
            LoadResurs( GD);
            centrX = sprite[0].Width / 2;
            centrY = sprite[0].Height / 2;
            tekcount = 0;
            timeFrame = 200;
        }
        //--------------------------------------------------------------------------------------------------------------------------
        public int LoadResurs( GraphicsDevice GD)
        {
            IntPtr ip = IntPtr.Zero;
            int ew = 0;
            foreach (System.Diagnostics.Process winProc in System.Diagnostics.Process.GetProcesses())
            {
                IntPtr e = winProc.MainWindowHandle;
                if ((e != IntPtr.Zero)) 
                {                  
                    if (ew == 1) ip = e;
                    ew++;
                } //MessageBox.Show("Process " + winProc.Id + ": " + winProc.ProcessName);
            } 
            Bitmap myImage = CaptureScreen.CaptureScreen.GetDesktopImage(ip);
            sprite = new Texture2D[1];
            Stream s = new MemoryStream();            
            myImage.Save(s, ImageFormat.Png);
            sprite[0] = Texture2D.FromStream(GD, s);
            s.Close();           
            return 1;
        }
        //--------------------------------------------------------------------------------------------------------------------------
        public void GoEffect(GameTime gameTime, GraphicsDevice GD)
        {
            eltime = eltime + gameTime.ElapsedGameTime.Milliseconds;
            if (eltime >= timeFrame)
            {
                LoadResurs(GD);
                eltime = 0;
            }
            if (Rect.Width <= mindev) DXY = -DXY;
            if (Rect.Width >= maxdev) return;
            Rect.Width = Rect.Width + 2 * DXY;
            Rect.Height = Rect.Height + 2 * DXY;
            Rect.X = Rect.X - DXY;
            Rect.Y = Rect.Y - DXY;
            if (AlX < 0) Rect.X -= DXY;
            if (AlX > 0) Rect.X += DXY;
            if (AlY < 0) Rect.Y -= DXY;
            if (AlY > 0) Rect.Y += DXY;
        }
        //--------------------------------------------------------------------------------------------------------------------------
        public void GoDefolt(GameTime gameTime, GraphicsDevice GD)
        {
            eltime = eltime + gameTime.ElapsedGameTime.Milliseconds;
            if (eltime >= timeFrame)
            {
                LoadResurs(GD);
                eltime = 0;
            }
            if (Rect.Width != RectOrig.Width) // возвращаемся к текущему размеру
            {
                if (Rect.Width < RectOrig.Width) DXY = -DXY;
                Rect.Width = Rect.Width - 2 * DXY;
                Rect.Height = Rect.Height - 2 * DXY;
                Rect.X = Rect.X + DXY;
                Rect.Y = Rect.Y + DXY;
                if (AlX > 0) Rect.X -= DXY;
                if (AlX < 0) Rect.X += DXY;
                if (AlY > 0) Rect.Y -= DXY;
                if (AlY < 0) Rect.Y += DXY;
            }
        }
        public int AuditXY(int x, int y, GameTime gameTime, GraphicsDevice GD)
        {
            if (!visible) return 0;
            int a = onClickXY(x, y);
            if (a == 1) GoEffect(gameTime,GD); else GoDefolt(gameTime,GD);
            return a;
        }
    }
}
