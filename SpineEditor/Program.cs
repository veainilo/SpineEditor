using System;

namespace SpineEditor
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SpineEventEditorGame())
                game.Run();
        }
    }
}
