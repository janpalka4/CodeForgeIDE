using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeForgeIDE
{
    public class DraggableWindow : Window
    {
        private bool _isDragging;
        private Point? _prevPoint = null;

        protected void SetupDragControl(Control control)
        {
            control.PointerPressed += Panel_PointerPressed_MoveWnd;
            control.PointerMoved += Panel_PointerMoved_MoveWnd;
            control.PointerReleased += Panel_PointerReleased_MoveWnd;
            control.PointerExited += Panel_PointerExited_MoveWnd;
        }

        private void Panel_PointerPressed_MoveWnd(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                _isDragging = true;
            }
        }

        private void Panel_PointerExited_MoveWnd(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            _isDragging = false;
        }

        private void Panel_PointerReleased_MoveWnd(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            _isDragging = false;
        }

        private void Panel_PointerMoved_MoveWnd(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (_isDragging)
            {
                if (!_prevPoint.HasValue)
                {
                    _prevPoint = e.GetPosition(this);
                }

                Point p = e.GetPosition(this);
                Point delta = p - _prevPoint.Value;

                this.Position += new PixelPoint((int)delta.X, (int)delta.Y);
            }
        }
    }
}
