using System;
using System.IO;
using ConlangBuilder.Core;

namespace ConlangBuilder
{
    public class LabContext : ILabContext
    {
        public string AppDataDir { get; }
        private readonly string _exportRoot;
        private readonly Action<string> _logger;

        public LabContext(string exportRoot, Action<string> logger)
        {
            _exportRoot = exportRoot;
            _logger = logger;
            AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ConlangBuilder");
            Directory.CreateDirectory(AppDataDir);
        }

        public void Log(string message) => _logger?.Invoke($"[Plugin] {message}");

        public Stream CreateOutput(string suggestedFileName)
        {
            Directory.CreateDirectory(_exportRoot);
            string path = Path.Combine(_exportRoot, suggestedFileName);
            return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        }
    }
}