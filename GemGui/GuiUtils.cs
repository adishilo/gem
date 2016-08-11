using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Gem;
using Image = System.Windows.Controls.Image;

namespace GemGui
{
    public static class GuiUtils
    {
        /// <summary>
        /// Add a simple <see cref="MenuItem"/> to a given <see cref="ContextMenu"/>.
        /// </summary>
        /// <param name="menu">The context menu.</param>
        /// <param name="header">The display name for the menu-item.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandParameters">Parameters to the command.</param>
        /// <param name="imageRelativePath">A relative path to the image. If 'null', no icon is presented.</param>
        /// <param name="toolTip">A tooltip to attach to the added menu item.</param>
        public static void AddMenuItem(
            this ContextMenu menu,
            string header,
            ICommand command,
            object commandParameters,
            string imageRelativePath = null,
            LazyEvalString toolTip = null)
        {
            object menuIcon = null;

            if (!string.IsNullOrEmpty(imageRelativePath))
            {
                menuIcon = new Image
                {
                    Source = new BitmapImage(new Uri(imageRelativePath, UriKind.Relative))
                };
            }

            menu.Items.Add(
                new MenuItem
                {
                    Header = header,
                    Icon = menuIcon,
                    Command = command,
                    CommandParameter = commandParameters,
                    ToolTip = toolTip
                });
        }
    }
}
