$IP_DEPLOY = $args[0]
$Origin = $args[1]
$USERNAME_DEPLOY = $args[2]
$PASSWORD_DEPLOY = $args[3]
$BASE = $args[4]
$DIRECTORYDESTINY = $args[5]

write-host "There are a total of $($args.count) arguments"

for ($i = 0; $i -lt $args.Length; $i++)
{
    # Output the current item
    Write-Host $args[$i]
}


Get-ChildItem -Path $Origin
Write-Output "ServicePath: " + $ServicePath
Write-Output "ServicePath: " + $ServiceName
Write-Output "Destino:" + $Destination


Set-Item wsman:\localhost\client\TrustedHosts -Value $IP_DEPLOY -Force
get-Item WSMan:\localhost\Client\TrustedHosts

$User = "$($IP_DEPLOY)\$($USERNAME_DEPLOY)"
Write-Output "User:" + $User
$PWord = ConvertTo-SecureString -String $PASSWORD_DEPLOY -AsPlainText -Force
$Credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $User, $PWord
$Session = New-PSSession -ComputerName $IP_DEPLOY -Credential $Credential
$ConfirmPreference = 'None'
$ServiceDescription = "Servicio Windows rca_gtm_synergy_suite_ServBoHExtInventarioTrasladoSS"
# $Origin = $WORKSPACE
#$DirectoryDestiny = $DIRECTORYDESTINY
$Destination = "$($BASE)\$($DIRECTORYDESTINY)\"
Write-Output "Destination:" + $Destination
$ServicePath = "$($Destination)$($Origin)\ServBoHExtInventarioTrasladoSS.exe"
Write-Output "ServicePath:" + $ServicePath
$ServiceName = "Service_$($Destination)".Replace("\","_").Replace(":","").TrimEnd("_")

Test-NetConnection $IP_DEPLOY -Port 5985

# Invoke-Command -Session $Session -ScriptBlock {
#     Get-Culture
# }


$resultQueryDestiny = Invoke-Command -Session $Session -ScriptBlock {
     param($Destination)
     Get-ChildItem -Path $Destination -ErrorAction SilentlyContinue
} -ArgumentList $Destination

Write-Output $resultQueryDestiny

if(!($resultQueryDestiny)){
    Invoke-Command -Session $Session -ScriptBlock {
        param($BASE, $DIRECTORYDESTINY)
        New-Item -Path $BASE -Name $DIRECTORYDESTINY -ItemType "directory" -ErrorAction SilentlyContinue
    } -ArgumentList $BASE, $DIRECTORYDESTINY
}

$service = Invoke-Command -Session $Session -ScriptBlock {
    param($ServiceName)
    Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
}-ArgumentList $ServiceName

Write-Output $service.Status

if(!($service))
{
    Write-Output "No hay servicio"
    Copy-Item $Origin -Destination $Destination -ToSession $Session -Recurse -Force
    Invoke-Command -Session $Session -ScriptBlock {
        param($ServiceName,$ServicePath,$ServiceDescription)
        New-Service -Name $ServiceName -BinaryPathName $ServicePath -Description $ServiceDescription
        Set-Service -Name $ServiceName -StartupType Automatic
    }-ArgumentList $ServiceName,$ServicePath,$ServiceDescription
}
else
{
    if (!($service.Started))
    {
        Write-Output "Servicio Existe"
        Invoke-Command -Session $Session -ScriptBlock {
            param($ServiceName)
            Stop-Service -Name $ServiceName
        }-ArgumentList $ServiceName
        Copy-Item $Origin -Destination $Destination -ToSession $Session -Recurse -Force
    } 
}
