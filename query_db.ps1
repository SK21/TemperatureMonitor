$dbs = Get-ChildItem -Path 'F:/Documents/GitHub/TemperatureMonitor' -Recurse -Filter '*.db' | Select-Object -ExpandProperty FullName
foreach ($db in $dbs) {
    [Reflection.Assembly]::LoadFile((Resolve-Path 'F:/Documents/GitHub/TemperatureMonitor/BinWatchApp/System.Data.SQLite.dll').Path) | Out-Null
    $cn = New-Object Data.SQLite.SQLiteConnection "Data Source=$db"
    $cn.Open()
    $cmd = $cn.CreateCommand()
    $cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'"
    $r = $cmd.ExecuteReader()
    $tables = @()
    while ($r.Read()) { $tables += $r.GetValue(0) }
    $cn.Close()
    Write-Host "$db  ->  $($tables -join ', ')"
}
