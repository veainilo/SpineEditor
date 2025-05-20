using System;
using SpineEditor.Events;

namespace SpineEditor.Core
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            // 使用基于GUILayout系统的Spine事件编辑器
            using (var game = new SpineEventEditorGameGUI())
                game.Run();

            // 使用GUILayout演示
            // using (var game = new UI.GUILayoutComponents.GUILayoutDemo())
            //     game.Run();

            // 使用新的UI系统的游戏类
            // using (var game = new SpineEventEditorGameNew())
            //     game.Run();

            // 如果需要使用旧的游戏类，取消下面的注释
            // using (var game = new SpineEventEditorGame())
            //     game.Run();
        }
    }
}
