// COPYRIGHT (C) Tom. ALL RIGHTS RESERVED.
// THE AntdUI PROJECT IS AN WINFORM LIBRARY LICENSED UNDER THE Apache-2.0 License.
// LICENSED UNDER THE Apache License, VERSION 2.0 (THE "License")
// YOU MAY NOT USE THIS FILE EXCEPT IN COMPLIANCE WITH THE License.
// YOU MAY OBTAIN A COPY OF THE LICENSE AT
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE
// DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED.
// SEE THE LICENSE FOR THE SPECIFIC LANGUAGE GOVERNING PERMISSIONS AND
// LIMITATIONS UNDER THE License.
// GITEE: https://gitee.com/antdui/AntdUI
// GITHUB: https://github.com/AntdUI/AntdUI
// CSDN: https://blog.csdn.net/v_132
// QQ: 17379620

using System;
using System.Windows.Forms;

namespace Overview
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arge)
        {
            bool istop = false;
            if (arge.Length > 0 && arge[0] == "t") istop = true;
            //AntdUI.Localization.Provider = new Localizer();
            AntdUI.Config.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            AntdUI.Config.SetCorrectionTextRendering("Microsoft YaHei UI");
            Application.Run(new Main(istop));
        }
    }
    public class Localizer : AntdUI.ILocalization
    {
        public string GetLocalizedString(string key)
        {
            switch (key)
            {
                case "Cancel":
                    return "キャンセル";
                case "OK":
                    return "�_�J";
                case "Now":
                    return "書は";
                case "ToDay":
                    return "書晩";
                case "NoData":
                    return "デ�`タが匯�r議にありません";
                case "Year":
                    return "Ann└e";
                case "Month":
                    return "Mois";

                case "Mon":
                    return "Mon";
                case "Tue":
                    return "Tue";
                case "Wed":
                    return "Wed";
                case "Thu":
                    return "Thu";
                case "Fri":
                    return "Fri";
                case "Sat":
                    return "Sat";
                case "Sun":
                    return "Sun";
                case "ItemsPerPage":
                    return "Per/Page";

                default:
                    System.Diagnostics.Debug.WriteLine(key);
                    return key;
            }
        }
    }
}