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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace AntdUI
{
    /// <summary>
    /// Tabs 标签页
    /// </summary>
    /// <remarks>选项卡切换组件。</remarks>
    [Description("Tabs 标签页")]
    [ToolboxItem(true)]
    [DefaultEvent("SelectedIndexChanged")]
    [DefaultProperty("TabPages")]
    [Designer(typeof(TabControlDesigner))]
    public partial class Tabs : IControl
    {
        #region 属性

        Color? fill;
        /// <summary>
        /// 颜色
        /// </summary>
        [Description("颜色"), Category("外观"), DefaultValue(null)]
        [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
        public Color? Fill
        {
            get => fill;
            set
            {
                if (fill == value) return;
                fill = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 悬停颜色
        /// </summary>
        [Description("悬停颜色"), Category("外观"), DefaultValue(null)]
        [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
        public Color? FillHover { get; set; }

        TabAlignment alignment = TabAlignment.Top;
        /// <summary>
        /// 位置
        /// </summary>
        [Description("位置"), Category("外观"), DefaultValue(TabAlignment.Top)]
        public TabAlignment Alignment
        {
            get { return alignment; }
            set
            {
                if (alignment == value) return;
                alignment = value;
                LoadLayout();
            }
        }

        #region 线条

        float barsize = 3F;
        /// <summary>
        /// 条大小
        /// </summary>
        [Description("条大小"), Category("条"), DefaultValue(3F)]
        public float BarSize
        {
            get => barsize;
            set
            {
                if (barsize == value) return;
                barsize = value;
                if (Type == TabType.Line) LoadLayout();
            }
        }

        int barpadding = 0;
        /// <summary>
        /// 条边距
        /// </summary>
        [Description("条边距"), Category("条"), DefaultValue(0)]
        public int BarPadding
        {
            get => barpadding;
            set
            {
                if (barpadding == value) return;
                barpadding = value;
                if (Type == TabType.Line) LoadLayout();
            }
        }

        /// <summary>
        /// 条圆角
        /// </summary>
        [Description("条圆角"), Category("条"), DefaultValue(0)]
        public int BarRadius { get; set; }

        /// <summary>
        /// 条背景大小
        /// </summary>
        [Description("条背景大小"), Category("条"), DefaultValue(1F)]
        public float BarBackSize { get; set; } = 1F;

        /// <summary>
        /// 条背景
        /// </summary>
        [Description("条背景"), Category("条"), DefaultValue(null)]
        [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
        public Color? BarBack { get; set; }

        #endregion

        #region 卡片

        /// <summary>
        /// 卡片边框颜色
        /// </summary>
        [Description("卡片边框颜色"), Category("卡片"), DefaultValue(null)]
        [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
        public Color? CardBorder { get; set; }

        int _cardgap = 2;
        /// <summary>
        /// 卡片间距
        /// </summary>
        [Description("卡片间距"), Category("卡片"), DefaultValue(2)]
        public int CardGap
        {
            get => _cardgap;
            set
            {
                if (_cardgap == value) return;
                _cardgap = value;
                LoadLayout();
            }
        }

        #endregion

        [Description("样式"), Category("外观"), DefaultValue(TabType.Line)]
        public TabType Type { get; set; } = TabType.Line;

        Color _backColor = Color.Transparent;
        /// <summary>
        /// 背景颜色
        /// </summary>
        [Browsable(true)]
        [Description("背景颜色"), Category("外观"), DefaultValue(typeof(Color), "Transparent")]
        public override Color BackColor
        {
            get => _backColor;
            set
            {
                if (_backColor == value) return;
                _backColor = value;
                Invalidate();
            }
        }

        int _gap = 8;
        /// <summary>
        /// 间距
        /// </summary>
        [Description("间距"), Category("外观"), DefaultValue(8)]
        public int Gap
        {
            get => _gap;
            set
            {
                if (_gap == value) return;
                _gap = value;
                LoadLayout();
            }
        }

        bool _tabMenuVisible = true;
        [Description("是否显示头"), Category("外观"), DefaultValue(true)]
        public bool TabMenuVisible
        {
            get => _tabMenuVisible;
            set
            {
                _tabMenuVisible = value;
                LoadLayout();
            }
        }

        public override Rectangle DisplayRectangle
        {
            get => ClientRectangle.PaddingRect(Margin, Padding, _padding);
        }

        #region 徽标

        TabsBadgeCollection? badge;
        /// <summary>
        /// 徽标集合
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("徽标集合"), Category("徽标")]
        public TabsBadgeCollection Badge
        {
            get
            {
                badge ??= new TabsBadgeCollection(this);
                return badge;
            }
            set => badge = value.BindData(this);
        }

        [Description("徽标大小"), Category("徽标"), DefaultValue(9F)]
        public float BadgeSize { get; set; } = 9F;

        [Description("徽标背景颜色"), Category("徽标"), DefaultValue(null)]
        [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
        public Color? BadgeBack { get; set; }

        #endregion

        #region 数据

        TabCollection? items;
        /// <summary>
        /// 数据
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("集合"), Category("数据")]
        public TabCollection Pages
        {
            get
            {
                items ??= new TabCollection(this);
                return items;
            }
        }

        #region 动画

        int _select = 0;
        [Description("选中序号"), Category("数据"), DefaultValue(0)]
        public int SelectedIndex
        {
            get => _select;
            set
            {
                if (_select == value) return;
                int old = _select;
                _select = value;
                if (Type == TabType.Line) SetRect(old, _select);
                else Invalidate();
                SelectedIndexChanged?.Invoke(this, value);
                ShowPage();
            }
        }

        internal void ShowPage()
        {
            if (IsHandleCreated)
            {
                BeginInvoke(new Action(() =>
                {
                    Controls.Clear();
                    if (items == null) return;
                    if (items.Count <= _select || _select < 0) return;
                    var item = items[_select];
                    Controls.Add(item);
                }));
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ControlCollection Controls => base.Controls;

        /// <summary>
        /// SelectedIndex 属性值更改时发生
        /// </summary>
        [Description("SelectedIndex 属性值更改时发生"), Category("行为")]
        public event IntEventHandler? SelectedIndexChanged = null;

        protected override void Dispose(bool disposing)
        {
            ThreadBar?.Dispose();
            base.Dispose(disposing);
        }
        bool AnimationBar = false;
        RectangleF AnimationBarValue;
        ITask? ThreadBar = null;

        void SetRect(int old, int value)
        {
            if (value > -1)
            {
                if (items == null) return;
                if (old > -1 && items.Count > old)
                {
                    ThreadBar?.Dispose();
                    Helper.GDI(g =>
                    {
                        RectangleF OldValue = items[old].Rect_Line, NewValue = items[value].Rect_Line;
                        if (AnimationBarValue.Height == 0) AnimationBarValue = OldValue;
                        if (Config.Animation)
                        {
                            if (alignment == TabAlignment.Left || alignment == TabAlignment.Right)
                            {
                                if (OldValue.X == NewValue.X)
                                {
                                    AnimationBarValue.X = OldValue.X;
                                    AnimationBar = true;
                                    float p_val = Math.Abs(NewValue.Y - AnimationBarValue.Y) * 0.09F, p_w_val = Math.Abs(NewValue.Height - AnimationBarValue.Height) * 0.1F, p_val2 = (NewValue.Y - AnimationBarValue.Y) * 0.5F;
                                    ThreadBar = new ITask(this, () =>
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
                                                Invalidate();
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            AnimationBarValue.Y -= p_val;
                                            if (AnimationBarValue.Y < NewValue.Y)
                                            {
                                                AnimationBarValue.Y = NewValue.Y;
                                                Invalidate();
                                                return false;
                                            }
                                        }
                                        Invalidate();
                                        return true;
                                    }, 10, () =>
                                    {
                                        AnimationBarValue = NewValue;
                                        AnimationBar = false;
                                        Invalidate();
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
                                    ThreadBar = new ITask(this, () =>
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
                                                Invalidate();
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            AnimationBarValue.X -= p_val;
                                            if (AnimationBarValue.X < NewValue.X)
                                            {
                                                AnimationBarValue.X = NewValue.X;
                                                Invalidate();
                                                return false;
                                            }
                                        }
                                        Invalidate();
                                        return true;
                                    }, 10, () =>
                                    {
                                        AnimationBarValue = NewValue;
                                        AnimationBar = false;
                                        Invalidate();
                                    });
                                    return;
                                }
                            }
                        }
                        AnimationBarValue = NewValue;
                        Invalidate();
                    });
                }
                else AnimationBarValue = items[value].Rect_Line;
            }
        }

        #endregion

        #endregion

        #endregion

        #region 布局

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            LoadLayout(false);
            ShowPage();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            LoadLayout(false);
            base.OnSizeChanged(e);
        }

        Padding _padding = new Padding(0);
        bool SetPadding(int x, int y, int r, int b)
        {
            if (_padding.Left == x && _padding.Top == y && _padding.Right == r && _padding.Bottom == b) return true;
            _padding.Left = x;
            _padding.Top = y;
            _padding.Right = r;
            _padding.Bottom = b;
            base.OnSizeChanged(EventArgs.Empty);
            return false;
        }

        Rectangle rect_line_top;
        internal void LoadLayout(bool r = true)
        {
            if (IsHandleCreated)
            {
                if (items == null) return;
                if (_tabMenuVisible)
                {
                    var rect_t = ClientRectangle.DeflateRect(Margin);
                    Helper.GDI(g =>
                    {
                        int gap = (int)(_gap * Config.Dpi), gapI = gap / 2, xy = 0, xy2 = 0;
                        if (Type == TabType.Line)
                        {
                            int barSize = (int)(BarSize * Config.Dpi), barPadding = (int)(BarPadding * Config.Dpi), barPadding2 = barPadding * 2;
                            switch (alignment)
                            {
                                case TabAlignment.Bottom:
                                    foreach (var item in items)
                                    {
                                        var size = g.MeasureString(item.Text, Font).Size();
                                        var rect_it = new Rectangle(rect_t.X + xy, rect_t.Y, size.Width + gap, xy2);
                                        item.SetRect(rect_it);
                                        int h = size.Height + gap;
                                        if (xy2 < h) xy2 = h;
                                        xy += rect_it.Width;
                                    }
                                    SetPadding(0, 0, 0, xy2);
                                    foreach (var item in items)
                                    {
                                        var rect_it = item.SetRectH(rect_t.Bottom - xy2, xy2);
                                        item.Rect_Line = new Rectangle(rect_it.X + barPadding, rect_it.Y, rect_it.Width - barPadding2, barSize);
                                    }
                                    if (BarBackSize > 0)
                                    {
                                        int barBackSize = (int)(BarBackSize * Config.Dpi);
                                        rect_line_top = new Rectangle(rect_t.X, rect_t.Bottom - xy2, rect_t.Width, barBackSize);
                                    }
                                    break;
                                case TabAlignment.Left:
                                    foreach (var item in items)
                                    {
                                        var size = g.MeasureString(item.Text, Font).Size();
                                        var rect_it = new Rectangle(rect_t.X, rect_t.Y + xy, xy2, size.Height + gap);
                                        item.SetRect(rect_it);
                                        int w = size.Width + gap;
                                        if (xy2 < w) xy2 = w;
                                        xy += rect_it.Height;
                                    }
                                    SetPadding(xy2, 0, 0, 0);
                                    foreach (var item in items)
                                    {
                                        var rect_it = item.SetRectW(xy2);
                                        item.Rect_Line = new Rectangle(rect_it.X + xy2 - barSize, rect_it.Y + barPadding, barSize, rect_it.Height - barPadding2);
                                    }
                                    if (BarBackSize > 0)
                                    {
                                        int barBackSize = (int)(BarBackSize * Config.Dpi);
                                        rect_line_top = new Rectangle(rect_t.X + xy2, rect_t.Y, barBackSize, rect_t.Height);
                                    }
                                    break;
                                case TabAlignment.Right:
                                    foreach (var item in items)
                                    {
                                        var size = g.MeasureString(item.Text, Font).Size();
                                        var rect_it = new Rectangle(rect_t.X, rect_t.Y + xy, xy2, size.Height + gap);
                                        item.SetRect(rect_it);
                                        int w = size.Width + gap;
                                        if (xy2 < w) xy2 = w;
                                        xy += rect_it.Height;
                                    }
                                    SetPadding(0, 0, xy2, 0);
                                    foreach (var item in items)
                                    {
                                        var rect_it = item.SetRectW(rect_t.Right - xy2, xy2);
                                        item.Rect_Line = new Rectangle(rect_it.X, rect_it.Y + barPadding, barSize, rect_it.Height - barPadding2);
                                    }
                                    if (BarBackSize > 0)
                                    {
                                        int barBackSize = (int)(BarBackSize * Config.Dpi);
                                        rect_line_top = new Rectangle(rect_t.Right - xy2, rect_t.Y, barBackSize, rect_t.Height);
                                    }
                                    break;
                                case TabAlignment.Top:
                                default:
                                    foreach (var item in items)
                                    {
                                        var size = g.MeasureString(item.Text, Font).Size();
                                        var rect_it = new Rectangle(rect_t.X + xy, rect_t.Y, size.Width + gap, xy2);
                                        item.SetRect(rect_it);
                                        int h = size.Height + gap;
                                        if (xy2 < h) xy2 = h;
                                        xy += rect_it.Width;
                                    }
                                    SetPadding(0, xy2, 0, 0);
                                    foreach (var item in items)
                                    {
                                        var rect_it = item.SetRectH(xy2);
                                        item.Rect_Line = new Rectangle(rect_it.X + barPadding, rect_it.Bottom - barSize, rect_it.Width - barPadding2, barSize);
                                    }
                                    if (BarBackSize > 0)
                                    {
                                        int barBackSize = (int)(BarBackSize * Config.Dpi);
                                        rect_line_top = new Rectangle(rect_t.Left, rect_t.Y + xy2 - barBackSize, rect_t.Width, barBackSize);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            int cardgap = (int)(_cardgap * Config.Dpi);
                            switch (alignment)
                            {
                                case TabAlignment.Bottom:
                                    foreach (var item in items)
                                    {
                                        var size = g.MeasureString(item.Text, Font).Size();
                                        var rect_it = new Rectangle(rect_t.X + xy, rect_t.Y, size.Width + gap, xy2);
                                        item.SetRect(rect_it);
                                        int h = size.Height + gap;
                                        if (xy2 < h) xy2 = h;
                                        xy += rect_it.Width + cardgap;
                                    }
                                    SetPadding(0, 0, 0, xy2);
                                    foreach (var item in items) item.SetRectH(rect_t.Bottom - xy2, xy2);
                                    break;
                                case TabAlignment.Left:
                                    foreach (var item in items)
                                    {
                                        var size = g.MeasureString(item.Text, Font).Size();
                                        var rect_it = new Rectangle(rect_t.X, rect_t.Y + xy, xy2, size.Height + gap);
                                        item.SetRect(rect_it);
                                        int w = size.Width + gap;
                                        if (xy2 < w) xy2 = w;
                                        xy += rect_it.Height + cardgap;
                                    }
                                    SetPadding(xy2, 0, 0, 0);
                                    foreach (var item in items) item.SetRectW(xy2);
                                    break;
                                case TabAlignment.Right:
                                    foreach (var item in items)
                                    {
                                        var size = g.MeasureString(item.Text, Font).Size();
                                        var rect_it = new Rectangle(rect_t.X, rect_t.Y + xy, xy2, size.Height + gap);
                                        item.SetRect(rect_it);
                                        int w = size.Width + gap;
                                        if (xy2 < w) xy2 = w;
                                        xy += rect_it.Height + cardgap;
                                    }
                                    SetPadding(0, 0, xy2, 0);
                                    foreach (var item in items) item.SetRectW(rect_t.Right - xy2, xy2);
                                    break;
                                case TabAlignment.Top:
                                default:
                                    foreach (var item in items)
                                    {
                                        var size = g.MeasureString(item.Text, Font).Size();
                                        var rect_it = new Rectangle(rect_t.X + xy, rect_t.Y, size.Width + gap, xy2);
                                        item.SetRect(rect_it);
                                        int h = size.Height + gap;
                                        if (xy2 < h) xy2 = h;
                                        xy += rect_it.Width + cardgap;
                                    }
                                    SetPadding(0, xy2, 0, 0);
                                    foreach (var item in items) item.SetRectH(xy2);
                                    break;
                            }
                        }
                        if (r) Invalidate();
                    });
                }
                else SetPadding(0, 0, 0, 0);
            }
        }

        #endregion

        #region 渲染

        Dictionary<int, TabsBadge> badges = new Dictionary<int, TabsBadge>();
        public void ChangeBadge()
        {
            badges.Clear();
#if NET40 || NET46 || NET48
            foreach (TabsBadge it in Badge)
            {
                if (!badges.ContainsKey(it.Index)) badges.Add(it.Index, it);
            }
#else
            foreach (TabsBadge it in Badge)
            {
                badges.TryAdd(it.Index, it);
            }
#endif
        }

        StringFormat s_c = Helper.SF_ALL();
        protected override void OnPaint(PaintEventArgs e)
        {
            if (items == null || !_tabMenuVisible) return;
            var g = e.Graphics.High();
            int selectedIndex = _select, hover_i = Hover_i;
            using (var brush_fore = new SolidBrush(ForeColor))
            using (var brush_fill = new SolidBrush(fill ?? Style.Db.Primary))
            using (var brush_hover = new SolidBrush(FillHover ?? Style.Db.PrimaryHover))
            {
                if (Type == TabType.Line)
                {
                    if (BarBackSize > 0)
                    {
                        using (var brush = new SolidBrush(BarBack ?? Style.Db.Fill))
                        {
                            g.FillRectangle(brush, rect_line_top);
                        }
                    }
                    if (AnimationBar)
                    {
                        if (BarRadius > 0)
                        {
                            using (var path = AnimationBarValue.RoundPath(BarRadius * Config.Dpi))
                            {
                                g.FillPath(brush_fill, path);
                            }
                        }
                        else g.FillRectangle(brush_fill, AnimationBarValue);
                        int i = 0;
                        foreach (var page in items)
                        {
                            if (page.Visible)
                            {
                                if (selectedIndex == i) g.DrawString(page.Text, Font, brush_fill, page.Rect, s_c);
                                else if (hover_i == i) g.DrawString(page.Text, Font, brush_hover, page.Rect, s_c);
                                else g.DrawString(page.Text, Font, brush_fore, page.Rect, s_c);
                            }
                            i++;
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (var page in items)
                        {
                            if (page.Visible)
                            {
                                if (selectedIndex == i)//是否选中
                                {
                                    if (BarRadius > 0)
                                    {
                                        using (var path = page.Rect_Line.RoundPath(BarRadius * Config.Dpi))
                                        {
                                            g.FillPath(brush_fill, path);
                                        }
                                    }
                                    else g.FillRectangle(brush_fill, page.Rect_Line);
                                    g.DrawString(page.Text, Font, brush_fill, page.Rect, s_c);
                                }
                                else if (hover_i == i) g.DrawString(page.Text, Font, brush_hover, page.Rect, s_c);
                                else g.DrawString(page.Text, Font, brush_fore, page.Rect, s_c);
                            }
                            i++;
                        }
                    }
                }
                else
                {
                    var rect_t = ClientRectangle;
                    int radius = (int)(6 * Config.Dpi), sp = (int)(1F * Config.Dpi), sp2 = sp * 6, sp22 = sp2 * 2;

                    using (var brush_bg = new SolidBrush(fill ?? Style.Db.FillQuaternary))
                    {
                        TabPage? sel = null;
                        int i = 0;
                        switch (alignment)
                        {
                            case TabAlignment.Bottom:
                                foreach (var page in items)
                                {
                                    if (page.Visible)
                                    {
                                        if (selectedIndex == i) sel = page;
                                        else
                                        {
                                            using (var path = Helper.RoundPath(page.Rect, radius, false, false, true, true))
                                            {
                                                g.FillPath(brush_bg, path);
                                                using (var pen_bg = new Pen(Style.Db.BorderSecondary, sp))
                                                {
                                                    g.DrawPath(pen_bg, path);
                                                }
                                                if (hover_i == i) g.DrawString(page.Text, Font, brush_hover, page.Rect, s_c);
                                                else g.DrawString(page.Text, Font, brush_fore, page.Rect, s_c);
                                            }
                                        }
                                    }
                                    i++;
                                }
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, false, false, true, true))
                                    {
                                        using (var brush_bgw = new SolidBrush(Style.Db.BgContainer))
                                        {
                                            using (var pen_bg = new Pen(CardBorder ?? Style.Db.BorderColor, sp))
                                            {
                                                int ly = rect_page.Y + sp;
                                                g.DrawLine(pen_bg, rect_t.X, ly, rect_t.Right, ly);
                                                g.FillPath(brush_bgw, path);
                                                g.SetClip(new Rectangle(rect_page.X - sp2, rect_page.Y + sp, rect_page.Width + sp22, rect_page.Height + sp22));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        g.DrawString(sel.Text, Font, brush_fill, rect_page, s_c);
                                    }
                                }
                                break;
                            case TabAlignment.Left:
                                foreach (var page in items)
                                {
                                    if (page.Visible)
                                    {
                                        if (selectedIndex == i) sel = page;
                                        else
                                        {
                                            using (var path = Helper.RoundPath(page.Rect, radius, true, false, false, true))
                                            {
                                                g.FillPath(brush_bg, path);
                                                using (var pen_bg = new Pen(Style.Db.BorderSecondary, sp))
                                                {
                                                    g.DrawPath(pen_bg, path);
                                                }
                                                if (hover_i == i) g.DrawString(page.Text, Font, brush_hover, page.Rect, s_c);
                                                else g.DrawString(page.Text, Font, brush_fore, page.Rect, s_c);
                                            }
                                        }
                                    }
                                    i++;
                                }
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, true, false, false, true))
                                    {
                                        using (var brush_bgw = new SolidBrush(Style.Db.BgContainer))
                                        {
                                            using (var pen_bg = new Pen(CardBorder ?? Style.Db.BorderColor, sp))
                                            {
                                                int lx = rect_page.Right - sp;
                                                g.DrawLine(pen_bg, lx, rect_t.Y, lx, rect_t.Bottom);
                                                g.FillPath(brush_bgw, path);
                                                g.SetClip(new Rectangle(rect_page.X - sp2, rect_page.Y - sp2, rect_page.Width + sp2 - sp, rect_page.Height + sp22));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        g.DrawString(sel.Text, Font, brush_fill, rect_page, s_c);
                                    }
                                }
                                break;
                            case TabAlignment.Right:
                                foreach (var page in items)
                                {
                                    if (page.Visible)
                                    {
                                        if (selectedIndex == i) sel = page;
                                        else
                                        {
                                            using (var path = Helper.RoundPath(page.Rect, radius, false, true, true, false))
                                            {
                                                g.FillPath(brush_bg, path);
                                                using (var pen_bg = new Pen(Style.Db.BorderSecondary, sp))
                                                {
                                                    g.DrawPath(pen_bg, path);
                                                }
                                                if (hover_i == i) g.DrawString(page.Text, Font, brush_hover, page.Rect, s_c);
                                                else g.DrawString(page.Text, Font, brush_fore, page.Rect, s_c);
                                            }
                                        }
                                    }
                                    i++;
                                }
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, false, true, true, false))
                                    {
                                        using (var brush_bgw = new SolidBrush(Style.Db.BgContainer))
                                        {
                                            using (var pen_bg = new Pen(CardBorder ?? Style.Db.BorderColor, sp))
                                            {
                                                int lx = rect_page.X + sp;
                                                g.DrawLine(pen_bg, lx, rect_t.Y, lx, rect_t.Bottom);
                                                g.FillPath(brush_bgw, path);
                                                g.SetClip(new Rectangle(rect_page.X + sp, rect_page.Y - sp2, rect_page.Width + sp22, rect_page.Height + sp22));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        g.DrawString(sel.Text, Font, brush_fill, rect_page, s_c);
                                    }
                                }
                                break;
                            case TabAlignment.Top:
                            default:
                                foreach (var page in items)
                                {
                                    if (page.Visible)
                                    {
                                        if (selectedIndex == i) sel = page;
                                        else
                                        {
                                            using (var path = Helper.RoundPath(page.Rect, radius, true, true, false, false))
                                            {
                                                g.FillPath(brush_bg, path);
                                                using (var pen_bg = new Pen(Style.Db.BorderSecondary, sp))
                                                {
                                                    g.DrawPath(pen_bg, path);
                                                }
                                                if (hover_i == i) g.DrawString(page.Text, Font, brush_hover, page.Rect, s_c);
                                                else g.DrawString(page.Text, Font, brush_fore, page.Rect, s_c);
                                            }
                                        }
                                    }
                                    i++;
                                }
                                if (sel != null)//是否选中
                                {
                                    var rect_page = sel.Rect;
                                    using (var path = Helper.RoundPath(rect_page, radius, true, true, false, false))
                                    {
                                        using (var brush_bgw = new SolidBrush(Style.Db.BgContainer))
                                        {
                                            using (var pen_bg = new Pen(CardBorder ?? Style.Db.BorderColor, sp))
                                            {
                                                int ly = rect_page.Bottom - sp;
                                                g.DrawLine(pen_bg, rect_t.X, ly, rect_t.Right, ly);
                                                g.FillPath(brush_bgw, path);
                                                g.SetClip(new Rectangle(rect_page.X - sp2, rect_page.Y - sp2, rect_page.Width + sp22, rect_page.Height + sp2 - sp));
                                                g.DrawPath(pen_bg, path);
                                                g.ResetClip();
                                            }
                                        }
                                        g.DrawString(sel.Text, Font, brush_fill, rect_page, s_c);
                                    }
                                }
                                break;
                        }
                    }
                }
                if (badges.Count > 0)
                {
                    //using (var font = new Font(Font.FontFamily, BadgeSize))
                    //{
                    //    for (int i = 0; i < TabCount; i++)
                    //    {
                    //        if (badges.TryGetValue(i, out var find)) this.PaintBadge(find, GetTabRect(i), font, g);
                    //    }
                    //}
                }
            }
            base.OnPaint(e);
        }

        #endregion

        #region 鼠标

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (items == null) { base.OnMouseDown(e); return; }
            if (_tabMenuVisible)
            {
                int i = 0;
                foreach (var item in items)
                {
                    if (item.Visible && item.Contains(e.X, e.Y))
                    {
                        item.MDown = true;
                        return;
                    }
                    i++;
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (items == null) { base.OnMouseUp(e); return; }
            if (_tabMenuVisible)
            {
                int i = 0;
                foreach (var item in items)
                {
                    if (item.MDown)
                    {
                        item.MDown = false;
                        if (item.Contains(e.X, e.Y)) SelectedIndex = i;
                        return;
                    }
                    i++;
                }
            }
            base.OnMouseUp(e);
        }

        int hover_i = -1;
        int Hover_i
        {
            get => hover_i;
            set
            {
                if (hover_i == value) return;
                hover_i = value;
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (items == null) return;
            int i = 0;
            foreach (var item in items)
            {
                if (item.Contains(e.X, e.Y))
                {
                    Hover_i = i;
                    return;
                }
                i++;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Hover_i = -1;
            base.OnMouseLeave(e);
        }

        #endregion
    }

    public class TabsBadgeCollection : iCollection<TabsBadge>
    {
        public TabsBadgeCollection(Tabs it)
        {
            BindData(it);
        }

        internal TabsBadgeCollection BindData(Tabs it)
        {
            action = render =>
            {
                if (render) it.ChangeBadge();
                it.Invalidate();
            };
            return this;
        }
    }
    public class TabsBadge
    {
        /// <summary>
        /// 序号
        /// </summary>
        [Description("序号"), Category("外观")]
        public int Index { get; set; }


        /// <summary>
        /// 徽标计数 0是点
        /// </summary>
        [Description("徽标计数"), Category("外观")]
        public int Count { get; set; }

        /// <summary>
        /// 填充颜色
        /// </summary>
        [Description("填充颜色"), Category("外观")]
        public Color? Fill { get; set; }

        /// <summary>
        /// 用户定义数据
        /// </summary>
        [Description("用户定义数据"), Category("数据"), DefaultValue(null)]
        public object? Tag { get; set; }
    }
}