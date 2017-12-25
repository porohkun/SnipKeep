using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.AvalonEdit.Highlighting;

namespace SnipKeep
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SnipKeep.Resources.sql.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    var sql = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    HighlightingManager.Instance.RegisterHighlighting("SQL", new[] { "sql" }, sql);
                }
            }
        }
    }
}
