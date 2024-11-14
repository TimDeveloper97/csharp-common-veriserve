@echo off
setlocal

set "PipeName=PipeName"
set "DataToSend=Hello from another process!"

echo Sending data to MAUI app...

powershell -command "& { $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', '%PipeName%', 'Out'); $pipe.Connect(); $writer = new-object System.IO.StreamWriter($pipe); $writer.Write('%DataToSend%'); $writer.Flush(); $writer.Close(); $pipe.Close(); }"

echo Data sent successfully!
pause
