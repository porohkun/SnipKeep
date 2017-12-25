using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MimiJson;

namespace SnipKeep
{
    public class LocalLibrary : Library
    {
        public override ImageSource IconSource => Icons.Sources["database.png"];
        public override string Name => "Local library";

        public LocalLibrary(string libraryPath) : base()
        {
            _libraryPath = libraryPath;
            try
            {
                LoadLibrary();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static string SyntaxToExt(string syntax)
        {
            switch (syntax)
            {
                case "XmlDoc": return ".xml";
                case "C#": return ".cs";
                case "JavaScript": return ".js";
                case "HTML": return ".htm";
                case "ASP/XHTML": return ".htm";
                case "Boo": return ".boo";
                case "Coco": return ".co";
                case "CSS": return ".css";
                case "C++": return ".cpp";
                case "Java": return ".java";
                case "Patch": return ".txt";
                case "PowerShell": return ".ps1";
                case "PHP": return ".php";
                case "TeX": return ".tex";
                case "VB": return ".vb";
                case "XML": return ".xml";
                case "MarkDown": return ".md";
                default: return ".txt";
            }
        }

        protected override void Save(IEnumerable<Snippet> snippets)
        {
            if (!Directory.Exists(SnippetsPath))
                Directory.CreateDirectory(SnippetsPath);
            foreach (var snippet in snippets)
            {
                var json = new JsonValue(new JsonObject(
                    new JOPair("name", snippet.Name),
                    new JOPair("tags", new JsonArray(snippet.Tags.Select(t => (JsonValue)t.Name))),
                    new JOPair("description", snippet.Description),
                    new JOPair("parts", new JsonArray(snippet.Parts.Select(p => new JsonObject(
                        new JOPair("id", p.Id),
                        new JOPair("syntax", p.Syntax)
                        )).DynamicCast<JsonValue>()))
                        ));
                json.ToFile(Path.Combine(SnippetsPath, snippet.Id + ".snip"));
                foreach (var part in snippet.Parts)
                    File.WriteAllText(Path.Combine(SnippetsPath, part.Id + SyntaxToExt(part.Syntax)), part.Text);
                snippet.Saved = true;
            }
        }

        protected override void Remove(Snippet snippet)
        {
            foreach (var part in snippet.Parts)
            {
                var filename = Path.Combine(SnippetsPath, part.Id + SyntaxToExt(part.Syntax));
                if (File.Exists(filename))
                    File.Delete(filename);
            }
            {
                var filename = Path.Combine(SnippetsPath, snippet.Id + ".snip");
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        protected override IEnumerable<ILoadOperation> Load()
        {
            foreach (var file in Directory.GetFiles(SnippetsPath, "*.snip", SearchOption.TopDirectoryOnly))
            {
                yield return new LocalLibraryLoadOperation(this, file);
            }
        }

        protected class LocalLibraryLoadOperation : ILoadOperation
        {
            public string Id { get; }
            public DateTime SaveTime { get; }

            private string _path;
            private LocalLibrary _library;

            public LocalLibraryLoadOperation(LocalLibrary library, string path)
            {
                _library = library;
                _path = path;
                Id = Path.GetFileNameWithoutExtension(path);
                SaveTime = File.GetLastWriteTimeUtc(path);
            }

            public Snippet Load()
            {
                var json = LoadJson(_path);
                return new Snippet(Id, _library, SaveTime, json["name"], json["description"],
                    json["tags"].Array.Select(t => Label.GetLabelByName(t)),
                    json["parts"].Array.Select(p => new SnippetPart(p["id"], p["syntax"], p["text"], true)));
            }

            public void Update(Snippet snippet)
            {
                var json = LoadJson(_path);
                snippet.Update(json["name"], json["description"],
                    json["tags"].Array.Select(t => Label.GetLabelByName(t)),
                    json["parts"].Array.Select(p => new SnippetPart(p["id"], p["syntax"], p["text"], true)));
            }

            private JsonValue LoadJson(string file)
            {
                var path = Path.GetDirectoryName(file);

                var snip = JsonValue.ParseFile(file);
                foreach (var part in snip["parts"])
                    part.Object.Add("text", File.ReadAllText(Path.Combine(path, part["id"] + SyntaxToExt(part["syntax"]))));
                return snip;
            }
        }
    }
}
