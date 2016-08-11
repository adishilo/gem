using System.Runtime.Remoting.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HelperGui.Commands;

namespace HelperGui
{
    /// <summary>
    /// A text box with a default message when empty, and a red border when not valid.
    /// </summary>
    public class DefaultedTextBox : TextBox
    {
        #region Dependency Properties

        public static readonly DependencyProperty DefaultTextProperty = DependencyProperty.Register(
            "DefaultText",
            typeof(string),
            typeof(DefaultedTextBox),
            new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static readonly DependencyProperty HasValidValueProperty = DependencyProperty.Register(
            "HasValidValue",
            typeof(bool),
            typeof(DefaultedTextBox),
            new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        internal static readonly DependencyProperty ClearTextCommandProperty = DependencyProperty.Register(
            "ClearTextCommand",
            typeof(ICommand),
            typeof(DefaultedTextBox),
            new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        #endregion

        #region Constructors

        static DefaultedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DefaultedTextBox), new FrameworkPropertyMetadata(typeof(DefaultedTextBox)));
        }

        public DefaultedTextBox()
        {
            SetValue(ClearTextCommandProperty, new ClearTextBoxCommand(this));
        }

        #endregion

        #region Dependency Properties binding properties

        /// <summary>
        /// Gets or sets the default text to put when the textbox is empty.
        /// </summary>
        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the text in the textbox is valid.
        /// </summary>
        public bool HasValidValue
        {
            get { return (bool)GetValue(HasValidValueProperty); }
            set { SetValue(HasValidValueProperty, value); }
        }

        internal ICommand ClearTextCommand
        {
            get { return (ICommand)GetValue(ClearTextCommandProperty); }
            set { SetValue(ClearTextCommandProperty, value); }
        }

        #endregion
    }
}
