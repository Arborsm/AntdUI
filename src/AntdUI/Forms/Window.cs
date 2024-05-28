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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Vanara.PInvoke;
using static Vanara.PInvoke.DwmApi;
using static Vanara.PInvoke.User32;

namespace AntdUI
{
    public class Window : BaseForm, IMessageFilter
    {
        #region 属性

        bool resizable = true;
        [Description("调整窗口大小"), Category("行为"), DefaultValue(true)]
        public bool Resizable
        {
            get => resizable;
            set
            {
                if (resizable == value) return;
                resizable = value;
                HandMessage();
            }
        }

        bool dark = false;
        /// <summary>
        /// 深色模式
        /// </summary>
        [Description("深色模式"), Category("外观"), DefaultValue(false)]
        public bool Dark
        {
            get => dark;
            set
            {
                if (dark == value) return;
                dark = value;
                mode = dark ? TAMode.Dark : TAMode.Light;
                if (IsHandleCreated) DarkUI.UseImmersiveDarkMode(Handle, value);
            }
        }

        TAMode mode = TAMode.Auto;
        /// <summary>
        /// 色彩模式
        /// </summary>
        [Description("色彩模式"), Category("外观"), DefaultValue(TAMode.Auto)]
        public TAMode Mode
        {
            get => mode;
            set
            {
                if (mode == value) return;
                mode = value;
                if (mode == TAMode.Dark || (mode == TAMode.Auto || Config.Mode == TMode.Dark)) Dark = true;
                else Dark = false;
            }
        }

        WState winState = WState.Restore;
        WState WinState
        {
            set
            {
                if (winState == value) return;
                winState = value;
                if (IsHandleCreated) HandMessage();
            }
        }

        protected virtual bool UseMessageFilter { get => false; }
        void HandMessage()
        {
            ReadMessage = winState == WState.Restore && resizable;
            if (UseMessageFilter) IsAddMessage = true;
            else IsAddMessage = ReadMessage;
        }

        bool ReadMessage = false;
        bool _isaddMessage = false;
        bool IsAddMessage
        {
            set
            {
                if (_isaddMessage == value) return;
                _isaddMessage = value;
                if (value) Application.AddMessageFilter(this);
                else Application.RemoveMessageFilter(this);
            }
        }

        #endregion

        internal HWND handle { get; private set; }

        protected override void OnHandleCreated(EventArgs e)
        {
            handle = new HWND(Handle);
            base.OnHandleCreated(e);
            if (mode == TAMode.Dark || (mode == TAMode.Auto || Config.Mode == TMode.Dark)) DarkUI.UseImmersiveDarkMode(Handle, true);
            DisableProcessWindowsGhosting();
            HandMessage();
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            var msg = (WindowMessage)m.Msg;
            switch (msg)
            {
                case WindowMessage.WM_ACTIVATE:
                    DwmExtendFrameIntoClientArea(handle, new MARGINS(0, 0, 1, 0));
                    break;
                case WindowMessage.WM_NCCALCSIZE when m.WParam != IntPtr.Zero:
                    if (WmNCCalcSize(ref m)) return;
                    break;
                case WindowMessage.WM_NCACTIVATE:
                    if (WmNCActivate(ref m)) return;
                    break;
                case WindowMessage.WM_SIZE:
                    WmSize(ref m);
                    break;
            }
            base.WndProc(ref m);
        }

        #region 区域

        /// <summary>
        /// 获取或设置窗体的位置
        /// </summary>
        public new Point Location
        {
            get
            {
                if (winState == WState.Restore) return base.Location;
                return ScreenRectangle.Location;
            }
            set { base.Location = value; }
        }

        /// <summary>
        /// 控件的顶部坐标
        /// </summary>
        public new int Top
        {
            get => Location.Y;
            set { base.Top = value; }
        }

        /// <summary>
        /// 控件的左侧坐标
        /// </summary>
        public new int Left
        {
            get => Location.X;
            set { base.Left = value; }
        }

        /// <summary>
        /// 控件的右坐标
        /// </summary>
        public new int Right
        {
            get => ScreenRectangle.Right;
        }

        /// <summary>
        /// 控件的底部坐标
        /// </summary>
        public new int Bottom
        {
            get => ScreenRectangle.Bottom;
        }

        /// <summary>
        /// 获取或设置窗体的大小
        /// </summary>
        public new Size Size
        {
            get
            {
                if (winState == WState.Restore) return base.Size;
                return ScreenRectangle.Size;
            }
            set { base.Size = value; }
        }

        /// <summary>
        /// 控件的宽度
        /// </summary>
        public new int Width
        {
            get => Size.Width;
            set { base.Width = value; }
        }

        /// <summary>
        /// 控件的高度
        /// </summary>
        public new int Height
        {
            get => Size.Height;
            set { base.Height = value; }
        }

        /// <summary>
        /// 获取或设置窗体屏幕区域
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ScreenRectangle
        {
            get
            {
                if (winState == WState.Restore) return new Rectangle(base.Location, base.Size);
                var rect = ClientRectangle;
                var point = RectangleToScreen(Rectangle.Empty);
                return new Rectangle(point.Location, rect.Size);
            }
            set
            {
                base.Location = value.Location;
                base.Size = value.Size;
            }
        }

        #endregion

        #region 交互

        #region 调整窗口大小

        /// <summary>
        /// 调整窗口大小（鼠标移动）
        /// </summary>
        /// <returns>可以调整</returns>
        public override bool ResizableMouseMove()
        {
            var retval = HitTest(PointToClient(MousePosition));
            if (retval != HitTestValues.HTNOWHERE)
            {
                var mode = retval;
                if (mode != HitTestValues.HTCLIENT && winState == WState.Restore)
                {
                    SetCursorHit(mode);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 调整窗口大小（鼠标移动）
        /// </summary>
        /// <param name="point">客户端坐标</param>
        /// <returns>可以调整</returns>
        public override bool ResizableMouseMove(Point point)
        {
            var retval = HitTest(point);
            if (retval != HitTestValues.HTNOWHERE)
            {
                var mode = retval;
                if (mode != HitTestValues.HTCLIENT && winState == WState.Restore)
                {
                    SetCursorHit(mode);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 鼠标

        public override bool IsMax
        {
            get => winState == WState.Maximize;
        }

        public static bool CanHandMessage = true;
        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            if (is_resizable) return OnPreFilterMessage(m);
            if (CanHandMessage && ReadMessage)
            {
                switch (m.Msg)
                {
                    case 0xa0:
                    case 0x200:
                        if (isMe(m.HWnd))
                        {
                            if (ResizableMouseMove(PointToClient(MousePosition))) return true;
                        }
                        break;
                    case 0xa1:
                    case 0x201:
                        if (isMe(m.HWnd))
                        {
                            if (ResizableMouseDown()) return true;
                        }
                        break;
                }
            }
            return OnPreFilterMessage(m);
        }

        protected virtual bool OnPreFilterMessage(System.Windows.Forms.Message m) { return false; }

        bool isMe(IntPtr intPtr)
        {
            var frm = FromHandle(intPtr);
            if (frm == this || GetParent(frm) == this) return true;
            return false;
        }

        static Control? GetParent(Control? control)
        {
            try
            {
                if (control != null && control.IsHandleCreated && control.Parent != null)
                {
                    if (control is Form) return control;
                    return GetParent(control.Parent);
                }
            }
            catch { }
            return control;
        }

        #endregion

        #endregion

        #region WindowMessage Handlers

        const nint SIZE_RESTORED = 0;
        const nint SIZE_MINIMIZED = 1;
        const nint SIZE_MAXIMIZED = 2;
        void WmSize(ref System.Windows.Forms.Message m)
        {
            if (m.WParam == SIZE_MINIMIZED) WinState = WState.Minimize;
            else if (m.WParam == SIZE_MAXIMIZED) WinState = WState.Maximize;
            else if (m.WParam == SIZE_RESTORED) WinState = WState.Restore;
        }

        bool WmNCCalcSize(ref System.Windows.Forms.Message m)
        {
            if (FormBorderStyle == FormBorderStyle.None) return false;
#if NET40
            var nccsp = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));
#else
            var nccsp = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(m.LParam);
#endif
            var borders = GetNonClientMetrics();

            if (IsZoomed(handle))
            {
                nccsp.rgrc0.top -= borders.Top + 1;
                nccsp.rgrc0.top += borders.Bottom;
                Marshal.StructureToPtr(nccsp, m.LParam, false);
            }
            else
            {
                nccsp.rgrc0.top -= borders.Top;
                nccsp.rgrc0.left -= borders.Left - 1;
                nccsp.rgrc0.right += borders.Right - 1;
                nccsp.rgrc0.bottom += borders.Bottom - 1;
                Marshal.StructureToPtr(nccsp, m.LParam, false);
            }
            return false;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NCCALCSIZE_PARAMS
        {
            public RECT rgrc0, rgrc1, rgrc2;
            public WINDOWPOS lppos;
        }

        bool WmNCActivate(ref System.Windows.Forms.Message m)
        {
            if (m.HWnd == IntPtr.Zero) return false;
            if (IsIconic(m.HWnd)) return false;
            m.Result = DefWindowProc(m.HWnd, (uint)m.Msg, m.WParam, new IntPtr(-1));
            return true;
        }

        #endregion

        #region Frameless Crack

        protected override void SetClientSizeCore(int x, int y)
        {
            if (DesignMode) Size = new Size(x, y);
            else base.SetClientSizeCore(x, y);
        }

        protected Padding GetNonClientMetrics()
        {
            var screenRect = ClientRectangle;
            screenRect.Offset(-Bounds.Left, -Bounds.Top);
            var rect = new RECT(screenRect);
            AdjustWindowRectEx(ref rect, (WindowStyles)CreateParams.Style, false, (WindowStylesEx)CreateParams.ExStyle);
            return new Padding
            {
                Top = screenRect.Top - rect.top,
                Left = screenRect.Left - rect.left,
                Bottom = rect.bottom - screenRect.Bottom,
                Right = rect.right - screenRect.Right
            };
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (_shouldPerformMaximiazedState && winState != WState.Maximize)
            {
                if (y != Top) y = Top;
                if (x != Left) x = Left;
                _shouldPerformMaximiazedState = false;
            }
            var size = PatchWindowSizeInRestoreWindowBoundsIfNecessary(width, height);
            base.SetBoundsCore(x, y, size.Width, size.Height, specified);
        }

        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            var rect = base.GetScaledBounds(bounds, factor, specified);
            if (!GetStyle(ControlStyles.FixedWidth) && (specified & BoundsSpecified.Width) != BoundsSpecified.None)
            {
                var clientWidth = bounds.Width;// - sz.Width;
                rect.Width = (int)Math.Round((double)(clientWidth * factor.Width));// + sz.Width;
            }
            if (!GetStyle(ControlStyles.FixedHeight) && (specified & BoundsSpecified.Height) != BoundsSpecified.None)
            {
                var clientHeight = bounds.Height;// - sz.Height;
                rect.Height = (int)Math.Round((double)(clientHeight * factor.Height));// + sz.Height;
            }
            return rect;
        }

        bool _shouldPerformMaximiazedState = false;

        Size PatchWindowSizeInRestoreWindowBoundsIfNecessary(int width, int height)
        {
            if (WindowState == FormWindowState.Normal)
            {
                var restoredWindowBoundsSpecified = typeof(Form).GetField("restoredWindowBoundsSpecified", BindingFlags.NonPublic | BindingFlags.Instance) ?? typeof(Form).GetField("_restoredWindowBoundsSpecified", BindingFlags.NonPublic | BindingFlags.Instance);
                var restoredSpecified = (BoundsSpecified)restoredWindowBoundsSpecified!.GetValue(this)!;

                if ((restoredSpecified & BoundsSpecified.Size) != BoundsSpecified.None)
                {
                    var formStateExWindowBoundsFieldInfo = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
                    var formStateExFieldInfo = typeof(Form).GetField("formStateEx", BindingFlags.NonPublic | BindingFlags.Instance) ?? typeof(Form).GetField("_formStateEx", BindingFlags.NonPublic | BindingFlags.Instance);
                    var restoredBoundsFieldInfo = typeof(Form).GetField("restoredWindowBounds", BindingFlags.NonPublic | BindingFlags.Instance) ?? typeof(Form).GetField("_restoredWindowBounds", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (formStateExWindowBoundsFieldInfo != null && formStateExFieldInfo != null && restoredBoundsFieldInfo != null)
                    {
                        var restoredWindowBounds = (Rectangle)restoredBoundsFieldInfo.GetValue(this)!;
                        var section = (BitVector32.Section)formStateExWindowBoundsFieldInfo.GetValue(this)!;
                        var vector = (BitVector32)formStateExFieldInfo.GetValue(this)!;
                        if (vector[section] == 1)
                        {
                            width = restoredWindowBounds.Width;// + borders.Horizontal;
                            height = restoredWindowBounds.Height;
                        }
                    }
                }
            }
            return new Size(width, height);
        }

        #endregion
    }

    public enum WState
    {
        Restore,
        Maximize,
        Minimize
    }
}