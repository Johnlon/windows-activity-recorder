# windows-activity-recorder

Windows activity recorder - silently logs the time spent with active focus within applications

This was written to help me understand where I'm spending my time, but without me needing to think too hard about it.

# build

I built this using VSCode.  I had to first install the dotnet framework on Windows and then C# plugin into VSCode. 

But, if you install dotnet alone then its possible to build from the command line ...

`
dotnet build Recorder.sln
`

I used dotnet 8 as that was current at the time and the binary gets written to ....

`
bin\Debug\net8.0\Recorder.exe
`


# usage

Just run the binary.

An activity log file is written to %USERPROFILE%/recording.csv

The log files should be openable in excel.

NB: If you open the file in most windows apps then the file will be locked and will stop recording updates until you close the file.

The format of the log file is:    `datetime,process-name,window-title,time-spent-in-window`

New entries are written to the log file when the window focus is changed and/or every 60 seconds.

I'm not an excel expert but it's easy enough to process that CSV file in excel to extract info.