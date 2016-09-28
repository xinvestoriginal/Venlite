using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Windows.Forms;
using System.Text;
using VenLight.Explorer;
using VenLight.Utils;
using Cocos2D;
using Cocos2DGame1;



namespace VenLight
{
    public class VenLight06 : Microsoft.Xna.Framework.Game
    {


        private readonly GraphicsDeviceManager graphics;
        string ininame = AppDomain.CurrentDomain.BaseDirectory + "VenLight06.ini";
        public bool EnabledThemeLoadFromFile = false;
        private int Width;
        private int Height;
        private Point pMAX;
        private Point pMIN;

        private Form1 tools = new Form1();
        private SpriteBatch spriteBatch;
        private Cur cur;
        private WinFold wf = null;

        private RoundPanel test;
        private bool ViewVect = false;
        private MouseState msold = Mouse.GetState();                                                               //состояние мышки
        private Point mpold;
        private int oldRPB = 0;
        public VenLight06()
        {
            graphics = new GraphicsDeviceManager(this);
            //#if MACOS
            //            Content.RootDirectory = "AngryNinjas/Content";
            //#else
            Content.RootDirectory = "Content";
            //#endif
            //
            //#if XBOX || OUYA
            //            graphics.IsFullScreen = true;
            //#else
            
            //#endif

            // Frame rate is 30 fps by default for Windows Phone.
            //TargetElapsedTime = TimeSpan.FromTicks(333333 / 2);

            // Extend battery life under lock.
            //InactiveSleepTime = TimeSpan.FromSeconds(1);

            //CCApplication application = new AppDelegate(this, graphics);
            //Components.Add(application);
            //#if XBOX || OUYA
            //            CCDirector.SharedDirector.GamePadEnabled = true;
            //            application.GamePadButtonUpdate += new CCGamePadButtonDelegate(application_GamePadButtonUpdate);
            //#endif

            //----------------------------------------------------------------------------------------
            
            if (!File.Exists(ininame)) MessageBox.Show("Ошибка VenLight06 - не найден файл конфигурации: " + ininame);
            else
            {                
                string[] ini =File.ReadAllLines(ininame, Encoding.Default); // загрузка файла настроек
                int a = ini.Length;
                if ((a > 1) && (string.Compare(StringFactory.FromString(ini[1], 1), "1") == 0)) graphics.IsFullScreen = true;
                if ((a > 2) && (string.Compare(StringFactory.FromString(ini[2], 1), "1") == 0)) EnabledThemeLoadFromFile = true;
            }

            

            Width  = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics.PreferredBackBufferWidth  = Width;
            graphics.PreferredBackBufferHeight = Height;
            graphics.IsFullScreen = true;
            
            //this.graphics.PreferredBackBufferFormat = SurfaceFormat.Bgr565; //Устанавливаем формат пиксела //Задаем режим 
            
        }
       
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true; // видно мышку
            base.Initialize();
        }

       
        protected override void LoadContent()
        {
            //--------------------------------------------------------------------------------------------------------
            //ByteFactory.Decoder(AppDomain.CurrentDomain.BaseDirectory + "theme.w8se");
            //--------------------------------------------------------------------------------------------------------
            if (EnabledThemeLoadFromFile) ByteFactory.LoadTheme(AppDomain.CurrentDomain.BaseDirectory + "theme.w8se");
            //--------------------------------------------------------------------------------------------------------
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //--------------------------------------------------------------------------------------------------------
            wf = new WinFold(new Rectangle(0, 0, this.graphics.PreferredBackBufferWidth,
                                                 this.graphics.PreferredBackBufferHeight),
                                                 Content.Load<SpriteFont>("fonts/SpriteFont1"), GraphicsDevice);            
            //--------------------------------------------------------------------------------------------------------
            cur = new Cur(Settings.Settings.GetThemeLink() + "cur.ini", GraphicsDevice, new Point(Width,Height));
            //--------------------------------------------------------------------------------------------------------
            pMAX = wf.AP.GetMaxArial();
            pMIN = wf.AP.GetMinArial();
            //--------------------------------------------------------------------------------------------------------
            int wh;
            int dx = 0;
            int dy = 0;
            if (graphics.PreferredBackBufferHeight < graphics.PreferredBackBufferWidth)
            {
                wh = graphics.PreferredBackBufferHeight - 150;
                dx = (graphics.PreferredBackBufferWidth - wh - 150) / 2;
            }
            else
            {
                wh = graphics.PreferredBackBufferWidth - 150;
                dy = (graphics.PreferredBackBufferHeight - wh - 150) / 2;
            }
            test = new RoundPanel(Settings.Settings.GetThemeLink() + "round//config.ini",
                                  new Rectangle(dx, dy, wh, wh), Content.Load<SpriteFont>("fonts/SpriteFont1"),
                                  8, GraphicsDevice);
            
        }

        protected override void UnloadContent()
        {            
        }
      
        protected override void Update(GameTime gameTime)
        {
            
            if ((this.IsActive == true)&&(!tools.Visible))
            {
                int si = -1;//есть ли выделенные иконки
                //---------------Обработка клавиатуры------------------------------
                if (Keyboard.GetState().GetPressedKeys().Contains(Microsoft.Xna.Framework.Input.Keys.Escape)) this.Exit();
                //---------------Обработка мышки-----------------------------------
                MouseState ms = Mouse.GetState();                                                                               //получим состояние мышки в текущий момент          
                int a = ByteFactory.RPB(ms, msold);                                                                             //проверим состояние клавиш мыши              
                //----------------------------------------------------------------------------------------------------------------------------
                if (ms.ScrollWheelValue != msold.ScrollWheelValue) if (ms.ScrollWheelValue < msold.ScrollWheelValue) wf.MoveIco(-50); else wf.MoveIco(50);//MessageBox.Show(ms.ScrollWheelValue.ToString());
                //---------------------------обработка видимости------------------------------------------------------------------------------
                if ((a==0)&&(oldRPB==4))  // отжата правая клавиша
                {              
                    if (cur.ViewReg == 0)
                    {
                        wf.AP.SetVisible(true);                    
                        cur.ViewReg=1;                             
                        mpold.X = Width / 2;
                        mpold.Y = Height / 2;
                        Mouse.SetPosition(mpold.X, mpold.Y - 1);
                        ms = Mouse.GetState();
                        ViewVect = true;
                    }
                    else
                    {
                        ViewVect = false;
                        cur.ViewReg = 0;
                        wf.AP.SetVisible(false);
                    }
                   
                }
                //---------------------------обработка эксплорера---------------------------------------------------------------------------------
                      
                if (ViewVect)
                 {                    
                   
                    if (cur.ViewReg == 1)
                    {
                        Point tp = VectorFactory.RetVectPoint(mpold.X, mpold.Y, ms.X, ms.Y, pMIN, pMAX);
                        wf.AP.AuditXY(tp.X, tp.Y, gameTime);
                        wf.GoDefoult(gameTime);
                        cur.SetVect(tp.X, tp.Y);
                        cur.SetRot(mpold.X, mpold.Y, ms.X, ms.Y);
                    }
                   
                    if (cur.ViewReg == 2) wf.MoveIco(mpold.Y-ms.Y);
                    if (!cur.OnArial(ms.X, ms.Y)) 
                     {
                        ViewVect = false;
                        wf.AP.SetVisible(false);
                        cur.ViewReg = 0;                       
                     }
                    //------------
                 }
                 else 
                    si=wf.AuditXY(ms.X, ms.Y, gameTime);
                if (a == 1) //случилось нажатие левой клавиши
                {
                    if (cur.ViewReg == 1)
                    {
                        Point tp = VectorFactory.RetVectPoint(mpold.X, mpold.Y, ms.X, ms.Y, pMIN, pMAX);
                        int i = wf.Reactor(tp, GraphicsDevice);
                        if (i == 1) Exit();
                        if (i == 2) { tools = new Form1(); tools.WindowState = FormWindowState.Maximized; tools.Show(); }
                    }
                    else
                    {
                        si = wf.onClickXY(ms.X, ms.Y, GraphicsDevice);
                        if (si < 0) { cur.ViewReg = 2; mpold.X = Width / 2; mpold.Y = Height / 2; Mouse.SetPosition(mpold.X, mpold.Y - 1); ViewVect = true; }
                    }
                }
                if (a >= 0) oldRPB = a;
                msold = ms;
            }
            test.GoDefoult(gameTime);
             
            base.Update(gameTime);
        }

      
        protected override void Draw(GameTime gameTime)
        {
            if (this.IsActive == true)
            {
                wf.Draw(spriteBatch);
                cur.Draw(spriteBatch);
            }
            //test.Draw(spriteBatch);
            base.Draw(gameTime);            
        }
     
    }
}
