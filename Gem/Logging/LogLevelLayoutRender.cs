using System.Text;
using NLog;
using NLog.LayoutRenderers;

namespace Gem.Logging
{
    [LayoutRenderer("level")]
    public class LogLevelLayoutRender : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.Level.Name[0]);
        }
    }
}
