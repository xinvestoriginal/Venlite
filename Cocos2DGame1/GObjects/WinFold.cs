using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
//using IWshRuntimeLibrary;
using IWshRuntimeLibrary;
using VenLight.Utils;


namespace VenLight
{
    class WinFold
    {
        //-------------------------------------------------------------------------------------------------
        
        public string ini  = AppDomain.CurrentDomain.BaseDirectory + "explorer.ini";
        string startFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);        //папка Эксплорера
        public string DIR  = "";                   //текущий каталог
        public string pDIR = "";                   //родительский каталог
        //-------------------------------------------------------------------------------------------------
        public Rectangle RectF = new Rectangle();  // координаты проводника
        public Rectangle Arial = new Rectangle();  // координаты на которых будут расположены иконки
        //-------------------------------------------------------------------------------------------------
        //public int SelectedIco = -1;               //индекс выделенного в настоящее время файла
        public byte   BufferFileType = 0;
        public string BufferString = "";
        public bool eraseoldfile;
        //-------------------------------------------------------------------------------------------------
        private bool enabledAnimation = true;      //разрешает анимацию всех значков
        //-------------------------------------------------------------------------------------------------
        public string[] LinkList = new string[24]; //список прошлых папок проводника
        public int tekLink = 0;                    //текущая позиция в этом списке
        //-------------------------------------------------------------------------------------------------
        public ALLPanels AP;
        public Panel2D p2D;
        public SpriteFont font;
        public Ico fon = null;
       
        public WinFold(Microsoft.Xna.Framework.Rectangle r, SpriteFont font,GraphicsDevice GD1)
        {
            RectF = r;
            Arial = RectF;
            this.font = font;            
            string fonpath = Settings.Settings.GetBackgroundLink();
            AP = new ALLPanels(Settings.Settings.GetThemeLink() + "\\panels.ini", RectF,(int)(RectF.Width * 0.1), (int)(RectF.Height * 0.38), GD1);
            //------ загрузка настроек из файла ---------------------------------------------------------------
            if (System.IO.File.Exists(ini))
            {
                string[] nastr = System.IO.File.ReadAllLines(ini);
                int nastrcount = nastr.Length;
                if ((nastrcount > 1) && (string.Compare(StringFactory.FromString(nastr[1], 1), "0") == 0)) enabledAnimation = false; //запрещает анимацию значков
                if ((nastrcount > 2) && (string.Compare(StringFactory.FromString(nastr[2], 1), "0") == 0)) AP.SetEA(false);          //запрещает анимацию панелей
                //if ((nastrcount > 3) && (string.Compare(ByteFactory.FromString(nastr[3],1), "0") == 0)) enabledShortStr = false;  //запрещает короткие строки в имени файлов
                //if  (nastrcount > 4) fonpath = ByteFactory.FromString(nastr[4],1);
            }
            else MessageBox.Show("Файл настроек Эксплорера "+ini+" не найден");
            //------ загрука фона -----------------------------------------------------------------------------
            if (System.IO.File.Exists(fonpath) == true)
            {
                fon = new Ico(fonpath, GD1);
                fon.SetRect(RectF);
                fon.enabledAnimation = enabledAnimation;
                fon.timeFrame = 30;
            }
            else MessageBox.Show("!!!"+fonpath);
            //-------------------------------------------------------------------------------------------------
            search(startFolder, true, GD1);
        }
        //-----------------------------------------------------------------------------------------------------
        private string GetLinkFromExt(string fl)
        {
            string ex = Path.GetExtension(fl);
            if (ex == ".jpg") return fl;
            if (ex == ".ico") return fl;
            if (ex == ".gif") return fl;
            if (ex == ".png") return fl;
            if (ex == ".exe") return fl;
            if (ex == ".zip") return Settings.Settings.GetThemeLink() + "arch.png";
            if (ex == ".rar") return Settings.Settings.GetThemeLink() + "arch.png";
            if (ex == ".txt") return Settings.Settings.GetThemeLink() + "blocknot.png";
            if (ex == ".rtf") return Settings.Settings.GetThemeLink() + "blocknot.png";
            if (ex == ".ini") return Settings.Settings.GetThemeLink() + "blocknot.png";
            if (ex == ".url") return Settings.Settings.GetThemeLink() + "url.png";
            /*
            if (ex == ".lnk")
            {
                WshShell shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(fl);
                return GetLinkFromExt(link.TargetPath);
            }
             * */
            return Settings.Settings.GetThemeLink() + "default_icon.gif";
        }
        //-----------------------------------------------------------------------------------------------------
        public void search(string dir, bool AddLnk, GraphicsDevice GD) // ищет файлы и папки в заданой директории и загружает их в значки 
        {
            if ((dir == null) || (string.Compare(dir, "") == 0)) return;
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            try
            {
                //------------проверяем доступность диска по указанному пути если диск недоступен то выходим из процедуры ---------------------
                if (dir.Length < 2) return;
                DriveInfo disk = new DriveInfo(dir[0].ToString() + dir[1].ToString());
                if (!disk.IsReady) return;
                //------------получаем родительский каталог -----------------------------------------------------------------------------------
                DIR = dirInfo.FullName;
                pDIR = Path.GetDirectoryName(DIR);
                if ((pDIR==null) || (string.Compare(pDIR, "") == 0)) pDIR = DIR;
                //------------получаем информацию о каталоге ----------------------------------------------------------------------------------                     
                FileInfo[] files = dirInfo.GetFiles();
                DirectoryInfo[] dirs = dirInfo.GetDirectories();
                p2D = new Panel2D(RectF, dirs.Length + files.Length, font);
                for (int b = 0; b < dirs.Length; b++) p2D.AddIco(b, Settings.Settings.GetThemeLink() + "folder_icon.gif", dirs[b].Name, 0, enabledAnimation, GD);
                for (int a = dirs.Length; a < files.Length + dirs.Length; a++) p2D.AddIco(a, GetLinkFromExt(Path.Combine(DIR, files[a - dirs.Length].Name)), files[a - dirs.Length].Name, 1, enabledAnimation, GD);
                p2D.SetIcoXY(0,0);
                if (AddLnk) AddLink(DIR); //помещает ссылку в историю
            }
            catch
            {
                MessageBox.Show("Ошибка search.");
            }
        }
        //--- отображение объектов на экране --------------------------------------------------------------------------------------------------
        public void Draw(SpriteBatch SP)
        {            
            if (fon != null) fon.Draw(SP);
            p2D.Draw(SP);
            AP.Draw(SP);
        }
        //--- установка прозрачности --------------------------------------------------------------------------------------------------
        public void SetAlpha(byte a)
        {
            p2D.SetAlpha(a);
        }
        //--- проверка активности значков в зависимости от координат ------------------------------------------------------------------
        public int AuditXY(int x, int y, GameTime gameTime)
        {
            if (fon != null) fon.GoDefolt(gameTime);           
            return p2D.AuditXY(x, y, gameTime);
        }
        //--- все значки отображаются по дефолту ---------------------------------------------------------------------------------------------
        public void GoDefoult(GameTime gameTime)
        {
            if (fon != null) fon.GoDefolt(gameTime);
            p2D.GoDefoult(gameTime);
        }
       
        //--- реакция на нажатие клавиши --------------------------------------------------------------------------------------------------
        public int onClickXY(int x, int y,  GraphicsDevice GD1)                                             // возвращает  -1 если нажатие пришлось не на объект
        {
            if (AP.visible) return -2;                                                                                    //              0 если нажатие пришлось на пустое место объекта      
            if ((x < RectF.X) || (x > RectF.X + RectF.Width) || (y < RectF.Y) || (y > RectF.Y + RectF.Height)) return -1; //              1 если нажатие пришлось на папку
            //if (icons.Length != 0) for (int a = 0; a < icons.Length; a++) if (icons[a].onClickXY(x, y) == 1)              //              2 если нажатие пришлось на файл
              //      {                                                                                                     //              3 если нажатие пришлось на выход в предыдущую директорию
            if (p2D.SelectedIco<0) return-3;       
            string fn = DIR + "\\" + p2D.icons[p2D.SelectedIco].textstr;
            switch ( p2D.icons[p2D.SelectedIco].typeofico)
             {
               case 0:search(fn,true, GD1);                return 1;
               case 1:System.Diagnostics.Process.Start(fn);return 2;
               case 2:search(pDIR,true,GD1);               return 3;
               default: return 0;                        
             }   
            
        }
        //--- переходит по ссылке в истории вперёд или назад --------------------------------------------------------------------------------------------------
        public void GoToLink(int a, GraphicsDevice GD)   //переходит либо в предыдущий каталог либо в следующий
        {
            int t = tekLink;
            if (a < 0) { if (tekLink == 0) tekLink = LinkList.Length - 1; else tekLink--; }
            if (a >= 0) { if (tekLink == LinkList.Length - 1) tekLink = 0; else tekLink++; }
            string  res = LinkList[tekLink];
            if ((res != null) && (string.Compare(res, "") != 0)) search(res, false, GD); else tekLink = t;
        }
        //--- помещает ссылку в историю --------------------------------------------------------------------------------------------------
        public void AddLink(string s)                   //добавляет  директорию в список 
        {
            if (string.Compare(s, LinkList[tekLink]) == 0) return;
            tekLink++;
            if (tekLink == LinkList.Length) tekLink = 0;
            LinkList[tekLink] = s;            
        }
        public void MoveIco(int dy)
        {
            p2D.MoveIco(0, dy);
        }
        //--- удаляет файл или папку --------------------------------------------------------------------------------------------------
        public void EraseObj(GraphicsDevice GD)
        {
            if (p2D.SelectedIco != -1)
            {
                if (p2D.icons[p2D.SelectedIco].typeofico == 0) Directory.Delete(DIR + "\\" + p2D.icons[p2D.SelectedIco].textstr, true);
                if (p2D.icons[p2D.SelectedIco].typeofico == 1) System.IO.File.Delete(DIR + "\\" + p2D.icons[p2D.SelectedIco].textstr);
                search(DIR, false, GD);
            }
        }
        //--- копирует либо вырезает файл либо папку ----------------------------------------------------------------------------------
        public void CopyCutObj(bool e)
        {
            eraseoldfile = e;
            if (p2D.SelectedIco != -1)
            {
                BufferFileType = p2D.icons[p2D.SelectedIco].typeofico;
                BufferString = DIR + "\\" + p2D.icons[p2D.SelectedIco].textstr;
                p2D.SelectedIco = -1;
            }
        }
        //--- вставляет файл либо папку в текущую директорию --------------------------------------------------------------------------------------------------
        public void PasteObj(GraphicsDevice GD)
        {
            if (BufferFileType == 0) 
            {
                ByteFactory.CopyDir(BufferString, Path.Combine(DIR, Path.GetFileName(BufferString)));
                if (eraseoldfile) Directory.Delete(BufferString, true);          
            }
            if (BufferFileType == 1)
            {
                FileInfo fi = new FileInfo(BufferString);
                if (eraseoldfile) fi.MoveTo(DIR + Path.GetFileName(BufferString)); else fi.CopyTo(DIR + Path.GetFileName(BufferString));
            }
            search(DIR,false,GD);
        }
        //--- реакция на полученное имя форточки -------------------------------------------------------------------------------------------------
        public int Reactor(Point p,GraphicsDevice GD)
        {
            string s = AP.GetTextStrFromPos(p.X, p.Y);                  
            if (s.Length == 3) search(s, true, GD);
            if (string.Compare(s, "exit"      ) == 0) return 1;
            if (string.Compare(s, "parentdir" ) == 0) search(pDIR, true, GD); ;
            if (string.Compare(s, "left") == 0) p2D.MoveIco(0,-10);//p2D.changee(-1);
            if (string.Compare(s, "right") == 0) p2D.MoveIco(0,10);//p2D.changee(1);
            if (string.Compare(s, "back"      ) == 0) GoToLink(-1, GD);
            if (string.Compare(s, "move"      ) == 0) GoToLink(1, GD);
            if (string.Compare(s, "cutobj"    ) == 0) CopyCutObj(true);
            if (string.Compare(s, "copyobj"   ) == 0) CopyCutObj(false);
            if (string.Compare(s, "pasteobj"  ) == 0) PasteObj(GD);
            if (string.Compare(s, "eraseobj"  ) == 0) EraseObj(GD);
            //if (string.Compare(s, "browser"   ) == 0) wf.ShutDown();            //MessageBox.Show("Загружаю стрелку");
            if (string.Compare(s, "tools") == 0)  return 2;
            /*if (string.Compare(s, "tools") == 0)
            {
             foreach (System.Diagnostics.Process winProc in System.Diagnostics.Process.GetProcesses()) 
             {
                 IntPtr e = winProc.MainWindowHandle;
                 if (e != IntPtr.Zero) MessageBox.Show("Process " + winProc.Id + ": " + winProc.ProcessName);
             } 
            }*/
            return 0;
        }
        public void Reset()
        {
            ByteFactory.ShutDown(true);
        }
        public void ShutDown()
        {
            ByteFactory.ShutDown(false);
        }
    }
}
