using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace VenLight.Utils
{
    static class StringFactory
    {
        //--- получает массив строк заданного уровня ------------------------------------------------------
        public static string[] GetMasStr(string[] str, int stringpos, int level)
        {
            if (str.Length < stringpos + 1) MessageBox.Show("Ошибка GetMasStr- мало строк в файле");
            string tstr;
            int temp;
            string oldchar = " ";
            string[] res = new string[str.Length - stringpos];
            for (int b = stringpos; b < str.Length; b++)
            {
                tstr = "";
                temp = 1;
                for (int a = 0; a < str[b].Length; a++)
                {
                    if ((temp == level) && (string.Compare(str[b][a].ToString(), "&") != 0)) tstr = tstr + str[b][a].ToString();
                    if (((string.Compare(str[b][a].ToString(), "&") == 0) && (string.Compare(oldchar, "&") == 0)) || (a == str[b].Length - 1))
                    {
                        if (temp == level) res[b - stringpos] = tstr;
                        temp++;
                    }
                    oldchar = str[b][a].ToString();
                }
            }
            return res;
        }

        //------прибавляет к каждой строке заданую строку ------------------------------------------------------
        public static string[] PasteSTR(string s, string[] str)
        {
            if ((str.Length < 1) || (s == "")) return str;
            for (int a = 0; a < str.Length; a++) str[a] = s + str[a];
            return str;
        }
        //-----------------------------------------------------------------------------------------------------
        //--- получает участок строки разделённой символами && ----------------------------------------------
        public static string FromString(string st, int level)
        {
            int teklevel = 1;
            string oldchar = " ";
            string res = "";
            for (int a = 0; a < st.Length; a++)
            {
                if (string.Compare(oldchar + st[a].ToString(), "&&") == 0) teklevel++;
                if (teklevel == level)
                    if ((a == st.Length - 1) || ((string.Compare(st[a].ToString(), "&") != 0))) res = res + st[a].ToString();
                oldchar = st[a].ToString();
            }
            return res;
        }
        //--- получает укороченное имя файла для отображения в проводнике ----------------------------------
        public static string GetShortStr(string longstr, bool e)
        {
            if ((e == false) || (longstr == null) || longstr.Length == 0) return longstr;
            string tmp = Path.GetFileNameWithoutExtension(longstr);
            string res = "";
            for (int a = 0; a < tmp.Length; a++) if (a < 16) res = res + tmp[a].ToString();
            if (tmp.Length >= 16) res = res + "...";
            return res;
        }
        //--------------------------------------------------------------------------------------------------
        public static string GetPartStringWithSeparator(string str, char separator, int nPos)
        {
            if ((str == null) || (str.Length == 0)) return "";
            string[] words = str.Split(separator);
            if ((nPos < 1) || (nPos > words.Length)) return "";
            return words[nPos - 1];
        }
        //--------------------------------------------------------------------------------------------------
    }
}
