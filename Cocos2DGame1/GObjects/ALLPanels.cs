using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VenLight.Utils;

namespace VenLight
{
    //------------------------------------------------------------------------------------------------------------------------------------------------
    class ALLPanels
    {
        public Rectangle Rect;
        public Panel[] panels = null;
        public bool visible = false;
        public byte lp = 255;
        public byte rp = 255;
        public byte dp = 255;
        public byte up = 255;
        public string AlfFolder = AppDomain.CurrentDomain.BaseDirectory + "\\Theme\\alfavit";
        public ALLPanels(string ininame, Rectangle r, int Width, int Height, GraphicsDevice GD)
        {
            Rect = r;
            if (!File.Exists(ininame)) MessageBox.Show("Ошибка ALLPanels- не найден файл конфигурации: " + ininame);
            string[] inipanels = File.ReadAllLines(ininame);
            panels = new Panel[inipanels.Length];
            for (int a = 0; a < inipanels.Length; a++)
            {
                string t1 = StringFactory.FromString(inipanels[a], 1);
                string LinkToPanel = Path.GetDirectoryName(ininame) + "\\" + StringFactory.FromString(inipanels[a], 2); //ссылка на файл конфигурации каждой панели
                if (File.Exists(LinkToPanel))
                {
                    string[] fp = File.ReadAllLines(LinkToPanel);
                    string[] context = StringFactory.GetMasStr(fp, 0, 2);
                    fp = StringFactory.PasteSTR(Path.GetDirectoryName(LinkToPanel) + "\\", StringFactory.GetMasStr(fp, 0, 1));
                    if (string.Compare(t1, "L") == 0) { panels[a] = new Panel(new Rectangle(0, Height, Width, Rect.Height - 2 * Height), fp, context, ByteFactory.ALeft, GD); lp = (byte)a; }
                    else if (string.Compare(t1, "R") == 0) { panels[a] = new Panel(new Rectangle(Rect.Width - Width, Height, Width, Rect.Height - 2 * Height), fp, context, ByteFactory.ARight, GD); rp = (byte)a; }
                    else if (string.Compare(t1, "U") == 0) { panels[a] = new Panel(new Rectangle(0, 0, Rect.Width, Height), fp, context, ByteFactory.AUp, GD); up = (byte)a; }
                    else if (string.Compare(t1, "D") == 0) { panels[a] = new Panel(new Rectangle(0, Rect.Height - Height, Rect.Width, Height), fp, context, ByteFactory.ADown, GD); dp = (byte)a; }
                    else panels[a] = null;
                }
                else
                {
                    MessageBox.Show("Файл конфигурации панели " + LinkToPanel + "не найден");
                }
            }
            panels[up] = AddPanel(panels[up], CreateDisksPanel(AlfFolder, GD));
        }
        public void Draw(SpriteBatch SP)
        {
            if (!visible) return;
            for (int a = 0; a < panels.Length; a++) if (panels[a] != null) panels[a].Draw(SP);
        }
        public void AuditXY(int x, int y, GameTime gameTime)
        {
            if (!visible) return;
            for (int a = 0; a < panels.Length; a++) if (panels[a] != null) panels[a].AuditXY(x, y, gameTime);
        }
        public Point GetMinArial()
        {
            Point res;
            if (lp != 255) res.X = panels[lp].icons[0].GetRect().Right; else res.X = Rect.X;
            if (up != 255) res.Y = panels[up].icons[0].GetRect().Bottom; else res.Y = Rect.Y;
            return res;
        }
        public Point GetMaxArial()
        {
            Point res;
            if (rp != 255) res.X = panels[rp].icons[0].GetRect().X; else res.X = Rect.Right;
            if (up != 255) res.Y = panels[dp].icons[0].GetRect().Y; else res.Y = Rect.Bottom;
            return res;
        }
        public string GetTextStrFromPos(int x, int y)
        {
            if (!visible) return "NULL";
            for (int a = 0; a < panels.Length; a++) if (panels[a] != null)
                {
                    string s = panels[a].GetTextStrFromPos(x, y);
                    if (string.Compare(s, "NULL") != 0) return s;
                }
            return "NULL";
        }
        public void SetVisible(bool u)
        {
            visible = u;
        }
        public void SetEA(bool ea)
        {
            for (int a = 0; a < panels.Length; a++) if (panels[a] != null) for (int b = 0; b < panels[a].icons.Length; b++) panels[a].icons[b].enabledAnimation = ea;
        }
        public Panel AddPanel(Panel p1, Panel p2)
        {
            if (p1 == null) return null;
            if (p2 == null) return p1;
            if (p2.icons == null) return p1;
            for (int a = 0; a < p2.icons.Length; a++) p1.Add(p2.icons[a]);
            p2 = null;
            return p1;
        }
        public Panel CreateDisksPanel(string alffolder, GraphicsDevice GD)
        {
            string[] DriveList = Environment.GetLogicalDrives();
            string[] LinkAlf = new string[DriveList.Length];
            for (int a = 0; a < DriveList.Length; a++)
            {
                char c = (char)((int)(DriveList[a][0]) + 32);
                LinkAlf[a] = alffolder + "\\" + c.ToString() + ".gif";
            }
            Panel p = new Panel(new Rectangle(0, 0, 100, 100), LinkAlf, DriveList, ByteFactory.ANone, GD);
            return p;
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------
    class Panel
    {
        public Rectangle Rect;
        public Ico[] icons;
        public int count = 0;
        public bool visible = true;                //разрешает отображение
        private bool gorpos = true;                //определяет является ли панель горизонтальной
        public int Aline = 0;
        public int SelectedIco = -1;
        public int iView = 0;

        public Panel(Rectangle r, string[] s, string[] kontext, int al, GraphicsDevice GD)
        {
            this.Rect = r;
            count = s.Length;
            icons = new Ico[s.Length];
            for (int a = 0; a < count; a++) icons[a] = new Ico(s[a], GD);
            int temp = (int)(10 * Rect.Width / Rect.Height);
            if (temp < 1) gorpos = false;
            SetTextStr(kontext);
            Aline = al;
            SetAl();
            //if (Aline == ByteFactory.AUp) for (int a = 0; a < count; a++) if (a == 0) icons[a].SetAlXY(1, 1); else if (a == count - 1) icons[a].SetAlXY(-1, 1); else icons[a].SetAlXY(0, 1);
            ResetPosition();
        }

        public void SetAl()
        {
            if (Aline == ByteFactory.AUp) for (int a = 0; a < count; a++) if (a == 0) icons[a].SetAlXY(1, 1); else if (a == count - 1) icons[a].SetAlXY(-1, 1); else icons[a].SetAlXY(0, 1);
            if (Aline == ByteFactory.ADown) for (int a = 0; a < count; a++) if (a == 0) icons[a].SetAlXY(1, -1); else if (a == count - 1) icons[a].SetAlXY(-1, -1); else icons[a].SetAlXY(0, -1);
            if (Aline == ByteFactory.ALeft) for (int a = 0; a < count; a++) icons[a].SetAlXY(1, 0);
            if (Aline == ByteFactory.ARight) for (int a = 0; a < count; a++) icons[a].SetAlXY(-1, 0);
        }

        public int GetiView()
        {
            return iView;
        }

        public void ResetPosition() //пересчитывает координаты и размеры значков в панеле
        {
            int x;
            int y;
            int Width;
            int Height;
            if (gorpos == true) Height = Rect.Height; else Height = (int)(0.99 * Rect.Height / count);              //(int)((Rect.Height / count) - (Rect.Height * 0.01));
            if (gorpos == false) Width = Rect.Width; else Width = (int)(0.99 * Rect.Width / count);                                                  //(int)((Rect.Width / count) - (Rect.Width * 0.01));
            if (Width < Height) Height = Width; else Width = Height;//иконки будут квадратные           
            for (int a = 0; a < count; a++)
            {
                if (gorpos == false) y = Rect.Y + (int)((Rect.Height / count) * a + (Rect.Height - Height * count) / (2 * count)); else y = Rect.Y;
                if (gorpos == true) x = Rect.X + (int)((Rect.Width / count) * a + (Rect.Width - Width * count) / (2 * count)); else x = Rect.X;
                if (Aline == ByteFactory.ADown) y = Rect.Y + Rect.Height - Height;
                icons[a].SetRect(new Rectangle(x, y, Width, Height));
            }
        }

        private int SetTextStr(string[] ms)
        {
            if (count == 0) return -1;
            if (ms.Length == 0)
            {
                for (int a = 0; a < count; a++) icons[a].textstr = "";
                return 0;
            }
            for (int a = 0; a < count; a++) if (a < ms.Length) icons[a].textstr = ms[a]; else icons[a].textstr = "";
            return 1;
        }
        public int SetEffect(int nI, bool SeA, SpriteEffects SsE, Microsoft.Xna.Framework.Color clr)
        {
            if ((nI < 0) || (nI >= count)) return -1;
            if (SeA) icons[nI].enabledAnimation = !icons[nI].enabledAnimation;
            if (!(SsE == SpriteEffects.None)) icons[nI].effect = SsE;
            icons[nI].color = clr;
            return 0;
        }

        public void Add(string path, string txtstr, GraphicsDevice GD)
        {
            count++;
            Array.Resize(ref icons, count);
            icons[count - 1] = new Ico(path, GD);
            icons[count - 1].textstr = txtstr;
            SetAl();
            ResetPosition();
        }
        public void Add(Ico i)
        {
            count++;
            Array.Resize(ref icons, count);
            icons[count - 1] = i;
            SetAl();
            ResetPosition();
        }

        public void Draw(SpriteBatch SP)
        {
            if (!visible) return;
            for (int a = 0; a < count; a++) if (SelectedIco != a) icons[a].Draw(SP);
            if (SelectedIco >= 0) icons[SelectedIco].Draw(SP);
        }
        public int AuditXY(int x, int y, GameTime gameTime)
        {
            if (!visible) return -1;
            int ncl = GetObjFromXY(x, y);
            if (ncl >= 0) SelectedIco = ncl; else SelectedIco = -1;
            for (int a = 0; a < count; a++) if (a == ncl) icons[a].GoEffect(gameTime); else icons[a].GoDefolt(gameTime);
            return ncl;
        }
        public int GetObjFromXY(int x, int y)
        {
            if ((x < Rect.X) || (x > Rect.X + Rect.Width) || (y < Rect.Y) || (y > Rect.Y + Rect.Height)) return -2;
            if (gorpos) for (int a = 0; a < count; a++) if ((x >= Rect.X + (Rect.Width / count) * a) && (x <= Rect.X + (Rect.Width / count) * (a + 1)) && (y >= Rect.Y) && (y <= Rect.Y + Rect.Height)) return a;
            if (!gorpos) for (int a = 0; a < count; a++) if ((x >= Rect.X) && (x <= Rect.X + Rect.Width) && (y >= Rect.Y + (Rect.Height / count) * a) && (y <= Rect.Y + (Rect.Height / count) * (a + 1))) return a;
            return -3;
        }
        public string GetTextStrFromPos(int x, int y)
        {
            if (!visible) return "NULL";
            int a = GetObjFromXY(x, y);
            if (a < 0) return "NULL";
            string s = icons[a].textstr;
            if (s == null) s = "NULL";
            if (String.Compare(s, "NULL") == 0) return a.ToString(); else return s;
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------
    public class Panel2D
    {
        //-------------------------------------------------------------------------------------------------
        //bool visible = false;
        public IcoM[] icons;
        protected Rectangle Rect;
        SpriteFont sf;
        protected int maxx = 4;
        protected int maxy = 3;
        protected Rectangle Arial = new Rectangle(0, 0, 0, 0);
        protected int ECount = 0;
        protected int EIndex = 0;
        public int SelectedIco;
        //-------------------------------------------------------------------------------------------------
        public Panel2D(Rectangle r, int kicons, SpriteFont sfon)
        {
            Rect = r;
            icons = new IcoM[kicons];
            sf = sfon;
            SelectedIco = -1;
        }
        //-------------------------------------------------------------------------------------------------
        public void AddIco(int k, string path, string str, byte tico, bool ea, GraphicsDevice GD)
        {
            icons[k] = new IcoM(path, sf, str, GD);
            icons[k].typeofico = tico;
            icons[k].enabledAnimation = ea;
            icons[k].shortstr = StringFactory.GetShortStr(str, true);
        }
        //--- изменить расположение иконок на экране  --------------------------------------------------------------------------------------------------
        public int SetIcoXY(int newmaxx, int newmaxy) //пересчитывает координаты значков
        {
            if (newmaxx != 0) maxx = newmaxx;
            if (newmaxy != 0) maxy = newmaxy;
            EIndex = 0;
            int x = -1;
            int y = 0;
            int fWidth = Rect.Width / maxx;
            int fHeight = Rect.Height / maxy;
            int Width = (int)(fWidth * 0.62);
            int Height = (int)(fHeight * 0.62);
            int DX = (Rect.Width - maxx * Width) / (2 * maxx);
            Arial = Rect;
            for (int a = 0; a < icons.Length; a++)
            {
                x++;
                if (x >= maxx)
                {
                    x = 0;
                    y++;
                    if (y >= maxy)
                    {
                        y = 0;
                        EIndex++;
                    }
                };
                //int alx = 0;
                //int aly = 0;
                //if (x == 0) alx = 1; else if (x == maxx - 1) alx = -1;
                //if (y == 0) aly = 1; else if (y == maxy - 1) aly = -1;

                icons[a].SetRect(new Rectangle((Arial.X + fWidth * x + DX), (Arial.Y + fHeight * y + EIndex * Arial.Height), Width, Height));
                icons[a].visible = true;
            }
            for (int a = 0; a < icons.Length; a++) icons[a].SetTextPos();
            ECount = EIndex + 1;
            Arial.Height = icons[icons.Length - 1].GetRect().Bottom + 100;
            EIndex = 0;
            return ECount;
        }
        /*public int SetIcoXY(int newmaxx, int newmaxy) //пересчитывает координаты значков
        {
            if (newmaxx != 0) maxx = newmaxx;
            if (newmaxy != 0) maxy = newmaxy;
            int x;
            int y;
            int Width=100;
            int Height=100;
            for (int a = 0; a < icons.Length; a++)
            {
                double angle = (3.14159255 * (a) / (icons.Length-1));
                double R = (Rect.Width-Width) / 2;
                x = (Rect.Width-Width)/2+(int)(Math.Cos(angle) * R);
                y = Rect.Height-(int)(Math.Sin(angle) *R)-150;
                icons[a].SetRect(new Rectangle(x,y, Width, Height));
                icons[a].visible = true;
            }

            return 0;
        }
          */
        //--- установка прозрачности --------------------------------------------------------------------------------------------------
        public void SetAlpha(byte a)
        {
            if (icons.Length < 1) return;
            for (int b = 0; b < icons.Length; b++) icons[b].SetAlpha(a);
        }
        //-------------------------------------------------------------------------------------------------
        public void MoveIco(int dx, int dy)
        {
            if (dx != 0) for (int a = 0; a < icons.Length; a++) { icons[a].SetX(icons[a].GetRect().X + dx); if (icons[a].GetRect().X > Arial.Right) icons[a].SetX(icons[a].GetRect().X - Arial.Width); if (icons[a].GetRect().X < Arial.X) icons[a].SetX(icons[a].GetRect().X + Arial.Width); }
            if (Arial.Bottom + dy < Rect.Height) dy = Rect.Height - Arial.Bottom;
            if (Arial.Y + dy > Rect.Y) dy = Rect.Y - Arial.Y;
            if ((dy != 0) && (Arial.Bottom + dy >= Rect.Height) && (Arial.Y + dy <= Rect.Y))
            {
                Arial.Y += dy;
                for (int a = 0; a < icons.Length; a++) icons[a].SetY(icons[a].GetRect().Y + dy);
            }
        }
        //-------------------------------------------------------------------------------------------------
        public void Draw(SpriteBatch SP)
        {
            for (int a = icons.Length - 1; a >= 0; a--) if (SelectedIco != a) icons[a].Draw(SP);
            if (SelectedIco != -1) icons[SelectedIco].Draw(SP);
        }
        //-------------------------------------------------------------------------------------------------
        public void GoDefoult(GameTime gameTime)
        {
            for (int a = 0; a < icons.Length; a++) if (a == SelectedIco) icons[a].GoEffect(gameTime); else icons[a].GoDefolt(gameTime);
        }
        //-------------------------------------------------------------------------------------------------
        //--- проверка активности значков в зависимости от координат ------------------------------------------------------------------
        public int AuditXY(int x, int y, GameTime gameTime)
        {
            SelectedIco = -1;
            for (int a = 0; a < icons.Length; a++) if (icons[a].AuditXY(x, y, gameTime) == 1) SelectedIco = a;
            return SelectedIco;
        }
        //-------------------------------------------------------------------------------------------------
    }

   

}
