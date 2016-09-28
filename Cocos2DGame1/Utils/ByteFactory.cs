using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
//using System.IO.Compression;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VenLight
{
    public static class ByteFactory //здесь будет набор функций для преобразования чисел и файлов
    {
        public static string ThemeFolder = AppDomain.CurrentDomain.BaseDirectory+"\\Theme";
        public static string tini = "theme.ini";
        //--------------------------------
        public const int ALeft   = 1;
        public const int ARight  = 2;
        public const int AUp     = 4;
        public const int ADown   = 8;
        public const int ANone   = 0;
        //--------------------------------
        //-----выключение либо перезагрузка ---------------------------------------------------------------
        [DllImport("advapi32.dll", EntryPoint = "InitiateSystemShutdownEx")] public static extern int InitiateSystemShutdownEx(string lpMachineName, string lpMessage, int dwTimeout, bool bForceAppsClosed, bool bRebootAfterShutdown, int dwReason);
        public static void ShutDown(bool u)
        {
            InitiateSystemShutdownEx("127.0.0.1", null, 0, false, u, 0);
        }
        //-------------------------------------------------------------------------------------------------
        public static int Decoder(string path)    //декодирует файл
        {
            if (!File.Exists(path)) MessageBox.Show("Ошибка Decoder файл "+path+" не найден");
            byte[] bytes = File.ReadAllBytes(path);
            if (bytes.Length < 5) MessageBox.Show("Ошибка Decoder файл меньше 1000 байтов");
            for (int a = 0; a <5; a++) bytes[a] = (byte)(255 - bytes[a]);
                File.WriteAllBytes(path, bytes);
            return 0;
        }
        //--- кодирует файл -----------------------------------------------------------------------------
        public static int Encoder(string path)    
        {
            return Decoder(path);
        }
        //--- загружает файл темы ----------------------------------------------------------------------- 
        public static int LoadTheme(string path)                                                   
        {
           /*if (!File.Exists(path)) MessageBox.Show("Ошибка LoadTheme не найден файл: "+path);
           if (Directory.Exists(ThemeFolder)) Directory.Delete(ThemeFolder, true);                //удаляем папку с темой если есть
           if (File.Exists(path + "0")) File.Delete(path + "0");                                  //удаляем временный файл
           if (File.Exists(Path.GetDirectoryName(path) + "\\theme.tmp"))
               File.Delete(Path.GetDirectoryName(path) + "\\theme.tmp");                          //удаляем  2oi временный файл
           File.Copy(path, path + "0");                                                           //копируем файл темы во временный файл
           Decoder(path + "0");                                                                   //декодируем временный файл в архив
           ZipFile.ExtractToDirectory(path + "0", Path.GetDirectoryName(path));                   //распаковываем архив
           ZipFile.ExtractToDirectory(Path.GetDirectoryName(path) + "\\theme.tmp", Path.GetDirectoryName(path));             //распаковываем архив
           File.Delete(path + "0");                                                               //удаляем временный файл
           if (File.Exists(Path.GetDirectoryName(path) + "\\theme.tmp"))
               File.Delete(Path.GetDirectoryName(path) + "\\theme.tmp");                          //удаляем  2oi временный файл
           if (!File.Exists(ThemeFolder+tini)) return -1; */    //проверяем наличие файла конфигурации
           return 0;
        }
        /*public static void DeleteTheme()
        {
            if (Directory.Exists(ThemeFolder)) Directory.Delete(ThemeFolder, true);                //удаляем папку с темой если есть
        }*/
        //--- возвращает псевдослучайное число в заданом диапазоне -------------------------------------
        public static int RND(int a)
        {
            Random rnd = new Random();
            return rnd.Next(a);
        }
        
        //--- возвращает комбинацию нажатых клавиш мыши ----------------------------------------------
        public static int RPB(MouseState ms1, MouseState ms2)    //возвращает состояние клавиш мыши  
        {
            if ((ms1.LeftButton == ms2.LeftButton) && (ms1.MiddleButton == ms2.MiddleButton) && (ms1.RightButton == ms2.RightButton)) return -1;
            int r = 0;
            if (ms1.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)   r = r + 1;
            if (ms1.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) r = r + 2;
            if (ms1.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)  r = r + 4;
            return r;
        }
        //-------------------------------------------------------------------------------------------
        //--- работа с текстовиками -----------------------------------------------------------------
       
        //------проверяет наличие всех файлов в списке ------------------------------------------------------
        public static int AuditFiles(string[] str)
        {
            if (str.Length < 1) MessageBox.Show("Ошибка AuditFiles- мало строк в файле");
            int b = 0;
            for (int a = 0; a < str.Length; a++) if (!File.Exists(str[a])) b++;
            return 0;
        }
       
        public static string CreateIniDisksPanels(string inipanel,string alffolder,string newinipanel)
        {
            if (!File.Exists(inipanel)) return "Ошибка CreateIniDisksPanels файл " + inipanel+" не найден";
            string[] res = File.ReadAllLines(inipanel);
            int b=res.Length;
            string[] DriveList = Environment.GetLogicalDrives();
            Array.Resize(ref res, res.Length+DriveList.Length);
            for (int a = b; a < res.Length; a++)
            {
                char c = (char)((int)(DriveList[a-b][0]) + 32);
                res[a] = alffolder + c.ToString() + ".gif&&" + DriveList[a - b];
            }
            File.WriteAllLines(newinipanel, res);
            return newinipanel;
        }
      
      
        //--- копирует директорию из одного места в другое -------------------------------------------------
        public static void CopyDir(string sourcePath, string targetPath) 
        {    
            DirectoryInfo dir_inf = new DirectoryInfo(sourcePath); 
            foreach (DirectoryInfo dir in dir_inf.GetDirectories())          
            {   
                if (Directory.Exists(Path.Combine(targetPath,dir.Name)) != true) Directory.CreateDirectory(targetPath+"\\"+dir.Name);     
                CopyDir(dir.FullName,Path.Combine(targetPath,dir.Name));    
            }    
            string[] files = System.IO.Directory.GetFiles(sourcePath);
            foreach (string s in files)
            {
                string fileName = System.IO.Path.GetFileName(s);
                string destFile = System.IO.Path.Combine(targetPath, fileName);
                File.Copy(s, destFile, true);
            }            
        }
        //--- копирует директорию из одного места в другое -------------------------------------------------


    }
}
