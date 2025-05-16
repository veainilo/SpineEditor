using System;
using SpineEditor.Events;

namespace SpineEditor.Core
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
