using System;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace GtaEditor.UI
{
    /// <summary>
    /// Пользовательский интерфейс редактора.
    ///
    /// Пока здесь находится базовая структура:
    ///
    /// Верхняя панель:
    ///     Файл
    ///     Настройки
    ///     О программе
    ///
    /// Левая панель:
    ///     Список IPL-файлов
    ///
    /// В дальнейшем сюда добавим:
    ///     IDE
    ///     IPL
    ///     COL
    ///     DFF
    ///     TXD
    ///     объекты сцены
    ///     свойства выбранного объекта
    ///     дерево сцены
    /// </summary>
    public class EditorUI : IDisposable
    {
        // ============================================================
        // WIN32
        // ============================================================

        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;

        private const int WS_BORDER = 0x00800000;

        private const int WS_VSCROLL = 0x00200000;

        private const int LBS_NOTIFY = 0x00000001;
        private const int LBS_HASSTRINGS = 0x00000040;

        private const int BS_PUSHBUTTON = 0x00000000;

        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;

        private const int WM_SETFONT = 0x0030;

        private const int DEFAULT_GUI_FONT = 17;

        private const int WS_EX_CLIENTEDGE = 0x00000200;
        private const int WS_EX_COMPOSITED = 0x02000000;

        private const int SS_OWNERDRAW = 0x0000000D;
        private const int WM_CTLCOLORSTATIC = 0x0138;


        // ============================================================
        // WIN32 API
        // ============================================================

        [DllImport("user32.dll")]
        private static extern IntPtr CreateWindowEx(
            int dwExStyle,
            string lpClassName,
            string lpWindowName,
            int dwStyle,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll")]
        private static extern bool DestroyWindow(
            IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        [DllImport("gdi32.dll")]  // ← ИСПРАВЛЕНО!
        private static extern IntPtr GetStockObject(
            int fnObject);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnableWindow(
            IntPtr hWnd,
            bool bEnable);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(
            IntPtr hWnd,
            int nCmdShow);


        // ============================================================
        // HANDLE ОСНОВНОГО ОКНА
        // ============================================================

        private readonly IntPtr _parentHandle;


        // ============================================================
        // ЭЛЕМЕНТЫ UI
        // ============================================================

        private IntPtr _topMenu;

        private IntPtr _fileButton;

        private IntPtr _settingsButton;

        private IntPtr _aboutButton;

        private IntPtr _sidePanel;

        private IntPtr _sceneLabel;

        private IntPtr _iplLabel;

        private IntPtr _iplList;


        // ============================================================
        // РАЗМЕРЫ UI
        // ============================================================

        private const int TopMenuHeight = 36;

        private const int SidePanelWidth = 260;


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public EditorUI(IntPtr parentHandle)
        {
            _parentHandle = parentHandle;

            CreateInterface();
        }


        // ============================================================
        // СОЗДАНИЕ ИНТЕРФЕЙСА
        // ============================================================

        private void CreateInterface()
        {
            // ========================================================
            // ВЕРХНЯЯ ПАНЕЛЬ - контейнер для кнопок меню
            // ========================================================

            _topMenu = CreateWindowEx(
                0,
                "STATIC",
                "",
                WS_CHILD | WS_VISIBLE,
                0,
                0,
                1000,
                TopMenuHeight,
                _parentHandle,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );


            // ========================================================
            // КНОПКА ФАЙЛ
            // ========================================================

            _fileButton = CreateWindowEx(
                0,
                "BUTTON",
                "Файл",
                WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
                10,
                8,
                80,
                24,
                _topMenu,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );


            // ========================================================
            // КНОПКА НАСТРОЙКИ
            // ========================================================

            _settingsButton = CreateWindowEx(
                0,
                "BUTTON",
                "Настройки",
                WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
                95,
                8,
                100,
                24,
                _topMenu,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );


            // ========================================================
            // КНОПКА О ПРОГРАММЕ
            // ========================================================

            _aboutButton = CreateWindowEx(
                0,
                "BUTTON",
                "О программе",
                WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
                200,
                8,
                110,
                24,
                _topMenu,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );


            // ========================================================
            // ЛЕВАЯ ПАНЕЛЬ - контейнер для элементов сцены
            // ========================================================

            _sidePanel = CreateWindowEx(
                WS_EX_CLIENTEDGE,
                "STATIC",
                "",
                WS_CHILD | WS_VISIBLE,
                0,
                TopMenuHeight,
                SidePanelWidth,
                500,
                _parentHandle,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );


            // ========================================================
            // ЗАГОЛОВОК СЦЕНЫ
            // ========================================================

            _sceneLabel = CreateControl(
                "STATIC",
                "СЦЕНА",
                10,
                TopMenuHeight + 10,
                SidePanelWidth - 20,
                25,
                0
            );


            // ========================================================
            // ЗАГОЛОВОК IPL
            // ========================================================

            _iplLabel = CreateControl(
                "STATIC",
                "IPL ФАЙЛЫ",
                10,
                TopMenuHeight + 40,
                SidePanelWidth - 20,
                25,
                0
            );


            // ========================================================
            // СПИСОК IPL
            // ========================================================

            _iplList = CreateListBox(
                10,
                TopMenuHeight + 70,
                SidePanelWidth - 20,
                400
            );


            // ========================================================
            // ТЕСТОВЫЕ IPL
            //
            // Позже этот список будет заполняться
            // автоматически из загрузчика IPL.
            // ========================================================

            AddIPLFile("gta3.ipl");

            AddIPLFile("custom.ipl");

            AddIPLFile("map.ipl");

            AddIPLFile("interior.ipl");


            // ========================================================
            // ШРИФТ
            // ========================================================

            ApplyDefaultFont();
        }


        // ============================================================
        // СОЗДАНИЕ ОБЫЧНОГО CONTROL
        // ============================================================

        private IntPtr CreateControl(
            string className,
            string text,
            int x,
            int y,
            int width,
            int height,
            int exStyle = 0)
        {
            int style = WS_CHILD | WS_VISIBLE;
            
            // Добавляем рамку только если не используется WS_EX_CLIENTEDGE
            if (exStyle == 0)
            {
                style |= WS_BORDER;
            }
            
            return CreateWindowEx(
                exStyle,
                className,
                text,
                style,
                x,
                y,
                width,
                height,
                _parentHandle,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );
        }


        // ============================================================
        // СОЗДАНИЕ КНОПКИ
        // ============================================================

        private IntPtr CreateButton(
            string text)
        {
            return CreateWindowEx(
                0,
                "BUTTON",
                text,
                WS_CHILD |
                WS_VISIBLE |
                BS_PUSHBUTTON,
                0,
                0,
                100,
                TopMenuHeight - 8,
                _topMenu,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );
        }


        // ============================================================
        // СОЗДАНИЕ LISTBOX
        // ============================================================

        private IntPtr CreateListBox(
            int x,
            int y,
            int width,
            int height)
        {
            return CreateWindowEx(
                WS_EX_CLIENTEDGE,
                "LISTBOX",
                "",
                WS_CHILD |
                WS_VISIBLE |
                WS_BORDER |
                WS_VSCROLL |
                LBS_NOTIFY |
                LBS_HASSTRINGS,
                x,
                y,
                width,
                height,
                _sidePanel,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );
        }


        // ============================================================
        // ДОБАВЛЕНИЕ IPL
        // ============================================================

        public void AddIPLFile(
            string fileName)
        {
            if (_iplList == IntPtr.Zero)
                return;


            const uint LB_ADDSTRING = 0x0180;


            SendMessage(
                _iplList,
                LB_ADDSTRING,
                IntPtr.Zero,
                Marshal.StringToHGlobalUni(fileName)
            );
        }


        // ============================================================
        // ШРИФТ
        // ============================================================

        private void ApplyDefaultFont()
        {
            IntPtr font =
                GetStockObject(
                    DEFAULT_GUI_FONT
                );


            IntPtr[] controls =
            {
                _fileButton,
                _settingsButton,
                _aboutButton,
                _sceneLabel,
                _iplLabel,
                _iplList
            };


            foreach (IntPtr control in controls)
            {
                if (control == IntPtr.Zero)
                    continue;


                SendMessage(
                    control,
                    WM_SETFONT,
                    font,
                    new IntPtr(1)
                );
            }
        }


        // ============================================================
        // RESIZE
        // ============================================================

        public void Resize(
            int width,
            int height)
        {
            if (_topMenu == IntPtr.Zero)
                return;


            // ========================================================
            // ВЕРХНЯЯ ПАНЕЛЬ - растягиваем на всю ширину
            // ========================================================

            SetWindowPos(
                _topMenu,
                IntPtr.Zero,
                0,
                0,
                width,
                TopMenuHeight,
                SWP_NOZORDER |
                SWP_NOACTIVATE
            );


            // ========================================================
            // КНОПКА ФАЙЛ
            // ========================================================

            SetWindowPos(
                _fileButton,
                IntPtr.Zero,
                10,
                8,
                80,
                24,
                SWP_NOZORDER |
                SWP_NOACTIVATE
            );


            // ========================================================
            // КНОПКА НАСТРОЙКИ
            // ========================================================

            SetWindowPos(
                _settingsButton,
                IntPtr.Zero,
                95,
                8,
                100,
                24,
                SWP_NOZORDER |
                SWP_NOACTIVATE
            );


            // ========================================================
            // КНОПКА О ПРОГРАММЕ
            // ========================================================

            SetWindowPos(
                _aboutButton,
                IntPtr.Zero,
                200,
                8,
                110,
                24,
                SWP_NOZORDER |
                SWP_NOACTIVATE
            );


            // ========================================================
            // ЛЕВАЯ ПАНЕЛЬ - растягиваем по высоте
            // ========================================================

            SetWindowPos(
                _sidePanel,
                IntPtr.Zero,
                0,
                TopMenuHeight,
                SidePanelWidth,
                Math.Max(
                    100,
                    height - TopMenuHeight
                ),
                SWP_NOZORDER |
                SWP_NOACTIVATE
            );


            // ========================================================
            // СЦЕНА - внутри sidePanel координаты относительные
            // ========================================================

            SetWindowPos(
                _sceneLabel,
                IntPtr.Zero,
                10,
                10,
                SidePanelWidth - 20,
                25,
                SWP_NOZORDER |
                SWP_NOACTIVATE
            );


            // ========================================================
            // IPL - внутри sidePanel координаты относительные
            // ========================================================

            SetWindowPos(
                _iplLabel,
                IntPtr.Zero,
                10,
                40,
                SidePanelWidth - 20,
                25,
                SWP_NOZORDER |
                SWP_NOACTIVATE
            );


            // ========================================================
            // IPL LIST - внутри sidePanel координаты относительные
            // ========================================================

            SetWindowPos(
                _iplList,
                IntPtr.Zero,
                10,
                70,
                SidePanelWidth - 20,
                Math.Max(
                    100,
                    height - TopMenuHeight - 90
                ),
                SWP_NOZORDER |
                SWP_NOACTIVATE
            );
        }


        // ============================================================
        // DISPOSE
        // ============================================================

        public void Dispose()
        {
            DestroyWindow(_iplList);

            DestroyWindow(_iplLabel);

            DestroyWindow(_sceneLabel);

            DestroyWindow(_sidePanel);

            DestroyWindow(_aboutButton);

            DestroyWindow(_settingsButton);

            DestroyWindow(_fileButton);

            DestroyWindow(_topMenu);
        }
    }
}