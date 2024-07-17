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
        public Tabs() { style = SetType(type); }

        #region 属性

        Color? fore;
        /// <summary>
        /// 文字颜色
        /// </summary>
        [Description("文字颜色"), Category("外观"), DefaultValue(null)]
        [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
        public new Color? ForeColor
        {
            get => fore;
            set
            {
                if (fore == value) fore = value;
                fore = value;
                Invalidate();
            }
        }

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

        /// <summary>
        /// 激活颜色
        /// </summary>
        [Description("激活颜色"), Category("外观"), DefaultValue(null)]
        [Editor(typeof(Design.ColorEditor), typeof(UITypeEditor))]
        public Color? FillActive { get; set; }

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

        #region 样式

        IStyle style;
        /// <summary>
        /// 样式
        /// </summary>
        [Description("样式"), Category("外观")]
        public IStyle Style
        {
            get => style;
            set => style = value;
        }

        TabType type = TabType.Line;
        /// <summary>
        /// 样式类型
        /// </summary>
        [Description("样式类型"), Category("外观"), DefaultValue(TabType.Line)]
        public TabType Type
        {
            get => type;
            set
            {
                if (type == value) return;
                type = value;
                style = SetType(value);
                LoadLayout();
            }
        }

        IStyle SetType(TabType type)
        {
            switch (type)
            {
                case TabType.Card:
                    return new StyleCard(this);
                case TabType.Line:
                default:
                    return new StyleLine(this);
            }
        }

        #endregion

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

        float iconratio = .7F;
        /// <summary>
        /// 图标比例
        /// </summary>
        [Description("图标比例"), Category("外观"), DefaultValue(.7F)]
        public float IconRatio
        {
            get => iconratio;
            set
            {
                if (iconratio == value) return;
                iconratio = value;
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
                style.SelectedIndexChanged(value, old);
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
            style.Dispose();
            base.Dispose(disposing);
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

        internal void LoadLayout(bool r = true)
        {
            if (IsHandleCreated)
            {
                if (items == null) return;
                if (_tabMenuVisible)
                {
                    var rect_t = ClientRectangle.DeflateRect(Margin);
                    style.LoadLayout(this, rect_t, items);
                    if (r) Invalidate();
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
            style.Paint(this, g, items);
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
                        Invalidate();
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
                        else Invalidate();
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