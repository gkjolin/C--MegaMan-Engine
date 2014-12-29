﻿using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MegaMan.Common;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Tools
{
    public class SingleTileCursor : IToolCursor
    {
        private Tile _tile;

        private static ToolCursorAdorner _cursorAdorner;

        private FrameworkElement _element;

        private static Pen outlinePen = new Pen(new SolidColorBrush(Colors.Silver) { Opacity = 0.5 }, 2);
        private static Pen shadowPen = new Pen(new SolidColorBrush(Colors.Black) { Opacity = 0.5 }, 2);

        public SingleTileCursor(Tileset tileset, Tile tile)
        {
            _tile = tile;
        }

        private ImageSource CursorImage
        {
            get { return SpriteBitmapCache.GetOrLoadFrame(_tile.Sprite.SheetPath.Absolute, _tile.Sprite.CurrentFrame.SheetLocation); }
        }

        public void ApplyCursorTo(FrameworkElement element)
        {
            _element = element;

            var layer = AdornerLayer.GetAdornerLayer(element);
            var hideCursor = (_cursorAdorner == null || _cursorAdorner.Visibility == Visibility.Hidden);

            if (_cursorAdorner != null)
                layer.Remove(_cursorAdorner);

            _cursorAdorner = new ToolCursorAdorner(element, Render);
            layer.Add(_cursorAdorner);

            WeakEventManager<FrameworkElement, MouseEventArgs>.AddHandler(element, "MouseEnter", MouseEnter);
            WeakEventManager<FrameworkElement, MouseEventArgs>.AddHandler(element, "MouseLeave", MouseLeave);

            element.Cursor = Cursors.None;
            if (hideCursor)
                _cursorAdorner.Visibility = Visibility.Hidden;
        }

        private void MouseEnter(object sender, MouseEventArgs args)
        {
            _cursorAdorner.Visibility = Visibility.Visible;
        }

        private void MouseLeave(object sender, MouseEventArgs args)
        {
            _cursorAdorner.Visibility = Visibility.Hidden;
        }

        public void Dispose()
        {
            if (_element != null)
            {
                WeakEventManager<FrameworkElement, MouseEventArgs>.RemoveHandler(_element, "MouseEnter", MouseEnter);
                WeakEventManager<FrameworkElement, MouseEventArgs>.RemoveHandler(_element, "MouseLeave", MouseLeave);
            }
        }

        private void Render(DrawingContext drawingContext)
        {
            var zoom = 1d;
            if (((FrameworkElement)_element.Parent).Parent is StageControl)
            {
                zoom = (((FrameworkElement)_element.Parent).Parent as StageControl).Zoom;
            }

            var cursorPosition = Mouse.GetPosition(_element);

            var width = _tile.Width * zoom;
            var height = _tile.Height * zoom;

            var snapX = (int)(cursorPosition.X / width) * width;
            var snapY = (int)(cursorPosition.Y / height) * height;

            drawingContext.DrawImage(this.CursorImage,
                new Rect(
                    snapX,
                    snapY,
                    width,
                    height)
                );

            drawingContext.DrawRectangle(null, outlinePen, new Rect(snapX, snapY, width, height));
            drawingContext.DrawRoundedRectangle(null, shadowPen, new Rect(snapX - 1, snapY - 1, width + 2, height + 2), 2, 2);
        }
    }
}