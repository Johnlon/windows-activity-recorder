
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
bool first = true;
long focusTime = DateTime.Now.Ticks;

var fmt = "{0,-20},{1,-10},{2,-20},{3}";
        

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
        
        if (first){
            // skip first call as dont have prev program 
            first = false;
        } else {
            var p = lastProcess.Replace(",", " ");
            var t = lastTitle.Replace(",", " ");
            var elapsedSecs = (int)elapsedSpan.TotalSeconds;

            Console.WriteLine(fmt, DateTime.Now.ToString(), elapsedSecs, p, t);
    
            var home = System.Environment.GetEnvironmentVariable("USERPROFILE");
            if (home == null) {
                Console.WriteLine("USERPROFILE env var not defined - set it to where you want the log file written - no file logging will occur"); 
            } else {

                try {
                    var path = Path.Combine(home, "recording.csv");
                    var exists = File.Exists(path);

                    using (StreamWriter outputFile = new StreamWriter(path, true))
                    {
                        if (!exists) {
                            outputFile.WriteLine(fmt, "datetime", "duration", "program", "title");
                        }

                        outputFile.WriteLine(fmt, DateTime.Now.ToString(), elapsedSecs, p, t);
                    }
                } catch(Exception ex) {
                    Console.WriteLine("cant write : {0}", ex.Message);
                    // skip exceptions like errors caused by output file open in excel 
                }
            }
        }
        lastTitle = title;
        lastProcess = processName;
        focusTime = DateTime.Now.Ticks;
    }
}

void doCheck(Object? source, object? e) {
    Check();
}

var aTimer = new System.Timers.Timer(1000);
aTimer.Elapsed += doCheck;
aTimer.AutoReset = true;
aTimer.Enabled = true;

Console.WriteLine("Monitoring... Hit enter to end.");
Console.WriteLine(fmt, "datetime", "duration", "program", "title");
        
Console.ReadLine();
