﻿using Sheet.Block;
using Sheet.Block.Core;
using Sheet.Block.Model;
using Sheet.Item.Model;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sheet.WPF
{
    public class WpfBlockFactory : IBlockFactory
    {
        #region Brushes

        public static SolidColorBrush NormalBrush = Brushes.Black;
        public static SolidColorBrush SelectedBrush = Brushes.Red;
        public static SolidColorBrush HoverBrush = Brushes.Yellow;
        public static SolidColorBrush TransparentBrush = Brushes.Transparent;

        #endregion

        #region Styles

        private void SetStyle(Ellipse ellipse, bool isVisible)
        {
            var style = new Style(typeof(Ellipse));
            style.Setters.Add(new Setter(Ellipse.FillProperty, isVisible ? NormalBrush : TransparentBrush));
            style.Setters.Add(new Setter(Ellipse.StrokeProperty, isVisible ? NormalBrush : TransparentBrush));

            var isSelectedTrigger = new Trigger() { Property = FrameworkElementProperties.IsSelectedProperty, Value = true };
            isSelectedTrigger.Setters.Add(new Setter(Ellipse.FillProperty, SelectedBrush));
            isSelectedTrigger.Setters.Add(new Setter(Ellipse.StrokeProperty, SelectedBrush));
            style.Triggers.Add(isSelectedTrigger);

            var isMouseOverTrigger = new Trigger() { Property = Ellipse.IsMouseOverProperty, Value = true };
            isMouseOverTrigger.Setters.Add(new Setter(Ellipse.FillProperty, HoverBrush));
            isMouseOverTrigger.Setters.Add(new Setter(Ellipse.StrokeProperty, HoverBrush));
            style.Triggers.Add(isMouseOverTrigger);

            ellipse.Style = style;

            FrameworkElementProperties.SetIsSelected(ellipse, false);
        }

        #endregion

        #region Thumb

        private string ThumbTemplate = "<Thumb Cursor=\"SizeAll\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Thumb.Template><ControlTemplate><Rectangle Fill=\"Transparent\" Stroke=\"Red\" StrokeThickness=\"2\" Width=\"8\" Height=\"8\" Margin=\"-4,-4,0,0\"/></ControlTemplate></Thumb.Template></Thumb>";

        private void SetLineDragDeltaHandler(ILine line, IThumb thumb, Action<ILine, IThumb, double, double> drag)
        {
            (thumb.Native as Thumb).DragDelta += (sender, e) => drag(line, thumb, e.HorizontalChange, e.VerticalChange);
        }

        private void SetElementDragDeltaHandler(IElement element, IThumb thumb, Action<IElement, IThumb, double, double> drag)
        {
            (thumb.Native as Thumb).DragDelta += (sender, e) => drag(element, thumb, e.HorizontalChange, e.VerticalChange);
        }

        #endregion

        #region Color

        private IArgbColor ToArgbColor(ItemColor color)
        {
            if (color != null)
            {
                return new XArgbColor(color.Alpha, color.Red, color.Green, color.Blue);
            }
            return null;
        }

        #endregion

        #region Create

        public IThumb CreateThumb(double x, double y)
        {
            using (var stringReader = new System.IO.StringReader(ThumbTemplate))
            {
                using (var xmlReader = System.Xml.XmlReader.Create(stringReader))
                {
                    var thumb = XamlReader.Load(xmlReader) as Thumb;
                    Canvas.SetLeft(thumb, x);
                    Canvas.SetTop(thumb, y);
                    return new XThumb(thumb);
                }
            }
        }

        public IThumb CreateThumb(double x, double y, ILine line, Action<ILine, IThumb, double, double> drag)
        {
            var thumb = CreateThumb(x, y);
            SetLineDragDeltaHandler(line, thumb, drag);
            return thumb;
        }

        public IThumb CreateThumb(double x, double y, IElement element, Action<IElement, IThumb, double, double> drag)
        {
            var thumb = CreateThumb(x, y);
            SetElementDragDeltaHandler(element, thumb, drag);
            return thumb;
        }

        public IPoint CreatePoint(double thickness, double x, double y, bool isVisible)
        {
            var ellipse = new Ellipse()
            {
                StrokeThickness = thickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Width = 8.0,
                Height = 8.0,
                Margin = new Thickness(-4.0, -4.0, 0.0, 0.0),
            };

            SetStyle(ellipse, isVisible);
            Panel.SetZIndex(ellipse, 1);

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);

            var xpoint = new XPoint(ellipse, x, y, isVisible);

            return xpoint;
        }

        public ILine CreateLine(double thickness, double x1, double y1, double x2, double y2, ItemColor stroke)
        {
            var strokeBrush = new SolidColorBrush(Color.FromArgb(stroke.Alpha, stroke.Red, stroke.Green, stroke.Blue));

            strokeBrush.Freeze();

            var line = new Line()
            {
                Stroke = strokeBrush,
                StrokeThickness = thickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };

            var xline = new XLine(line);

            return xline;
        }

        public ILine CreateLine(double thickness, IPoint start, IPoint end, ItemColor stroke)
        {
            var xline = CreateLine(thickness, start.X, start.Y, end.X, end.Y, stroke);
            xline.Start = start;
            xline.End = end;
            return xline;
        }

        public IRectangle CreateRectangle(double thickness, double x, double y, double width, double height, bool isFilled, ItemColor stroke, ItemColor fill)
        {
            var strokeBrush = new SolidColorBrush(Color.FromArgb(stroke.Alpha, stroke.Red, stroke.Green, stroke.Blue));
            var fillBrush = new SolidColorBrush(Color.FromArgb(fill.Alpha, fill.Red, fill.Green, fill.Blue));

            strokeBrush.Freeze();
            fillBrush.Freeze();

            var rectangle = new Rectangle()
            {
                Fill = isFilled ? fillBrush : TransparentBrush,
                Stroke = strokeBrush,
                StrokeThickness = thickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Width = width,
                Height = height
            };

            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);

            var xrectangle = new XRectangle(rectangle);

            return xrectangle;
        }

        public IEllipse CreateEllipse(double thickness, double x, double y, double width, double height, bool isFilled, ItemColor stroke, ItemColor fill)
        {
            var strokeBrush = new SolidColorBrush(Color.FromArgb(stroke.Alpha, stroke.Red, stroke.Green, stroke.Blue));
            var fillBrush = new SolidColorBrush(Color.FromArgb(fill.Alpha, fill.Red, fill.Green, fill.Blue));

            strokeBrush.Freeze();
            fillBrush.Freeze();

            var ellipse = new Ellipse()
            {
                Fill = isFilled ? fillBrush : TransparentBrush,
                Stroke = strokeBrush,
                StrokeThickness = thickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Width = width,
                Height = height
            };

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);

            var xellipse = new XEllipse(ellipse);

            return xellipse;
        }

        public IText CreateText(string text, double x, double y, double width, double height, int halign, int valign, double fontSize, ItemColor backgroud, ItemColor foreground)
        {
            var backgroundBrush = new SolidColorBrush(Color.FromArgb(backgroud.Alpha, backgroud.Red, backgroud.Green, backgroud.Blue));
            var foregroundBrush = new SolidColorBrush(Color.FromArgb(foreground.Alpha, foreground.Red, foreground.Green, foreground.Blue));

            backgroundBrush.Freeze();
            foregroundBrush.Freeze();

            var grid = new Grid();
            grid.Background = backgroundBrush;
            grid.Width = width;
            grid.Height = height;
            Canvas.SetLeft(grid, x);
            Canvas.SetTop(grid, y);

            var tb = new TextBlock();
            tb.HorizontalAlignment = (HorizontalAlignment)halign;
            tb.VerticalAlignment = (VerticalAlignment)valign;
            tb.Background = backgroundBrush;
            tb.Foreground = foregroundBrush;
            tb.FontSize = fontSize;
            tb.FontFamily = new FontFamily("Calibri");
            tb.Text = text;

            grid.Children.Add(tb);

            var xtext = new XText(grid);

            return xtext;
        }

        public IImage CreateImage(double x, double y, double width, double height, byte[] data)
        {
            Image image = new Image();

            // enable high quality image scaling
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            // store original image data is Tag property
            image.Tag = data;

            // opacity mask is used for determining selection state
            image.OpacityMask = NormalBrush;

            //using(var ms = new System.IO.MemoryStream(data))
            //{
            //    image = Image.FromStream(ms);
            //}
            using (var ms = new System.IO.MemoryStream(data))
            {
                IBitmap profileImage = BitmapLoader.Current.Load(ms, null, null).Result;
                image.Source = profileImage.ToNative();
            }

            image.Width = width;
            image.Height = height;

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);

            var ximage = new XImage(image);

            return ximage;
        }

        public IBlock CreateBlock(int id, double x, double y, double width, double height, int dataId, string name, ItemColor backgroud)
        {
            var xblock = new XBlock(id, x, y, width, height, dataId, name)
            {
                Backgroud = ToArgbColor(backgroud)
            };
            return xblock;
        }

        #endregion
    }
}