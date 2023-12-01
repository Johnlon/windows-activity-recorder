
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;


[DllImport("user32.dll")]
static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

[DllImport("user32.dll")]
static extern IntPtr GetForegroundWindow();

[DllImport("user32.dll")]
static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

Console.WriteLine("Recording!");

String lastTitle = "";
String lastProcess = "";
long focusTime = DateTime.Now.Ticks;

void Check() {
        
    StringBuilder sb = new StringBuilder(100);

    var w = GetForegroundWindow();
    if (w != null) {
        GetWindowText(w, sb, sb.Capacity);
    }
    String title = sb.ToString();

    uint pid;
    GetWindowThreadProcessId(w, out pid);
    
    string processName = "not-known";
    
    try {
        Process proc = Process.GetProcessById((int)pid);
        processName = proc.ProcessName;
    } catch(Exception ex) {
        Console.WriteLine("cant get process {0}", ex);
    }

    if (title == "Task Switching") {
        return;
    }

    if (title.Trim() == "") {
        title = "no-window-focused";
    }

    long elapsed = DateTime.Now.Ticks - focusTime;
    TimeSpan elapsedSpan = new TimeSpan(elapsed);

    if (title != lastTitle || elapsedSpan.TotalSeconds > 60) {
        
        var p = lastProcess.Replace(",", " ");
        var t = lastTitle.Replace(",", " ");
        
        Console.WriteLine("{0},{1},{2},{3}", DateTime.Now.ToString(), p, t, elapsedSpan.TotalSeconds);
 
        var home = System.Environment.GetEnvironmentVariable("USERPROFILE");
        try {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(home, "recording.csv"), true))
            {
                outputFile.WriteLine("{0},{1},{2},{3}", DateTime.Now.ToString(), p, t, elapsedSpan.TotalSeconds);
            }
        } catch(Exception ex) {
            Console.WriteLine("cany write : {0}", ex.Message);
    
            // skip exceptions like errors caused by output file open in excel 
        }

        lastTitle = title;
        lastProcess = processName;
        focusTime = DateTime.Now.Ticks;
    }
}

void doCheck(Object source, object e) {
    Check();
}

var aTimer = new System.Timers.Timer(1000);
aTimer.Elapsed += doCheck;
aTimer.AutoReset = true;
aTimer.Enabled = true;

Console.WriteLine("Monitoring... Hit enter to end.");
Console.ReadLine();