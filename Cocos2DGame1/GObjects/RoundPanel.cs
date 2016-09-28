using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using VenLight.Utils;
using System.Windows.Forms;

namespace VenLight.Explorer
{
    class RoundPanel
    {
        private int count;
        public IcoM[] icons;
        private SpriteFont spriteFont;
        private Rectangle rect;

        public RoundPanel(string filePath, Rectangle r, SpriteFont sf, int count,GraphicsDevice graphicsDevice)
        {
                     
            spriteFont = sf;
            rect = r;
            if ((filePath == null) || (!File.Exists(filePath)))
            {
                this.count = count;   
                icons = new IcoM[count];
                for (int a = 0; a < this.count; a++)
                {
                    icons[a] = new IcoM(Settings.Settings.GetDefaultIconImageLink(),spriteFont,"123456", graphicsDevice);
                    Point XY = VectorFactory.GetPointFromRound(new Point(rect.Width / 2 + rect.X, rect.Height / 2 + rect.Y), rect.Height / 2, this.count, a);
                    icons[a].SetRect(new Rectangle(XY.X, XY.Y, 150, 150));
                }
            }
            else
            {
                string[] iniFile = File.ReadAllLines(filePath);
                this.count = iniFile.Length;
                icons = new IcoM[iniFile.Length];
                for (int a = 0; a < iniFile.Length; a++)
                {
                    icons[a] = new IcoM(Settings.Settings.GetThemeLink() + "round//" + StringFactory.GetPartStringWithSeparator(iniFile[a], " "[0], 1), spriteFont, StringFactory.GetPartStringWithSeparator(iniFile[a], " "[0], 3), graphicsDevice);
                    Point XY = VectorFactory.GetPointFromRound(new Point(rect.Width / 2 + rect.X, rect.Height / 2 + rect.Y), rect.Height / 2, this.count, a);
                    icons[a].SetRect(new Rectangle(XY.X, XY.Y, 150, 150));
                }
            }
           
        }
        //--- установка прозрачности --------------------------------------------------------------------------------------------------
        public void SetAlpha(byte a)
        {
            if (icons.Length < 1) return;
            for (int b = 0; b < icons.Length; b++) icons[b].SetAlpha(a);
        }
        //-------------------------------------------------------------------------------------------------
        public void Draw(SpriteBatch SP)
        {
            for (int a = 0; a < icons.Length; a++) icons[a].Draw(SP);
        }
        //-------------------------------------------------------------------------------------------------
        public void GoDefoult(GameTime gameTime)
        {
            for (int a = 0; a < icons.Length; a++) icons[a].GoEffect(gameTime); 
        }
        //-------------------------------------------------------------------------------------------------
    }
}
