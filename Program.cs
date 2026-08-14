using GtaEditor.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

var settings = new NativeWindowSettings()
{
    // Начальный размер окна.
    // Windows затем автоматически развернёт его.
    ClientSize = new Vector2i(1600, 900),

    // Название окна.
    Title = "GTA SA Editor",

    // Используем современный OpenGL-контекст.
    Flags = ContextFlags.ForwardCompatible,

    // Открываем окно сразу развёрнутым.
    WindowState = WindowState.Maximized
};

using (var window = new EditorWindow(
    GameWindowSettings.Default,
    settings))
{
    window.Run();
}