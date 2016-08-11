using System;
using System.Windows;
using System.Windows.Controls;
using Gem;

namespace HelperGui
{
    /// <summary>
    /// Interaction logic for BrowseTextBox.xaml
    /// </summary>
    public partial class BrowseTextBox : UserControl
    {
        public BrowseTextBox()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        #region BrowsingType

        public static readonly DependencyProperty BrowsingTypeProperty = DependencyProperty.Register(
            "BrowsingType",
            typeof(IEntitiesBrowser),
            typeof(BrowseTextBox),
            new FrameworkPropertyMetadata(
                default(IEntitiesBrowser),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public IEntitiesBrowser BrowsingType
        {
            get { return (IEntitiesBrowser)GetValue(BrowsingTypeProperty); }
            set { SetValue(BrowsingTypeProperty, value); }
        }

        #endregion

        #region DefaultText

        public static readonly DependencyProperty DefaultTextProperty = DependencyProperty.Register(
            "DefaultText",
            typeof(string),
            typeof(BrowseTextBox),
            new FrameworkPropertyMetadata(
                default(string),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        #endregion

        #region Text

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(BrowseTextBox),
            new FrameworkPropertyMetadata(
                default(string),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                (dependencyObj, args) =>
                {
                    dependencyObj.CoerceValue(HasValidPathProperty);
                }));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region IsEmptyPathValid

        public static readonly DependencyProperty IsEmptyPathValidProperty = DependencyProperty.Register(
            "IsEmptyPathValid",
            typeof(bool),
            typeof(BrowseTextBox),
            new FrameworkPropertyMetadata(
                default(bool),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                (dependencyObj, args) =>
                {
                    dependencyObj.CoerceValue(HasValidPathProperty);
                }));

        public bool IsEmptyPathValid
        {
            get { return (bool)GetValue(IsEmptyPathValidProperty); }
            set { SetValue(IsEmptyPathValidProperty, value); }
        }

        #endregion

        #region HasValidPath

        public static readonly DependencyProperty HasValidPathProperty = DependencyProperty.Register(
            "HasValidPath",
            typeof(bool),
            typeof(BrowseTextBox),
            new FrameworkPropertyMetadata(
                default(bool),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                null,
                (dependencyObj, value) =>
                {
                    // Verify the current path written in the 'Text' property:
                    var browseBoxControl = dependencyObj as BrowseTextBox;

                    if (browseBoxControl == null)
                    {
                        throw new InvalidOperationException($"Coerced value comes from a non {nameof(BrowseTextBox)}");
                    }

                    if (string.IsNullOrEmpty(browseBoxControl.Text))
                    {
                        return browseBoxControl.IsEmptyPathValid;
                    }

                    return browseBoxControl.BrowsingType.IsValidValue(browseBoxControl.Text);
                }));

        public bool HasValidPath
        {
            get { return (bool)GetValue(HasValidPathProperty); }
            set { SetValue(HasValidPathProperty, value); }
        }

        #endregion

        #endregion

        private void ButtonBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            Utils.GuardNotNull(BrowsingType, "BrowsingType");

            Text = BrowsingType.Browse(Text);
        }
    }
}
