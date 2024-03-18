$IP_DEPLOY = $args[0]
$Origin = $args[1]
$USERNAME_DEPLOY = $args[2]
$PASSWORD_DEPLOY = $args[3]
$DIRECTORYDESTINY = $args[4]
$DIRECTORYDESTINYC = $args[5]
$SiteName = $args[6]
$AppPoolName = $args[7]
$portUse = $args[8]

write-host "There are a total of $($args.count) arguments"

for ($i = 0; $i -lt $args.Length; $i++) {
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
$so = New-PSSessionOption -IdleTimeout 600000
$Session = New-PSSession -ComputerName $IP_DEPLOY -Credential $Credential -SessionOption $so


$Destination = "$DIRECTORYDESTINY"
$OriginPath = $Origin
Write-Output "Destination:" + $Destination

Test-NetConnection $IP_DEPLOY -Port 5985

# Verificar que la carpeta de origen exista
if (!(Test-Path $OriginPath -PathType Container)) {
    Write-Host "La carpeta de origen '$OriginPath' no existe."
    exit
}

# Obtener el contenido de la carpeta local Ws_OLS
$resultQueryOrigin = Get-ChildItem -Path $OriginPath -ErrorAction SilentlyContinue

Write-Output $resultQueryOrigin

# Verificar si el IIS está instalado
$IISInstalled = Invoke-Command -Session $Session -ScriptBlock {
    return (Test-Path "IIS:\")
}

if ($IISInstalled) {
    Write-Host "IIS está instalado en el servidor."
    
    # Verificar la existencia del IIS Management
    $IISManagementExists = Invoke-Command -Session $Session -ScriptBlock {
        return (Get-Module -Name WebAdministration -ListAvailable)
    }

    if ($IISManagementExists) {
        Write-Host "El IIS Management está instalado."
    }
    else {
        Write-Host "El IIS Management no está instalado."
    }

    # Verificar la existencia de los Application Pools
    $AppPools = Invoke-Command -Session $Session -ScriptBlock {
        Import-Module WebAdministration
        return Get-ChildItem IIS:\AppPools
    }

    if ($AppPools) {
        Write-Host "Los Application Pools existen en el servidor."
        
        # Verificar si el Application Pool específico existe
        $AppPoolExists = $AppPools | Where-Object { $_.Name -eq $AppPoolName }

        if (-not $AppPoolExists) {
            # Si el Application Pool no existe, crearlo
            Invoke-Command -Session $Session -ScriptBlock {
                Import-Module WebAdministration
                New-WebAppPool -Name $using:AppPoolName
            }
            Write-Host "Application Pool '$AppPoolName' creado."
        }
        else {
            Write-Host "El Application Pool '$AppPoolName' ya existe."
        }
    }
    else {
        Write-Host "No se encontraron Application Pools en el servidor."
    }
}
else {
    Write-Host "IIS no está instalado en el servidor."
}

Invoke-Command -Session $Session -ScriptBlock {
    param($SiteName)
    
    Import-Module WebAdministration
    
    # Verificar si el sitio web existe
    $Site = Get-Item "IIS:\Sites\$SiteName" -ErrorAction SilentlyContinue
    
    if ($Site -ne $null) {
        # Detener el sitio web
        Stop-Website -Name "$SiteName"
        Write-Host "Sitio '$SiteName' detenido."
    }
    else {
        Write-Host "El sitio web '$SiteName' no existe."
        exit
    }
} -ArgumentList $SiteName


# Verificar si la carpeta de destino existe en el servidor remoto
$resultQueryDestiny = Invoke-Command -Session $Session -ScriptBlock {
    param($DIRECTORYDESTINYC)
    Test-Path $DIRECTORYDESTINYC
} -ArgumentList $DIRECTORYDESTINYC

# Verificar el valor de $resultQueryDestiny
Write-Host "El directorio existe: $resultQueryDestiny"

if (-not $resultQueryDestiny) {
    Write-Host "La carpeta de destino '$Destination' no existe en el servidor remoto. Creándola..."

    # Crear la carpeta de destino en el servidor remoto
    Invoke-Command -Session $Session -ScriptBlock {
        param($DIRECTORYDESTINYC)
        New-Item -Path "$DIRECTORYDESTINYC"  -ItemType Directory -Force
    } -ArgumentList "$DIRECTORYDESTINYC"

    Write-Host "Carpeta de destino creada."
}

# Verificar si el sitio web existe en el servidor remoto
Invoke-Command -Session $Session -ScriptBlock {
    param($SiteName, $Destination)
    
    Import-Module WebAdministration
    
    # Obtener el sitio web
    $Site = Get-Item "IIS:\Sites\$SiteName" -ErrorAction SilentlyContinue
    
    if ($Site -ne $null) {
        # Modificar el PhysicalPath del sitio web
        $Site | Set-ItemProperty -Name "physicalPath" -Value $DIRECTORYDESTINY
        Write-Output "PhysicalPath del sitio '$SiteName' modificado correctamente a '$DIRECTORYDESTINY'"
    }
    else {
        Write-Output "No se encontró el sitio web '$SiteName'"
    }
} -ArgumentList $SiteName, $DIRECTORYDESTINY

$resultQueryWebsite = Invoke-Command -Session $Session -ScriptBlock {
    param($SiteName)
    Import-Module WebAdministration
    if (Get-Website -Name $SiteName -ErrorAction SilentlyContinue) {
        $true
    }
    else {
        $false
    }
} -ArgumentList $SiteName

if (-not $resultQueryWebsite) {
    Write-Host "El sitio web '$SiteName' no existe en el servidor remoto. Creándolo..."

    # Crear el sitio web en el servidor remoto
    Invoke-Command -Session $Session -ScriptBlock {
        param($SiteName, $PhysicalPath, $portUse)
        Import-Module WebAdministration
        New-Website -Name $SiteName -PhysicalPath $PhysicalPath -Port $portUse -Force
    } -ArgumentList $SiteName, $Destination, $portUse

    Write-Host "Sitio web creado."
}

# Eliminar todos los archivos y carpetas dentro del destino, excluyendo 'aspnet_client'
Invoke-Command -Session $Session -ScriptBlock {
    param($DIRECTORYDESTINYC)
    Get-ChildItem -Path $DIRECTORYDESTINYC | Where-Object { $_.Name -ne 'aspnet_client' } | Remove-Item -Recurse -Force
} -ArgumentList $DIRECTORYDESTINYC

Write-Host "Archivos anteriores eliminados."
$Destination1 = "C:\inetpub\wwwroot1"
# Copiar el contenido de la carpeta local Ws_OLS en el servidor remoto
Copy-Item -Path "$Origin\*" -Destination $Destination1 -ToSession $Session -Recurse -Force
Write-Host "Se copian archivos a ruta destino."

# Iniciar el sitio web en el servidor remoto
Invoke-Command -Session $Session -ScriptBlock {
    param($SiteName)
    Import-Module WebAdministration
    Start-Website -Name "$SiteName"
} -ArgumentList $SiteName

Write-Host "IIS iniciado."
Remove-PSSession $Session
