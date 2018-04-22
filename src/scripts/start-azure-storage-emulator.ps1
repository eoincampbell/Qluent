$programName = 'AzureStorageEmulator'
$programPath = 'C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe'
$running = Get-Process $programName -ErrorAction SilentlyContinue
if($running -eq $null) {
	Start-Process $programPath start
}