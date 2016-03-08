//ImageButton.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VehicleGPS.Views.Control
{
    /// <summary>
    /// ImageButton.xaml 的交互逻辑
    /// </summary>
    public partial class ImageButton : UserControl
    {
        public ImageButton()
        {
            InitializeComponent();
           // InputBinding inputBinding = new InputBinding(this.Command, new MouseGesture(MouseAction.LeftClick));
        }

        /*Image属性*/
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new UIPropertyMetadata(null));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton), new UIPropertyMetadata(0d));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageButton), new UIPropertyMetadata(0d));

        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }
        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(ImageButton), new UIPropertyMetadata(null));

        /*Text属性*/
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ImageButton), new UIPropertyMetadata(""));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }
        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(ImageButton), new UIPropertyMetadata(null));

        public Brush TextFontColor
        {
            get { return (Brush)GetValue(TextFontColorProperty); }
            set { SetValue(TextFontColorProperty, value); }
        }
        public static readonly DependencyProperty TextFontColorProperty =
            DependencyProperty.Register("TextFontColor", typeof(Brush), typeof(ImageButton), new UIPropertyMetadata(null));

        public FontFamily TextFontFamily
        {
            get { return (FontFamily)GetValue(TextFontFamilyProperty); }
            set { SetValue(TextFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty TextFontFamilyProperty =
            DependencyProperty.Register("TextFontFamily", typeof(FontFamily), typeof(ImageButton), new UIPropertyMetadata(null));

        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(double), typeof(ImageButton), new UIPropertyMetadata(0d));


        /*Border属性*/
        public CornerRadius MouseOverBorderCorner
        {
            get { return (CornerRadius)GetValue(MouseOverBorderCornerProperty); }
            set { SetValue(MouseOverBorderCornerProperty, value); }
        }
        public static readonly DependencyProperty MouseOverBorderCornerProperty =
            DependencyProperty.Register("MouseOverBorderCorner", typeof(CornerRadius), typeof(ImageButton), new UIPropertyMetadata(null));

        public Brush MouseOverBorderBackground
        {
            get { return (Brush)GetValue(MouseOverBorderBackgroundProperty); }
            set { SetValue(MouseOverBorderBackgroundProperty, value); }
        }
        public static readonly DependencyProperty MouseOverBorderBackgroundProperty =
            DependencyProperty.Register("MouseOverBorderBackground", typeof(Brush), typeof(ImageButton), new UIPropertyMetadata(null));

        public CornerRadius NormalBorderCorner
        {
            get { return (CornerRadius)GetValue(NormalBorderCornerProperty); }
            set { SetValue(NormalBorderCornerProperty, value); }
        }
        public static readonly DependencyProperty NormalBorderCornerProperty =
            DependencyProperty.Register("NormalBorderCorner", typeof(CornerRadius), typeof(ImageButton), new UIPropertyMetadata(null));

        public Brush NormalBorderBackground
        {
            get { return (Brush)GetValue(NormalBorderBackgroundProperty); }
            set { SetValue(NormalBorderBackgroundProperty, value); }
        }
        public static readonly DependencyProperty NormalBorderBackgroundProperty =
            DependencyProperty.Register("NormalBorderBackground", typeof(Brush), typeof(ImageButton), new UIPropertyMetadata(null));

        public Orientation ControlOrientation
        {
            get { return (Orientation)GetValue(ControlOrientationProperty); }
            set { SetValue(ControlOrientationProperty, value); }
        }
        public static readonly DependencyProperty ControlOrientationProperty =
            DependencyProperty.Register("ControlOrientation", typeof(Orientation), typeof(ImageButton), new UIPropertyMetadata(Orientation.Horizontal));
    }
}