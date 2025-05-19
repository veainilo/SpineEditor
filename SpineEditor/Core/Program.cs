using System;
using SpineEditor.Events;

namespace SpineEditor.Core
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            // 使用新的Game1类测试Spine动画渲染
            using (var game = new Game1New())
                game.Run();

            // 如果需要使用事件编辑器，取消下面的注释
            // using (var game = new SpineEventEditorGameNew())
            //     game.Run();
        }
    }
}
