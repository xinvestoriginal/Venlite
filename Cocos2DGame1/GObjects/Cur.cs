using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Windows.Forms;
using VenLight.Utils;
using Microsoft.Xna.Framework;

namespace VenLight
{
    class Cur
    {
        public Ico CurV;
        public Ico Cur0;
        public Ico CurWhite;
        public int ViewReg = 0;
        public Cur(string path, GraphicsDevice GD, Point pos)
        {
            if (!File.Exists(path)) MessageBox.Show("Ошибка Cur - не найден файл конфигурации: " + path);
            else
            {
                string[] ini = File.ReadAllLines(path, Encoding.Default); // загрузка файла настроек
                int a = ini.Length;
                if (a > 3) 
                {
                    string curVpath     = StringFactory.FromString(ini[1], 1);
                    string cur0path     = StringFactory.FromString(ini[2], 1);
                    string CurWhitepath = StringFactory.FromString(ini[3], 1);
                    CurV     = new Ico(Path.Combine(Path.GetDirectoryName(path), curVpath),     GD);
                    Cur0     = new Ico(Path.Combine(Path.GetDirectoryName(path), cur0path),     GD);
                    CurWhite = new Ico(Path.Combine(Path.GetDirectoryName(path), CurWhitepath), GD);
                    Cur0.SetRect(new Microsoft.Xna.Framework.Rectangle((pos.X / 2)-100, (pos.Y / 2)-100, 200, 200));
                    CurWhite.SetRect(new Microsoft.Xna.Framework.Rectangle((pos.X / 2) - 100, (int)(pos.Y /2) - 100, 200, 200));
                }                
            } 
        }

        public void SetVect(int x,int y)
        {
            CurV.SetX(x - CurV.GetRect().Width / 2);
            CurV.SetY(y - CurV.GetRect().Height / 2);
        }

        public void SetRot(int x1, int y1, int x2, int y2)
        {
            CurV.Rot = (float)VectorFactory.RetAngel(x1, y1, x2, y2);
        }

        public bool OnArial(int x,int y)
        {
            if ((Cur0.onClickXY(x, y) == 1) || ((CurWhite.onClickXY(x, y) == 1))) return true;
            return false;
        }

        public void Draw(SpriteBatch SP)
        {
            if (ViewReg == 0)                                  return;
            if (ViewReg == 1) { CurV.Draw(SP); Cur0.Draw(SP);  return; }
            if (ViewReg == 2) { CurWhite.Draw(SP);             return; }
        }
    }
}
