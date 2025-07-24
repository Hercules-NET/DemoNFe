using System.Diagnostics;

namespace HerculesZeusDfeDemo
{
    public static class Helpers
    {
        public static void AbrirXml(string xml)
        {
            string caminho = Path.GetTempPath() + Guid.NewGuid().ToString() + ".xml";
            File.WriteAllText(caminho, xml);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = caminho,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Maximized
            };
            Process.Start(startInfo);
        }

        public static void AbrirPdf(byte[] bytes)
        {
            string caminho = Path.GetTempPath() + Guid.NewGuid().ToString() + ".pdf";
            File.WriteAllBytes(caminho, bytes);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = caminho,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Maximized
            };
            Process.Start(startInfo);
        }
    }
}
