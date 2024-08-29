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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Overview.Controls
{
    public partial class Icon : UserControl
    {
        Form form;
        public Icon(Form _form)
        {
            form = _form;
            InitializeComponent();
            LoadData();
        }

        private void segmented1_SelectIndexChanged(object sender, AntdUI.IntEventArgs e)
        {
            LoadData();
        }

        void LoadData()
        {
            var data = GetData();
            var svgs = new List<VItem>(data.Count);
            foreach (var it in data) svgs.Add(new VItem(it.Key, it.Value));
            vpanel.Items.Clear();
            txt_search.Text = "";
            vpanel.Items.AddRange(svgs);
        }

        Dictionary<string, string> GetData()
        {
            var svgs = new Dictionary<string, string>(AntdUI.SvgDb.Custom.Count);
            if (segmented1.SelectIndex == 0)
            {
                foreach (var it in AntdUI.SvgDb.Custom)
                {
                    if (it.Key == "StepBackwardFilled") return svgs;
                    svgs.Add(it.Key, it.Value);
                }
            }
            else
            {
                bool isadd = false;
                foreach (var it in AntdUI.SvgDb.Custom)
                {
                    if (it.Key == "UpCircleTwoTone") return svgs;
                    else if (it.Key == "StepBackwardFilled") isadd = true;
                    if (isadd) svgs.Add(it.Key, it.Value);
                }
            }
            return svgs;
        }

        class VItem : AntdUI.VirtualItem
        {
            public string Key, Value;
            public VItem(string key, string value) { Tag = Key = key; Value = value; }

            StringFormat s_f = AntdUI.Helper.SF_NoWrap();
            Bitmap bmp = null, bmp_ac = null;
            public override void Paint(Graphics g, AntdUI.VirtualPanelArgs e)
            {
                var dpi = AntdUI.Config.Dpi;
                int icon_size = (int)(36 * dpi), text_size = (int)(24 * dpi), y = e.Rect.Y + (e.Rect.Height - (icon_size + text_size)) / 2;
                var rect_icon = new Rectangle(e.Rect.X + (e.Rect.Width - icon_size) / 2, y, icon_size, icon_size);
                var rect_text = new Rectangle(e.Rect.X, y + icon_size / 2 + text_size, e.Rect.Width, text_size);
                if (Hover)
                {
                    using (var path = AntdUI.Helper.RoundPath(e.Rect, e.Radius))
                    {
                        using (var brush = new SolidBrush(AntdUI.Style.Db.Primary))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    if (bmp_ac == null) bmp_ac = AntdUI.SvgExtend.SvgToBmp(Value, icon_size, icon_size, AntdUI.Style.Db.PrimaryColor);
                    g.DrawImage(bmp_ac, rect_icon);

                    using (var fore = new SolidBrush(AntdUI.Style.Db.PrimaryColor))
                    {
                        AntdUI.CorrectionTextRendering.DrawStr(g, Key, e.Panel.Font, fore, rect_text, s_f);
                    }
                }
                else
                {
                    if (bmp == null) bmp = AntdUI.SvgExtend.SvgToBmp(Value, icon_size, icon_size, AntdUI.Style.Db.Text);
                    g.DrawImage(bmp, rect_icon);
                    using (var fore = new SolidBrush(AntdUI.Style.Db.Text))
                    {
                        AntdUI.CorrectionTextRendering.DrawStr(g, Key, e.Panel.Font, fore, rect_text, s_f);
                    }
                }

            }
            public override Size Size(Graphics g, AntdUI.VirtualPanelArgs e)
            {
                var dpi = AntdUI.Config.Dpi;
                return new Size((int)(200 * dpi), (int)(100 * dpi));
            }
        }

        private void vpanel_ItemClick(object sender, AntdUI.VirtualItemEventArgs e)
        {
            Clipboard.SetText(e.Item.Tag.ToString());
        }

        private void txt_search_TextChanged(object sender, System.EventArgs e) => LoadSearchList();
        private void txt_search_SuffixClick(object sender, MouseEventArgs e) => LoadSearchList();

        void LoadSearchList()
        {
            string search = txt_search.Text;
            BeginInvoke(new Action(() =>
            {
                vpanel.PauseLayout = true;
                if (string.IsNullOrEmpty(search))
                {
                    foreach (var it in vpanel.Items) it.Visible = true;
                }
                else
                {
                    string searchLower = search.ToLower();
                    foreach (VItem it in vpanel.Items) it.Visible = it.Key.ToLower().Contains(search);
                }
                vpanel.PauseLayout = false;
            }));
        }
    }
}