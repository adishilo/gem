using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HelperGui
{
    /// <summary>
    /// A custome control for a button displaying an image, and another image when hovering over.
    /// </summary>
    public class TwoImageButton : Button
    {
        #region Depenedency Property definitions

        public static readonly DependencyProperty RegularImageProperty =
            DependencyProperty.Register(
                "RegularImage",
                typeof(ImageSource),
                typeof(TwoImageButton),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static readonly DependencyProperty HoverImageProperty =
            DependencyProperty.Register(
                "HoverImage",
                typeof(ImageSource),
                typeof(TwoImageButton),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static readonly DependencyProperty ClickImageProperty = DependencyProperty.Register(
            "ClickImage",
            typeof(ImageSource),
            typeof(TwoImageButton),
            new PropertyMetadata(default(ImageSource)));

        #endregion

        static TwoImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TwoImageButton), new FrameworkPropertyMetadata(typeof(TwoImageButton)));
        }

        #region Dependency Properties binding properties

        /// <summary>
        /// Gets or sets the dependency property for the regular button image.
        /// </summary>
        public ImageSource RegularImage
        {
            get { return (ImageSource)GetValue(RegularImageProperty); }

            set { SetValue(RegularImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dependency property for the hover button image.
        /// </summary>
        public ImageSource HoverImage
        {
            get { return (ImageSource)GetValue(HoverImageProperty); }

            set { SetValue(HoverImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dependency property for the click button image.
        /// </summary>
        public ImageSource ClickImage
        {
            get { return (ImageSource)GetValue(ClickImageProperty); }
            set { SetValue(ClickImageProperty, value); }
        }

        #endregion
    }
}
