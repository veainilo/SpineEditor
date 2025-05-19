using System;
using SpineEditor.Events;

namespace SpineEditor.Core
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            // 使用新的UI系统的游戏类
            using (var game = new SpineEventEditorGameNew())
                game.Run();

            // 如果需要使用旧的游戏类，取消下面的注释
            // using (var game = new SpineEventEditorGame())
            //     game.Run();
        }
    }
}
