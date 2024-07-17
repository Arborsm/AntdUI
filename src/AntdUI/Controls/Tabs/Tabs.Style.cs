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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace AntdUI
{
    partial class Tabs
    {
        /// <summary>
        /// 线条样式
        /// </summary>
        public class StyleLine : IStyle
        {
            Tabs? owner;
            public StyleLine() { }
            public StyleLine(Tabs tabs) { owner = tabs; }

            int size = 3;
            /// <summary>
            /// 条大小
            /// </summary>
            [Description("条大小"), Category("样式"), DefaultValue(3)]
            public int Size
            {
                get => size;
                set
                {
                    if (size == value) return;
                    size = value;
                    owner?.LoadLayout();
                }
            }

            int padding = 0;
            /// <summary>
            /// 条边距
            /// </summary>
            [Description("条边距"), Category("样式"), DefaultValue(0)]
            public int Padding
            {
                get => padding;
                set
                {
                    if (padding == value) return;
                    padding = value;
                    owner?.LoadLayout();
                }
            }

            int radius = 0;
            /// <summary>
            /// 条圆角
            /// </summary>
            [Description("条圆角"), Category("样式"), DefaultValue(0)]
            public int Radius
            {
                get => radius;
                set
                {
                    if (radius == value) return;
                    radius = value;
                    owner?.Invalidate();
                }
            }

            int backsize = 1;
            /// <summary>
            /// 条背景大小
            /// </summary>
            [Description("条背景大小"), Category("样式"), DefaultValue(1)]
            public int BackSize
            {
                get => backsize;
                set
                {
                    if (backsize == value) return;
                    backsize = value;
                    owner?.LoadLayout();
                }
            }

            /// <summary>
            /// 条背景
            /// </summary>
            [Description("条背景"), Category("样式"), DefaultValue(null)]
            [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
            public Color? Back { get; set; }

            Rectangle rect_line_top;
            public void LoadLayout(Tabs tabs, Rectangle rect, TabCollection items)
            {
                owner = tabs;
                Helper.GDI(g =>
                {
                    int gap = (int)(tabs.Gap * Config.Dpi), gapI = gap / 2, xy = 0, xy2 = 0;
                    int barSize = (int)(Size * Config.Dpi), barPadding = (int)(Padding * Config.Dpi), barPadding2 = barPadding * 2;
                    switch (tabs.Alignment)
                    {
                        case TabAlignment.Bottom:
                            foreach (var item in items)
                            {
                                var size = g.MeasureString(item.Text, tabs.Font).Size();
                                Rectangle rect_it;
                                if (item.IconSvg == null && item.Icon == null)
                                {
                                    rect_it = new Rectangle(rect.X + xy, rect.Y, size.Width + gap, xy2);
                                    item.Rect_Text = rect_it;
                                }
                                else
                                {
                                    int ico_size = (int)(size.Height * tabs.IconRatio);
                                    rect_it = new Rectangle(rect.X + xy, rect.Y, size.Width + gap + ico_size + gap, xy2);
                                    item.Rect_Text = new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, size.Width + gap, rect_it.Height);
                                    item.Rect_Ico = new Rectangle(rect_it.X + gapI, rect_it.Y, ico_size, ico_size);
                                }
                                item.SetRect(rect_it);
                                int h = size.Height + gap;
                                if (xy2 < h) xy2 = h;
                                xy += rect_it.Width;
                            }
                            tabs.SetPadding(0, 0, 0, xy2);
                            foreach (var item in items)
                            {
                                var rect_it = item.SetRectH(rect.Bottom - xy2, xy2);
                                item.Rect_Line = new Rectangle(rect_it.X + barPadding, rect_it.Y, rect_it.Width - barPadding2, barSize);
                            }
                            if (BackSize > 0)
                            {
                                int barBackSize = (int)(BackSize * Config.Dpi);
                                rect_line_top = new Rectangle(rect.X, rect.Bottom - xy2, rect.Width, barBackSize);
                            }
                            break;
                        case TabAlignment.Left:
                            foreach (var item in items)
                            {
                                var size = g.MeasureString(item.Text, tabs.Font).Size();
                                var rect_it = new Rectangle(rect.X, rect.Y + xy, xy2, size.Height + gap);
                                item.Rect_Text = rect_it;
                                int w = size.Width + gap;
                                if (xy2 < w) xy2 = w;
                                item.SetRect(rect_it);
                                xy += rect_it.Height;
                            }
                            tabs.SetPadding(xy2, 0, 0, 0);
                            foreach (var item in items)
                            {
                                var rect_it = item.SetRectW(xy2);
                                item.Rect_Line = new Rectangle(rect_it.X + xy2 - barSize, rect_it.Y + barPadding, barSize, rect_it.Height - barPadding2);
                            }
                            if (BackSize > 0)
                            {
                                int barBackSize = (int)(BackSize * Config.Dpi);
                                rect_line_top = new Rectangle(rect.X + xy2, rect.Y, barBackSize, rect.Height);
                            }
                            break;
                        case TabAlignment.Right:
                            foreach (var item in items)
                            {
                                var size = g.MeasureString(item.Text, tabs.Font).Size();
                                var rect_it = new Rectangle(rect.X, rect.Y + xy, xy2, size.Height + gap);
                                item.Rect_Text = rect_it;
                                item.SetRect(rect_it);
                                int w = size.Width + gap;
                                if (xy2 < w) xy2 = w;
                                xy += rect_it.Height;
                            }
                            tabs.SetPadding(0, 0, xy2, 0);
                            foreach (var item in items)
                            {
                                var rect_it = item.SetRectW(rect.Right - xy2, xy2);
                                item.Rect_Line = new Rectangle(rect_it.X, rect_it.Y + barPadding, barSize, rect_it.Height - barPadding2);
                            }
                            if (BackSize > 0)
                            {
                                int barBackSize = (int)(BackSize * Config.Dpi);
                                rect_line_top = new Rectangle(rect.Right - xy2, rect.Y, barBackSize, rect.Height);
                            }
                            break;
                        case TabAlignment.Top:
                        default:
                            foreach (var item in items)
                            {
                                var size = g.MeasureString(item.Text, tabs.Font).Size();
                                Rectangle rect_it;
                                if (item.IconSvg == null && item.Icon == null)
                                {
                                    rect_it = new Rectangle(rect.X + xy, rect.Y, size.Width + gap, xy2);
                                    item.Rect_Text = rect_it;
                                }
                                else
                                {
                                    int ico_size = (int)(size.Height * tabs.IconRatio);
                                    rect_it = new Rectangle(rect.X + xy, rect.Y, size.Width + gap + ico_size + gap, xy2);
                                    item.Rect_Text = new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, size.Width + gap, rect_it.Height);
                                    item.Rect_Ico = new Rectangle(rect_it.X + gapI, rect_it.Y, ico_size, ico_size);
                                }
                                item.SetRect(rect_it);
                                int h = size.Height + gap;
                                if (xy2 < h) xy2 = h;
                                xy += rect_it.Width;
                            }
                            tabs.SetPadding(0, xy2, 0, 0);
                            foreach (var item in items)
                            {
                                var rect_it = item.SetRectH(xy2);
                                item.Rect_Line = new Rectangle(rect_it.X + barPadding, rect_it.Bottom - barSize, rect_it.Width - barPadding2, barSize);
                            }
                            if (BackSize > 0)
                            {
                                int barBackSize = (int)(BackSize * Config.Dpi);
                                rect_line_top = new Rectangle(rect.Left, rect.Y + xy2 - barBackSize, rect.Width, barBackSize);
                            }
                            break;
                    }
                });
            }

            public void Paint(Tabs owner, Graphics g, TabCollection items)
            {
                if (BackSize > 0)
                {
                    using (var brush = new SolidBrush(Back ?? AntdUI.Style.Db.BorderSecondary))
                    {
                        g.FillRectangle(brush, rect_line_top);
                    }
                }
                using (var brush_fore = new SolidBrush(owner.ForeColor ?? AntdUI.Style.Db.Text))
                using (var brush_fill = new SolidBrush(owner.Fill ?? AntdUI.Style.Db.Primary))
                using (var brush_active = new SolidBrush(owner.FillActive ?? AntdUI.Style.Db.PrimaryActive))
                using (var brush_hover = new SolidBrush(owner.FillHover ?? AntdUI.Style.Db.PrimaryHover))
                {
                    if (AnimationBar)
                    {
                        PaintBar(g, AnimationBarValue, brush_fill);
                        int i = 0;
                        foreach (var page in items)
                        {
                            if (owner.SelectedIndex == i) PaintText(g, owner, page, brush_fill);
                            else if (owner.hover_i == i) PaintText(g, owner, page, brush_hover);
                            else PaintText(g, owner, page, brush_fore);
                            i++;
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (var page in items)
                        {
                            if (owner.SelectedIndex == i)//是否选中
                            {
                                PaintBar(g, page.Rect_Line, brush_fill);
                                PaintText(g, owner, page, brush_fill);
                            }
                            else if (owner.hover_i == i) PaintText(g, owner, page, page.MDown ? brush_active : brush_hover);
                            else PaintText(g, owner, page, brush_fore);
                            i++;
                        }
                    }
                }
            }

            #region 辅助

            void PaintText(Graphics g, Tabs owner, TabPage page, SolidBrush brush)
            {
                if (page.Icon != null) g.DrawImage(page.Icon, page.Rect_Ico);
                else if (page.IconSvg != null)
                {
                    using (var bmp = SvgExtend.GetImgExtend(page.IconSvg, page.Rect_Ico, brush.Color))
                    {
                        if (bmp != null) g.DrawImage(bmp, page.Rect_Ico);
                    }
                }
                g.DrawString(page.Text, owner.Font, brush, page.Rect_Text, owner.s_c);
            }
            void PaintBar(Graphics g, RectangleF rect, SolidBrush brush)
            {
                if (Radius > 0)
                {
                    using (var path = rect.RoundPath(Radius * Config.Dpi))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else g.FillRectangle(brush, rect);
            }
            void PaintBar(Graphics g, Rectangle rect, SolidBrush brush)
            {
                if (Radius > 0)
                {
                    using (var path = rect.RoundPath(Radius * Config.Dpi))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else g.FillRectangle(brush, rect);
            }

            #endregion

            #region 动画

            bool AnimationBar = false;
            RectangleF AnimationBarValue;
            ITask? ThreadBar = null;

            void SetRect(int old, int value)
            {
                if (owner == null) return;
                if (value > -1)
                {
                    if (owner.items == null) return;
                    if (old > -1 && owner.items.Count > old)
                    {
                        ThreadBar?.Dispose();
                        Helper.GDI(g =>
                        {
                            RectangleF OldValue = owner.items[old].Rect_Line, NewValue = owner.items[value].Rect_Line;
                            if (AnimationBarValue.Height == 0) AnimationBarValue = OldValue;
                            if (Config.Animation)
                            {
                                if (owner.alignment == TabAlignment.Left || owner.alignment == TabAlignment.Right)
                                {
                                    if (OldValue.X == NewValue.X)
                                    {
                                        AnimationBarValue.X = OldValue.X;
                                        AnimationBar = true;
                                        float p_val = Math.Abs(NewValue.Y - AnimationBarValue.Y) * 0.09F, p_w_val = Math.Abs(NewValue.Height - AnimationBarValue.Height) * 0.1F, p_val2 = (NewValue.Y - AnimationBarValue.Y) * 0.5F;
                                        ThreadBar = new ITask(owner, () =>
                                        {
                                            if (AnimationBarValue.Height != NewValue.Height)
                                            {
                                                if (NewValue.Height > OldValue.Height)
                                                {
                                                    AnimationBarValue.Height += p_w_val;
                                                    if (AnimationBarValue.Height > NewValue.Height) AnimationBarValue.Height = NewValue.Height;
                                                }
                                                else
                                                {
                                                    AnimationBarValue.Height -= p_w_val;
                                                    if (AnimationBarValue.Height < NewValue.Height) AnimationBarValue.Height = NewValue.Height;
                                                }
                                            }
                                            if (NewValue.Y > OldValue.Y)
                                            {
                                                if (AnimationBarValue.Y > p_val2) AnimationBarValue.Y += p_val / 2F;
                                                else AnimationBarValue.Y += p_val;
                                                if (AnimationBarValue.Y > NewValue.Y)
                                                {
                                                    AnimationBarValue.Y = NewValue.Y;
                                                    owner.Invalidate();
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                AnimationBarValue.Y -= p_val;
                                                if (AnimationBarValue.Y < NewValue.Y)
                                                {
                                                    AnimationBarValue.Y = NewValue.Y;
                                                    owner.Invalidate();
                                                    return false;
                                                }
                                            }
                                            owner.Invalidate();
                                            return true;
                                        }, 10, () =>
                                        {
                                            AnimationBarValue = NewValue;
                                            AnimationBar = false;
                                            owner.Invalidate();
                                        });
                                        return;
                                    }
                                }
                                else
                                {
                                    if (OldValue.Y == NewValue.Y)
                                    {
                                        AnimationBarValue.Y = OldValue.Y;
                                        AnimationBar = true;
                                        float p_val = Math.Abs(NewValue.X - AnimationBarValue.X) * 0.09F, p_w_val = Math.Abs(NewValue.Width - AnimationBarValue.Width) * 0.1F, p_val2 = (NewValue.X - AnimationBarValue.X) * 0.5F;
                                        ThreadBar = new ITask(owner, () =>
                                        {
                                            if (AnimationBarValue.Width != NewValue.Width)
                                            {
                                                if (NewValue.Width > OldValue.Width)
                                                {
                                                    AnimationBarValue.Width += p_w_val;
                                                    if (AnimationBarValue.Width > NewValue.Width) AnimationBarValue.Width = NewValue.Width;
                                                }
                                                else
                                                {
                                                    AnimationBarValue.Width -= p_w_val;
                                                    if (AnimationBarValue.Width < NewValue.Width) AnimationBarValue.Width = NewValue.Width;
                                                }
                                            }
                                            if (NewValue.X > OldValue.X)
                                            {
                                                if (AnimationBarValue.X > p_val2) AnimationBarValue.X += p_val / 2F;
                                                else AnimationBarValue.X += p_val;
                                                if (AnimationBarValue.X > NewValue.X)
                                                {
                                                    AnimationBarValue.X = NewValue.X;
                                                    owner.Invalidate();
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                AnimationBarValue.X -= p_val;
                                                if (AnimationBarValue.X < NewValue.X)
                                                {
                                                    AnimationBarValue.X = NewValue.X;
                                                    owner.Invalidate();
                                                    return false;
                                                }
                                            }
                                            owner.Invalidate();
                                            return true;
                                        }, 10, () =>
                                        {
                                            AnimationBarValue = NewValue;
                                            AnimationBar = false;
                                            owner.Invalidate();
                                        });
                                        return;
                                    }
                                }
                            }
                            AnimationBarValue = NewValue;
                            owner.Invalidate();
                        });
                    }
                    else AnimationBarValue = owner.items[value].Rect_Line;
                }
            }

            #endregion

            public void SelectedIndexChanged(int i, int old)
            {
                SetRect(old, i);
            }
            public void Dispose()
            {
                ThreadBar?.Dispose();
            }
        }

        /// <summary>
        /// 卡片样式
        /// </summary>
        public class StyleCard : IStyle
        {
            Tabs? owner;
            public StyleCard() { }
            public StyleCard(Tabs tabs) { owner = tabs; }

            int radius = 6;
            /// <summary>
            /// 卡片圆角
            /// </summary>
            [Description("卡片圆角"), Category("样式"), DefaultValue(6)]
            public int Radius
            {
                get => radius;
                set
                {
                    if (radius == value) return;
                    radius = value;
                    owner?.Invalidate();
                }
            }

            Color? border;
            /// <summary>
            /// 卡片边框颜色
            /// </summary>
            [Description("卡片边框颜色"), Category("卡片"), DefaultValue(null)]
            [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
            public Color? Border
            {
                get => border;
                set
                {
                    if (border == value) return;
                    border = value;
                    owner?.Invalidate();
                }
            }

            int gap = 2;
            /// <summary>
            /// 卡片间距
            /// </summary>
            [Description("卡片间距"), Category("卡片"), DefaultValue(2)]
            public int Gap
            {
                get => gap;
                set
                {
                    if (gap == value) return;
                    gap = value;
                    owner?.LoadLayout();
                }
            }

            public void LoadLayout(Tabs tabs, Rectangle rect, TabCollection items)
            {
                owner = tabs;
                Helper.GDI(g =>
                {
                    int gap = (int)(tabs.Gap * Config.Dpi), gapI = gap / 2, xy = 0, xy2 = 0;
                    int cardgap = (int)(Gap * Config.Dpi);
                    switch (tabs.Alignment)
                    {
                        case TabAlignment.Bottom:
                            foreach (var item in items)
                            {
                                var size = g.MeasureString(item.Text, tabs.Font).Size();
                                var rect_it = new Rectangle(rect.X + xy, rect.Y, size.Width + gap, xy2);
                                item.SetRect(rect_it);
                                item.Rect_Text = rect_it;
                                int h = size.Height + gap;
                                if (xy2 < h) xy2 = h;
                                xy += rect_it.Width + cardgap;
                            }
                            tabs.SetPadding(0, 0, 0, xy2);
                            foreach (var item in items) item.SetRectH(rect.Bottom - xy2, xy2);
                            break;
                        case TabAlignment.Left:
                            foreach (var item in items)
                            {
                                var size = g.MeasureString(item.Text, tabs.Font).Size();
                                var rect_it = new Rectangle(rect.X, rect.Y + xy, xy2, size.Height + gap);
                                item.SetRect(rect_it);
                                item.Rect_Text = rect_it;
                                int w = size.Width + gap;
                                if (xy2 < w) xy2 = w;
                                xy += rect_it.Height + cardgap;
                            }
                            tabs.SetPadding(xy2, 0, 0, 0);
                            foreach (var item in items) item.SetRectW(xy2);
                            break;
                        case TabAlignment.Right:
                            foreach (var item in items)
                            {
                                var size = g.MeasureString(item.Text, tabs.Font).Size();
                                var rect_it = new Rectangle(rect.X, rect.Y + xy, xy2, size.Height + gap);
                                item.SetRect(rect_it);
                                item.Rect_Text = rect_it;
                                int w = size.Width + gap;
                                if (xy2 < w) xy2 = w;
                                xy += rect_it.Height + cardgap;
                            }
                            tabs.SetPadding(0, 0, xy2, 0);
                            foreach (var item in items) item.SetRectW(rect.Right - xy2, xy2);
                            break;
                        case TabAlignment.Top:
                        default:
                            foreach (var item in items)
                            {
                                var size = g.MeasureString(item.Text, tabs.Font).Size();
                                var rect_it = new Rectangle(rect.X + xy, rect.Y, size.Width + gap, xy2);
                                item.SetRect(rect_it);
                                item.Rect_Text = rect_it;
                                int h = size.Height + gap;
                                if (xy2 < h) xy2 = h;
                                xy += rect_it.Width + cardgap;
                            }
                            tabs.SetPadding(0, xy2, 0, 0);
                            foreach (var item in items) item.SetRectH(xy2);
                            break;
                    }
                });
            }

            public void Paint(Tabs owner, Graphics g, TabCollection items)
            {
                using (var brush_fore = new SolidBrush(owner.ForeColor ?? AntdUI.Style.Db.Text))
                using (var brush_fill = new SolidBrush(owner.Fill ?? AntdUI.Style.Db.Primary))
                using (var brush_active = new SolidBrush(owner.FillActive ?? AntdUI.Style.Db.PrimaryActive))
                using (var brush_hover = new SolidBrush(owner.FillHover ?? AntdUI.Style.Db.PrimaryHover))
                {
                    var rect_t = owner.ClientRectangle;
                    int radius = (int)(Radius * Config.Dpi), sp = (int)(1F * Config.Dpi), sp2 = sp * 6, sp22 = sp2 * 2;

                    using (var brush_bg = new SolidBrush(owner.Fill ?? AntdUI.Style.Db.FillQuaternary))
                    {
                        TabPage? sel = null;
                        int i = 0;
                        switch (owner.Alignment)
                        {
                            case TabAlignment.Bottom:
                                foreach (var page in items)
                                {
                                    if (owner.SelectedIndex == i) sel = page;
                                    else
                                    {
                                        using (var path = Helper.RoundPath(page.Rect, radius, false, false, true, true))
                                        {
                                            g.FillPath(brush_bg, path);
                                            using (var pen_bg = new Pen(AntdUI.Style.Db.BorderSecondary, sp))
                                            {
                                                g.DrawPath(pen_bg, path);
                                            }
                                            if (owner.hover_i == i) g.DrawString(page.Text, owner.Font, brush_hover, page.Rect_Text, owner.s_c);
                                            else g.DrawString(page.Text, owner.Font, brush_fore, page.Rect_Text, owner.s_c);
                                        }
                                    }
                                    i++;
                                }
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, false, false, true, true))
                                    {
                                        using (var brush_bgw = new SolidBrush(AntdUI.Style.Db.BgContainer))
                                        {
                                            using (var pen_bg = new Pen(Border ?? AntdUI.Style.Db.BorderColor, sp))
                                            {
                                                int ly = rect_page.Y + sp;
                                                g.DrawLine(pen_bg, rect_t.X, ly, rect_t.Right, ly);
                                                g.FillPath(brush_bgw, path);
                                                g.SetClip(new Rectangle(rect_page.X - sp2, rect_page.Y + sp, rect_page.Width + sp22, rect_page.Height + sp22));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        g.DrawString(sel.Text, owner.Font, brush_fill, rect_page, owner.s_c);
                                    }
                                }
                                break;
                            case TabAlignment.Left:
                                foreach (var page in items)
                                {
                                    if (owner.SelectedIndex == i) sel = page;
                                    else
                                    {
                                        using (var path = Helper.RoundPath(page.Rect, radius, true, false, false, true))
                                        {
                                            g.FillPath(brush_bg, path);
                                            using (var pen_bg = new Pen(AntdUI.Style.Db.BorderSecondary, sp))
                                            {
                                                g.DrawPath(pen_bg, path);
                                            }
                                            if (owner.hover_i == i) g.DrawString(page.Text, owner.Font, brush_hover, page.Rect_Text, owner.s_c);
                                            else g.DrawString(page.Text, owner.Font, brush_fore, page.Rect_Text, owner.s_c);
                                        }
                                    }
                                    i++;
                                }
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, true, false, false, true))
                                    {
                                        using (var brush_bgw = new SolidBrush(AntdUI.Style.Db.BgContainer))
                                        {
                                            using (var pen_bg = new Pen(Border ?? AntdUI.Style.Db.BorderColor, sp))
                                            {
                                                int lx = rect_page.Right - sp;
                                                g.DrawLine(pen_bg, lx, rect_t.Y, lx, rect_t.Bottom);
                                                g.FillPath(brush_bgw, path);
                                                g.SetClip(new Rectangle(rect_page.X - sp2, rect_page.Y - sp2, rect_page.Width + sp2 - sp, rect_page.Height + sp22));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        g.DrawString(sel.Text, owner.Font, brush_fill, rect_page, owner.s_c);
                                    }
                                }
                                break;
                            case TabAlignment.Right:
                                foreach (var page in items)
                                {
                                    if (owner.SelectedIndex == i) sel = page;
                                    else
                                    {
                                        using (var path = Helper.RoundPath(page.Rect, radius, false, true, true, false))
                                        {
                                            g.FillPath(brush_bg, path);
                                            using (var pen_bg = new Pen(AntdUI.Style.Db.BorderSecondary, sp))
                                            {
                                                g.DrawPath(pen_bg, path);
                                            }
                                            if (owner.hover_i == i) g.DrawString(page.Text, owner.Font, brush_hover, page.Rect_Text, owner.s_c);
                                            else g.DrawString(page.Text, owner.Font, brush_fore, page.Rect_Text, owner.s_c);
                                        }
                                    }
                                    i++;
                                }
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, false, true, true, false))
                                    {
                                        using (var brush_bgw = new SolidBrush(AntdUI.Style.Db.BgContainer))
                                        {
                                            using (var pen_bg = new Pen(Border ?? AntdUI.Style.Db.BorderColor, sp))
                                            {
                                                int lx = rect_page.X + sp;
                                                g.DrawLine(pen_bg, lx, rect_t.Y, lx, rect_t.Bottom);
                                                g.FillPath(brush_bgw, path);
                                                g.SetClip(new Rectangle(rect_page.X + sp, rect_page.Y - sp2, rect_page.Width + sp22, rect_page.Height + sp22));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        g.DrawString(sel.Text, owner.Font, brush_fill, rect_page, owner.s_c);
                                    }
                                }
                                break;
                            case TabAlignment.Top:
                            default:
                                foreach (var page in items)
                                {
                                    if (owner.SelectedIndex == i) sel = page;
                                    else
                                    {
                                        using (var path = Helper.RoundPath(page.Rect, radius, true, true, false, false))
                                        {
                                            g.FillPath(brush_bg, path);
                                            using (var pen_bg = new Pen(AntdUI.Style.Db.BorderSecondary, sp))
                                            {
                                                g.DrawPath(pen_bg, path);
                                            }
                                            if (owner.hover_i == i) g.DrawString(page.Text, owner.Font, brush_hover, page.Rect_Text, owner.s_c);
                                            else g.DrawString(page.Text, owner.Font, brush_fore, page.Rect_Text, owner.s_c);
                                        }
                                    }
                                    i++;
                                }
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, true, true, false, false))
                                    {
                                        using (var brush_bgw = new SolidBrush(AntdUI.Style.Db.BgContainer))
                                        {
                                            using (var pen_bg = new Pen(Border ?? AntdUI.Style.Db.BorderColor, sp))
                                            {
                                                int ly = rect_page.Bottom - sp;
                                                g.DrawLine(pen_bg, rect_t.X, ly, rect_t.Right, ly);
                                                g.FillPath(brush_bgw, path);
                                                g.SetClip(new Rectangle(rect_page.X - sp2, rect_page.Y - sp2, rect_page.Width + sp22, rect_page.Height + sp2 - sp));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        g.DrawString(sel.Text, owner.Font, brush_fill, sel.Rect_Text, owner.s_c);
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            public void SelectedIndexChanged(int i, int old)
            {
                owner?.Invalidate();
            }

            public void Dispose()
            {

            }
        }


        [TypeConverter(typeof(ExpandableObjectConverter))]
        public interface IStyle
        {
            void LoadLayout(Tabs owner, Rectangle rect, TabCollection items);
            void Paint(Tabs owner, Graphics g, TabCollection items);
            void SelectedIndexChanged(int i, int old);
            void Dispose();
        }
    }
}