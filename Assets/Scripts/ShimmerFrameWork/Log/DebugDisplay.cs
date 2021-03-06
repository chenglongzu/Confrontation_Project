using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace ShimmerFramework
{
    //挂载后在屏幕上显示debug信息
    class DebugDisplay : SingletonAutoMono<DebugDisplay>
    {
        struct Log
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }

        #region Inspector Settings  

        //是否开启堆栈打印
        public bool StackLog = false;

        //显示和隐藏控制台窗口的热键
        private KeyCode toggleKey = KeyCode.B;

        //是否保留一定数量的日志
        private bool restrictLogCount = true;

        //是否通过摇动设备(仅移动设备)来打开窗户
        private bool shakeToOpen = true;

        //显示字体大小
        private float FontSize = 30;

        //显示拖动条宽度
        private float ScrollbarSize = 50;

        //(平方)在上面的加速度，窗口应该打开
        private float shakeAcceleration = 100f;

        //在删除旧的日志之前保持日志的数量
        private int maxLogs = 1000;

        #endregion

        readonly List<Log> logs = new List<Log>();

        /// <summary>
        /// 对应横向、纵向滑动条对应的X,Y数值
        /// </summary>
        private Vector2 scrollPosition;

        /// <summary>
        /// 可见
        /// </summary>
        private bool visible;

        /// <summary>
        /// 折叠
        /// </summary>
        private bool collapse;

        // Visual elements:  

        private static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.magenta },
            { LogType.Log, Color.green },
            { LogType.Warning, Color.yellow },
        };

        #region OnGUI
        private const string windowTitle = "Debug（打印日志）";
        //边缘
        private const int margin = 20;

        private static readonly GUIContent clearLabel = new GUIContent("Clear", "清空打印信息.");
        private static readonly GUIContent colseLabel = new GUIContent("Close", "关闭打印面板.");
        //折叠
        private static readonly GUIContent collapseLabel = new GUIContent("Collapse", "隐藏重复信息.");

        private readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
        private Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
        #endregion

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (visible)
                {
                    visible = false;
                }
                else
                {
                    visible = true;
                }
            }

            Running();
        }

        [Conditional("EnableLog")]
        public void Running()
        {
            if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration || Input.touchCount >= 6)
            {
                visible = true;
            }
            if (Input.touchCount >= 3)
            {
                visible = true;
            }
        }

        void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            windowRect = GUILayout.Window(666, windowRect, DrawConsoleWindow, windowTitle);
        }

        /// <summary>  
        /// 显示一个列出已记录日志的窗口。
        /// </summary>  
        /// <param name="windowID">Window ID.</param>  
        private void DrawConsoleWindow(int windowID)
        {
            DrawLogsList();
            DrawToolbar();
            //允许拖动window的触发范围.  
            GUI.DragWindow(titleBarRect);
        }

        /// <summary>  
        /// 绘制log列表
        /// </summary>  
        private void DrawLogsList()
        {
            GUIStyle gs_vertica = GUI.skin.verticalScrollbar;
            GUIStyle gs1_vertica = GUI.skin.verticalScrollbarThumb;

            gs_vertica.fixedWidth = ScrollbarSize;
            gs1_vertica.fixedWidth = ScrollbarSize;

            GUIStyle gs_horizontal = GUI.skin.horizontalScrollbar;
            GUIStyle gs1_horizontal = GUI.skin.horizontalScrollbarThumb;

            gs_horizontal.fixedHeight = ScrollbarSize;
            gs1_horizontal.fixedHeight = ScrollbarSize;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            //scrollPosition = GUILayout.BeginScrollView(scrollPosition,true,true, customGuiStyle, customGuiStyle);


            for (var i = 0; i < logs.Count; i++)
            {
                var log = logs[i];

                //如果选择折叠选项，则组合相同的消息。 
                if (collapse && i > 0)
                {
                    var previousMessage = logs[i - 1].message;

                    if (log.message == previousMessage)
                    {
                        continue;
                    }
                }
                GUI.contentColor = logTypeColors[log.type];
                GUILayout.Label(log.message);
                if (StackLog)
                {
                    GUILayout.Label(log.stackTrace);
                }
            }
            GUI.color = Color.magenta;
            GUILayout.EndScrollView();

            gs_vertica.fixedWidth = 0;
            gs1_vertica.fixedWidth = 0;

            gs_horizontal.fixedHeight = 0;
            gs1_horizontal.fixedHeight = 0;

            // 在绘制其他组件之前，确保GUI颜色被重置。  
            GUI.contentColor = Color.white;
        }

        /// <summary>  
        /// Log日志工具栏
        /// </summary>  
        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(clearLabel, GUILayout.Height(40)))
            {
                logs.Clear();
            }

            if (GUILayout.Button("Stack开关", GUILayout.Height(40)))
            {
                StackLog = !StackLog;
            }

            if (GUILayout.Button(colseLabel, GUILayout.Height(40)))
            {
                visible = false;
            }
            collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(true), GUILayout.Height(40));// GUILayout.ExpandWidth保持长宽一致

            GUILayout.EndHorizontal();
        }

        /// <summary>  
        /// Debug 对应的回调处理
        /// </summary>  
        /// <param name="message">信息.</param>  
        /// <param name="stackTrace">信息的来源</param>  
        /// <param name="type">信息类型 (error, exception, warning, assert).</param>  
        private void HandleLog(string message, string stackTrace, LogType type)
        {
            logs.Add(new Log
            {
                message = "<size=" + FontSize + ">" + message + "</size>",
                stackTrace = "<size=" + FontSize + ">" + stackTrace + "</size>",
                type = type,
            });

            TrimExcessLogs();
        }

        /// <summary>  
        /// 删除超过允许的最大数量的旧日志。
        /// </summary>  
        private void TrimExcessLogs()
        {
            if (!restrictLogCount)
            {
                return;
            }

            var amountToRemove = Mathf.Max(logs.Count - maxLogs, 0);

            if (amountToRemove == 0)
            {
                return;
            }

            logs.RemoveRange(0, amountToRemove);
        }
    }
}