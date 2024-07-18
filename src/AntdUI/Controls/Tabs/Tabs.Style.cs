﻿// COPYRIGHT (C) Tom. ALL RIGHTS RESERVED.
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

            int padding = 8;
            /// <summary>
            /// 条边距
            /// </summary>
            [Description("条边距"), Category("样式"), DefaultValue(8)]
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
            Rectangle[][] rects = new Rectangle[0][];
            public void LoadLayout(Tabs tabs, Rectangle rect, TabCollection items)
            {
                owner = tabs;
                rects = Helper.GDI(g =>
                {
                    int gap = (int)(tabs.Gap * Config.Dpi), gapI = gap / 2, xy = 0, xy2 = 0;
                    int barSize = (int)(Size * Config.Dpi), barPadding = (int)(Padding * Config.Dpi), barPadding2 = barPadding * 2;
                    var rect_list = new List<Rectangle[]>(items.Count);
                    var rect_dir = GetDir(tabs, g, items, gap, out int ico_size, out xy2);
                    switch (tabs.Alignment)
                    {
                        case TabAlignment.Bottom:
                            int y = rect.Bottom - xy2;
                            foreach (var it in rect_dir)
                            {
                                Rectangle rect_it;
                                if (it.Key.HasIcon)
                                {
                                    rect_it = new Rectangle(rect.X + xy, y, it.Value.Width + gap + ico_size + gap, xy2);
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + barPadding, rect_it.Y, rect_it.Width - barPadding2, barSize),
                                        new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, it.Value.Width + gap, rect_it.Height),
                                        new Rectangle(rect_it.X + gapI, rect_it.Y + (rect_it.Height - ico_size) / 2, ico_size, ico_size)
                                    });
                                }
                                else
                                {
                                    rect_it = new Rectangle(rect.X + xy, y, it.Value.Width + gap, xy2);
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + barPadding, rect_it.Y, rect_it.Width - barPadding2, barSize)
                                    });
                                }
                                it.Key.SetRect(rect_it);
                                xy += rect_it.Width;
                            }
                            tabs.SetPadding(0, 0, 0, xy2);
                            if (BackSize > 0)
                            {
                                int barBackSize = (int)(BackSize * Config.Dpi);
                                rect_line_top = new Rectangle(rect.X, rect.Bottom - xy2, rect.Width, barBackSize);
                            }
                            break;
                        case TabAlignment.Left:
                            foreach (var it in rect_dir)
                            {
                                Rectangle rect_it = new Rectangle(rect.X, rect.Y + xy, xy2, it.Value.Height + gap);
                                if (it.Key.HasIcon)
                                {
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + xy2 - barSize, rect_it.Y + barPadding, barSize, rect_it.Height - barPadding2),
                                        new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, it.Value.Width + gap, rect_it.Height),
                                        new Rectangle(rect_it.X + gapI, rect_it.Y + (rect_it.Height - ico_size) / 2, ico_size, ico_size)
                                    });
                                }
                                else
                                {
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + xy2 - barSize, rect_it.Y + barPadding, barSize, rect_it.Height - barPadding2)
                                    });
                                }

                                it.Key.SetRect(rect_it);
                                xy += rect_it.Height;
                            }
                            tabs.SetPadding(xy2, 0, 0, 0);
                            if (BackSize > 0)
                            {
                                int barBackSize = (int)(BackSize * Config.Dpi);
                                rect_line_top = new Rectangle(rect.X + xy2 - barBackSize, rect.Y, barBackSize, rect.Height);
                            }
                            break;
                        case TabAlignment.Right:
                            int x = rect.Right - xy2;
                            foreach (var it in rect_dir)
                            {
                                Rectangle rect_it = new Rectangle(x, rect.Y + xy, xy2, it.Value.Height + gap);
                                if (it.Key.HasIcon)
                                {
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X, rect_it.Y + barPadding, barSize, rect_it.Height - barPadding2),
                                        new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, it.Value.Width + gap, rect_it.Height),
                                        new Rectangle(rect_it.X + gapI, rect_it.Y + (rect_it.Height - ico_size) / 2, ico_size, ico_size)
                                    });
                                }
                                else
                                {
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X, rect_it.Y + barPadding, barSize, rect_it.Height - barPadding2)
                                    });
                                }

                                it.Key.SetRect(rect_it);
                                xy += rect_it.Height;
                            }
                            tabs.SetPadding(0, 0, xy2, 0);
                            if (BackSize > 0)
                            {
                                int barBackSize = (int)(BackSize * Config.Dpi);
                                rect_line_top = new Rectangle(x, rect.Y, barBackSize, rect.Height);
                            }
                            break;
                        case TabAlignment.Top:
                        default:
                            foreach (var it in rect_dir)
                            {
                                Rectangle rect_it;
                                if (it.Key.HasIcon)
                                {
                                    rect_it = new Rectangle(rect.X + xy, rect.Y, it.Value.Width + gap + ico_size + gap, xy2);
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + barPadding, rect_it.Bottom - barSize, rect_it.Width - barPadding2, barSize),
                                        new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, it.Value.Width + gap, rect_it.Height),
                                        new Rectangle(rect_it.X + gapI, rect_it.Y + (rect_it.Height - ico_size) / 2, ico_size, ico_size)
                                    });
                                }
                                else
                                {
                                    rect_it = new Rectangle(rect.X + xy, rect.Y, it.Value.Width + gap, xy2);
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + barPadding, rect_it.Bottom - barSize, rect_it.Width - barPadding2, barSize)
                                    });
                                }
                                it.Key.SetRect(rect_it);
                                xy += rect_it.Width;
                            }
                            tabs.SetPadding(0, xy2, 0, 0);
                            if (BackSize > 0)
                            {
                                int barBackSize = (int)(BackSize * Config.Dpi);
                                rect_line_top = new Rectangle(rect.Left, rect.Y + xy2 - barBackSize, rect.Width, barBackSize);
                            }
                            break;
                    }
                    return rect_list.ToArray();
                });
            }

            public void Paint(Tabs owner, Graphics g, TabCollection items)
            {
                if (rects.Length == items.Count)
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
                                if (owner.SelectedIndex == i) PaintText(g, rects[i], owner, page, brush_fill);
                                else if (owner.hover_i == i) PaintText(g, rects[i], owner, page, brush_hover);
                                else PaintText(g, rects[i], owner, page, brush_fore);
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
                                    PaintBar(g, rects[i][1], brush_fill);
                                    PaintText(g, rects[i], owner, page, brush_fill);
                                }
                                else if (owner.hover_i == i) PaintText(g, rects[i], owner, page, page.MDown ? brush_active : brush_hover);
                                else PaintText(g, rects[i], owner, page, brush_fore);
                                i++;
                            }
                        }
                    }
                }
            }

            #region 辅助

            Dictionary<TabPage, Size> GetDir(Tabs owner, Graphics g, TabCollection items, int gap, out int ico_size, out int sizewh)
            {
                sizewh = 0;
                var size_t = g.MeasureString(Config.NullText, owner.Font).Size();
                var rect_dir = new Dictionary<TabPage, Size>(items.Count);
                foreach (var item in items)
                {
                    var size = g.MeasureString(item.Text, owner.Font).Size();
                    rect_dir.Add(item, size);
                }
                ico_size = (int)(size_t.Height * owner.IconRatio);
                switch (owner.Alignment)
                {
                    case TabAlignment.Left:
                    case TabAlignment.Right:
                        foreach (var item in rect_dir)
                        {
                            int w;
                            if (item.Key.HasIcon) w = item.Value.Width + ico_size + gap * 2;
                            else w = item.Value.Width + gap;
                            if (sizewh < w) sizewh = w;
                        }
                        break;
                    case TabAlignment.Top:
                    case TabAlignment.Bottom:
                    default:
                        foreach (var item in rect_dir)
                        {
                            int h = item.Value.Height + gap;
                            if (sizewh < h) sizewh = h;
                        }
                        break;
                }
                return rect_dir;
            }

            void PaintText(Graphics g, Rectangle[] rects, Tabs owner, TabPage page, SolidBrush brush)
            {
                if (page.HasIcon)
                {
                    if (page.Icon != null) g.DrawImage(page.Icon, rects[3]);
                    else if (page.IconSvg != null)
                    {
                        using (var bmp = SvgExtend.GetImgExtend(page.IconSvg, rects[3], brush.Color))
                        {
                            if (bmp != null) g.DrawImage(bmp, rects[3]);
                        }
                    }
                    g.DrawString(page.Text, owner.Font, brush, rects[2], owner.s_c);
                }
                else g.DrawString(page.Text, owner.Font, brush, rects[0], owner.s_c);
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
                            RectangleF OldValue = rects[old][1], NewValue = rects[value][1];
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
                    else AnimationBarValue = rects[value][1];
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

            int bordersize = 1;
            /// <summary>
            /// 边框大小
            /// </summary>
            [Description("边框大小"), Category("样式"), DefaultValue(1)]
            public int Border
            {
                get => bordersize;
                set
                {
                    if (bordersize == value) return;
                    bordersize = value;
                    owner?.LoadLayout();
                }
            }

            Color? border;
            /// <summary>
            /// 卡片边框颜色
            /// </summary>
            [Description("卡片边框颜色"), Category("卡片"), DefaultValue(null)]
            [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
            public Color? BorderColor
            {
                get => border;
                set
                {
                    if (border == value) return;
                    border = value;
                    owner?.Invalidate();
                }
            }

            /// <summary>
            /// 卡片边框激活颜色
            /// </summary>
            [Description("卡片边框激活颜色"), Category("卡片"), DefaultValue(null)]
            [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
            public Color? BorderActive { get; set; }


            Color? fill;
            /// <summary>
            /// 卡片颜色
            /// </summary>
            [Description("卡片颜色"), Category("卡片"), DefaultValue(null)]
            [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
            public Color? Fill
            {
                get => fill;
                set
                {
                    if (fill == value) return;
                    fill = value;
                    owner?.Invalidate();
                }
            }

            /// <summary>
            /// 卡片悬停颜色
            /// </summary>
            [Description("卡片悬停颜色"), Category("卡片"), DefaultValue(null)]
            [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
            public Color? FillHover { get; set; }

            /// <summary>
            /// 卡片激活颜色
            /// </summary>
            [Description("卡片激活颜色"), Category("卡片"), DefaultValue(null)]
            [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
            public Color? FillActive { get; set; }

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

            Rectangle[][] rects = new Rectangle[0][];
            public void LoadLayout(Tabs tabs, Rectangle rect, TabCollection items)
            {
                owner = tabs;
                rects = Helper.GDI(g =>
                {
                    int gap = (int)(tabs.Gap * Config.Dpi), gapI = gap / 2, xy = 0, xy2 = 0;
                    int cardgap = (int)(Gap * Config.Dpi);

                    var rect_list = new List<Rectangle[]>(items.Count);
                    var rect_dir = GetDir(tabs, g, items, gap, out int ico_size, out xy2);
                    switch (tabs.Alignment)
                    {
                        case TabAlignment.Bottom:
                            int y = rect.Bottom - xy2;
                            foreach (var it in rect_dir)
                            {
                                Rectangle rect_it;
                                if (it.Key.HasIcon)
                                {
                                    rect_it = new Rectangle(rect.X + xy, y, it.Value.Width + gap + ico_size + gap, xy2);
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, it.Value.Width + gap, rect_it.Height),
                                        new Rectangle(rect_it.X + gapI, rect_it.Y + (rect_it.Height - ico_size) / 2, ico_size, ico_size)
                                    });
                                }
                                else
                                {
                                    rect_it = new Rectangle(rect.X + xy, y, it.Value.Width + gap, xy2);
                                    rect_list.Add(new Rectangle[] {
                                        rect_it
                                    });
                                }
                                it.Key.SetRect(rect_it);
                                xy += rect_it.Width + cardgap;
                            }
                            tabs.SetPadding(0, 0, 0, xy2);
                            break;
                        case TabAlignment.Left:
                            foreach (var it in rect_dir)
                            {
                                Rectangle rect_it = new Rectangle(rect.X, rect.Y + xy, xy2, it.Value.Height + gap);
                                if (it.Key.HasIcon)
                                {
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, it.Value.Width + gap, rect_it.Height),
                                        new Rectangle(rect_it.X + gapI, rect_it.Y + (rect_it.Height - ico_size) / 2, ico_size, ico_size)
                                    });
                                }
                                else
                                {
                                    rect_list.Add(new Rectangle[] {
                                        rect_it
                                    });
                                }

                                it.Key.SetRect(rect_it);
                                xy += rect_it.Height + cardgap;
                            }
                            tabs.SetPadding(xy2, 0, 0, 0);
                            break;
                        case TabAlignment.Right:
                            int x = rect.Right - xy2;
                            foreach (var it in rect_dir)
                            {
                                Rectangle rect_it = new Rectangle(x, rect.Y + xy, xy2, it.Value.Height + gap);
                                if (it.Key.HasIcon)
                                {
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, it.Value.Width + gap, rect_it.Height),
                                        new Rectangle(rect_it.X + gapI, rect_it.Y + (rect_it.Height - ico_size) / 2, ico_size, ico_size)
                                    });
                                }
                                else
                                {
                                    rect_list.Add(new Rectangle[] {
                                        rect_it
                                    });
                                }

                                it.Key.SetRect(rect_it);
                                xy += rect_it.Height + cardgap;
                            }
                            tabs.SetPadding(0, 0, xy2, 0);
                            break;
                        case TabAlignment.Top:
                        default:
                            foreach (var it in rect_dir)
                            {
                                Rectangle rect_it;
                                if (it.Key.HasIcon)
                                {
                                    rect_it = new Rectangle(rect.X + xy, rect.Y, it.Value.Width + gap + ico_size + gap, xy2);
                                    rect_list.Add(new Rectangle[] {
                                        rect_it,
                                        new Rectangle(rect_it.X + ico_size + gapI, rect_it.Y, it.Value.Width + gap, rect_it.Height),
                                        new Rectangle(rect_it.X + gapI, rect_it.Y + (rect_it.Height - ico_size) / 2, ico_size, ico_size)
                                    });
                                }
                                else
                                {
                                    rect_it = new Rectangle(rect.X + xy, rect.Y, it.Value.Width + gap, xy2);
                                    rect_list.Add(new Rectangle[] { rect_it });
                                }
                                it.Key.SetRect(rect_it);
                                xy += rect_it.Width + cardgap;
                            }
                            tabs.SetPadding(0, xy2, 0, 0);
                            break;
                    }
                    return rect_list.ToArray();
                });
            }

            public void Paint(Tabs owner, Graphics g, TabCollection items)
            {
                if (rects.Length == items.Count)
                {
                    using (var brush_fore = new SolidBrush(owner.ForeColor ?? AntdUI.Style.Db.Text))
                    using (var brush_fill = new SolidBrush(owner.Fill ?? AntdUI.Style.Db.Primary))
                    using (var brush_active = new SolidBrush(owner.FillActive ?? AntdUI.Style.Db.PrimaryActive))
                    using (var brush_hover = new SolidBrush(owner.FillHover ?? AntdUI.Style.Db.PrimaryHover))
                    using (var brush_bg = new SolidBrush(Fill ?? AntdUI.Style.Db.FillQuaternary))
                    using (var brush_bg_hover = new SolidBrush(FillHover ?? AntdUI.Style.Db.FillQuaternary))
                    using (var brush_bg_active = new SolidBrush(FillActive ?? AntdUI.Style.Db.BgContainer))
                    {
                        var rect_t = owner.ClientRectangle;
                        int radius = (int)(Radius * Config.Dpi), bor = (int)(bordersize * Config.Dpi), bor2 = bor * 6, bor22 = bor2 * 2;
                        float borb2 = bor / 2F;
                        TabPage? sel = null;
                        int i = 0, select = owner.SelectedIndex;
                        switch (owner.Alignment)
                        {
                            case TabAlignment.Bottom:
                                int read_b_h = rects[0][0].Height + rects[0][0].X;
                                g.SetClip(new Rectangle(rect_t.X, rect_t.Bottom - read_b_h, rect_t.Width, read_b_h));
                                foreach (var page in items)
                                {
                                    if (select == i) sel = page;
                                    else
                                    {
                                        using (var path = Helper.RoundPath(page.Rect, radius, false, false, true, true))
                                        {
                                            g.FillPath(brush_bg, path);
                                            if (bor > 0)
                                            {
                                                using (var pen_bg = new Pen(border ?? AntdUI.Style.Db.BorderSecondary, bor))
                                                {
                                                    g.DrawPath(pen_bg, path);
                                                }
                                            }
                                            if (owner.hover_i == i) PaintText(g, rects[i], owner, page, page.MDown ? brush_active : brush_hover);
                                            else PaintText(g, rects[i], owner, page, brush_fore);
                                        }
                                    }
                                    i++;
                                }
                                g.ResetClip();
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, false, false, true, true))
                                    {
                                        if (bor > 0)
                                        {
                                            using (var pen_bg = new Pen(BorderActive ?? AntdUI.Style.Db.BorderColor, bor))
                                            {
                                                float ly = rect_page.Y + borb2;
                                                g.DrawLine(pen_bg, rect_t.X, ly, rect_t.Right, ly);
                                                using (var path2 = Helper.RoundPath(new RectangleF(rect_page.X + borb2, rect_page.Y - borb2, rect_page.Width - bor, rect_page.Height + borb2), radius, false, false, true, true))
                                                {
                                                    g.FillPath(brush_bg_active, path2);
                                                }
                                                g.SetClip(new RectangleF(rect_page.X - borb2, rect_page.Y + borb2, rect_page.Width + bor, rect_page.Height + bor));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        else g.FillPath(brush_bg_active, path);
                                        PaintText(g, rects[select], owner, sel, brush_fill);
                                    }
                                }
                                break;
                            case TabAlignment.Left:
                                g.SetClip(new Rectangle(rect_t.X, rect_t.Y, rects[0][0].Right, rect_t.Height));
                                foreach (var page in items)
                                {
                                    if (owner.SelectedIndex == i) sel = page;
                                    else
                                    {
                                        using (var path = Helper.RoundPath(page.Rect, radius, true, false, false, true))
                                        {
                                            g.FillPath(brush_bg, path);
                                            if (bor > 0)
                                            {
                                                using (var pen_bg = new Pen(border ?? AntdUI.Style.Db.BorderSecondary, bor))
                                                {
                                                    g.DrawPath(pen_bg, path);
                                                }
                                            }
                                            if (owner.hover_i == i) PaintText(g, rects[i], owner, page, page.MDown ? brush_active : brush_hover);
                                            else PaintText(g, rects[i], owner, page, brush_fore);
                                        }
                                    }
                                    i++;
                                }
                                g.ResetClip();
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, true, false, false, true))
                                    {
                                        if (bor > 0)
                                        {
                                            using (var pen_bg = new Pen(BorderActive ?? AntdUI.Style.Db.BorderColor, bor))
                                            {
                                                float lx = rect_page.Right - borb2;
                                                g.DrawLine(pen_bg, lx, rect_t.Y, lx, rect_t.Bottom);
                                                using (var path2 = Helper.RoundPath(new RectangleF(rect_page.X - borb2, rect_page.Y + borb2, rect_page.Width + borb2, rect_page.Height - bor), radius, true, false, false, true))
                                                {
                                                    g.FillPath(brush_bg_active, path2);
                                                }
                                                g.SetClip(new RectangleF(rect_page.X - borb2, rect_page.Y - borb2, rect_page.Width, rect_page.Height + bor));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        else g.FillPath(brush_bg_active, path);
                                        PaintText(g, rects[select], owner, sel, brush_fill);
                                    }
                                }
                                break;
                            case TabAlignment.Right:
                                int read_r_w = rects[0][0].Width + rects[0][0].Y;
                                g.SetClip(new Rectangle(rect_t.Right - read_r_w, rect_t.Y, read_r_w, rect_t.Height));
                                foreach (var page in items)
                                {
                                    if (owner.SelectedIndex == i) sel = page;
                                    else
                                    {
                                        using (var path = Helper.RoundPath(page.Rect, radius, false, true, true, false))
                                        {
                                            g.FillPath(brush_bg, path);
                                            if (bor > 0)
                                            {
                                                using (var pen_bg = new Pen(border ?? AntdUI.Style.Db.BorderSecondary, bor))
                                                {
                                                    g.DrawPath(pen_bg, path);
                                                }
                                            }
                                            if (owner.hover_i == i) PaintText(g, rects[i], owner, page, page.MDown ? brush_active : brush_hover);
                                            else PaintText(g, rects[i], owner, page, brush_fore);
                                        }
                                    }
                                    i++;
                                }
                                g.ResetClip();
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, false, true, true, false))
                                    {
                                        if (bor > 0)
                                        {
                                            using (var pen_bg = new Pen(BorderActive ?? AntdUI.Style.Db.BorderColor, bor))
                                            {
                                                float lx = rect_page.X + borb2;
                                                g.DrawLine(pen_bg, lx, rect_t.Y, lx, rect_t.Bottom);
                                                using (var path2 = Helper.RoundPath(new RectangleF(rect_page.X - borb2, rect_page.Y + borb2, rect_page.Width + borb2, rect_page.Height - bor), radius, false, true, true, false))
                                                {
                                                    g.FillPath(brush_bg_active, path2);
                                                }
                                                g.SetClip(new RectangleF(rect_page.X + borb2, rect_page.Y - borb2, rect_page.Width + borb2, rect_page.Height + bor));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        else g.FillPath(brush_bg_active, path);
                                        PaintText(g, rects[select], owner, sel, brush_fill);
                                    }
                                }
                                break;
                            case TabAlignment.Top:
                            default:
                                g.SetClip(new Rectangle(rect_t.X, rect_t.Y, rect_t.Width, rects[0][0].Bottom));
                                foreach (var page in items)
                                {
                                    if (owner.SelectedIndex == i) sel = page;
                                    else
                                    {
                                        using (var path = Helper.RoundPath(page.Rect, radius, true, true, false, false))
                                        {
                                            g.FillPath(owner.hover_i == i ? brush_bg_hover : brush_bg, path);
                                            if (bor > 0)
                                            {
                                                using (var pen_bg = new Pen(border ?? AntdUI.Style.Db.BorderSecondary, bor))
                                                {
                                                    g.DrawPath(pen_bg, path);
                                                }
                                            }
                                            if (owner.hover_i == i) PaintText(g, rects[i], owner, page, page.MDown ? brush_active : brush_hover);
                                            else PaintText(g, rects[i], owner, page, brush_fore);
                                        }
                                    }
                                    i++;
                                }
                                g.ResetClip();
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, true, true, false, false))
                                    {
                                        if (bor > 0)
                                        {
                                            using (var pen_bg = new Pen(BorderActive ?? AntdUI.Style.Db.BorderColor, bor))
                                            {
                                                float ly = rect_page.Bottom - borb2;
                                                g.DrawLine(pen_bg, rect_t.X, ly, rect_t.Right, ly);
                                                using (var path2 = Helper.RoundPath(new RectangleF(rect_page.X + borb2, rect_page.Y - borb2, rect_page.Width - bor, rect_page.Height + borb2), radius, true, true, false, false))
                                                {
                                                    g.FillPath(brush_bg_active, path2);
                                                }
                                                g.SetClip(new RectangleF(rect_page.X - borb2, rect_page.Y - bor, rect_page.Width + bor, rect_page.Height));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        else g.FillPath(brush_bg_active, path);
                                        PaintText(g, rects[select], owner, sel, brush_fill);
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            #region 辅助

            Dictionary<TabPage, Size> GetDir(Tabs owner, Graphics g, TabCollection items, int gap, out int ico_size, out int sizewh)
            {
                sizewh = 0;
                var size_t = g.MeasureString(Config.NullText, owner.Font).Size();
                var rect_dir = new Dictionary<TabPage, Size>(items.Count);
                foreach (var item in items)
                {
                    var size = g.MeasureString(item.Text, owner.Font).Size();
                    rect_dir.Add(item, size);
                }
                ico_size = (int)(size_t.Height * owner.IconRatio);
                switch (owner.Alignment)
                {
                    case TabAlignment.Left:
                    case TabAlignment.Right:
                        foreach (var item in rect_dir)
                        {
                            int w;
                            if (item.Key.HasIcon) w = item.Value.Width + ico_size + gap * 2;
                            else w = item.Value.Width + gap;
                            if (sizewh < w) sizewh = w;
                        }
                        break;
                    case TabAlignment.Top:
                    case TabAlignment.Bottom:
                    default:
                        foreach (var item in rect_dir)
                        {
                            int h = item.Value.Height + gap;
                            if (sizewh < h) sizewh = h;
                        }
                        break;
                }
                return rect_dir;
            }

            void PaintText(Graphics g, Rectangle[] rects, Tabs owner, TabPage page, SolidBrush brush)
            {
                if (page.HasIcon)
                {
                    if (page.Icon != null) g.DrawImage(page.Icon, rects[2]);
                    else if (page.IconSvg != null)
                    {
                        using (var bmp = SvgExtend.GetImgExtend(page.IconSvg, rects[2], brush.Color))
                        {
                            if (bmp != null) g.DrawImage(bmp, rects[2]);
                        }
                    }
                    g.DrawString(page.Text, owner.Font, brush, rects[1], owner.s_c);
                }
                else g.DrawString(page.Text, owner.Font, brush, rects[0], owner.s_c);
            }

            #endregion

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