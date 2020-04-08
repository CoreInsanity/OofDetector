$Action = New-ScheduledTaskAction -Execute "C:\Program Files (x86)\CorpInsanity\OofDetector\OofDetector.exe"
$Trigger = New-ScheduledTaskTrigger -AtLogOn
$ScheduledTask = New-ScheduledTask -Action $action -Trigger $trigger 
 
Register-ScheduledTask -TaskName "Tarkov Oof Detector" -InputObject $ScheduledTask

Start-Sleep -Seconds 2