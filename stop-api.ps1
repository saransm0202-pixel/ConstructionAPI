# Stop anything running on port 7179 / any SSConstructions instance.
# Run this BEFORE pressing F5 in Visual Studio if you get "address already in use".

$Port = 7179
$Exe  = 'SSConstructions'

$listening = Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue |
    Where-Object { $_.LocalPort -eq $Port }
foreach ($c in $listening) {
    Stop-Process -Id $c.OwningProcess -Force -ErrorAction SilentlyContinue
}
Get-Process -Name $Exe -ErrorAction SilentlyContinue |
    ForEach-Object { Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue }

Start-Sleep -Seconds 2

$left = @(Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue |
    Where-Object { $_.LocalPort -eq $Port }).Count
if ($left -eq 0) {
    Write-Host 'Port 7179 is free - you can run the app now.' -ForegroundColor Green
} else {
    Write-Host 'Still in use - check for another app on port 7179.' -ForegroundColor Red
}