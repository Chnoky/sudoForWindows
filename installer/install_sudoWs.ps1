$sudoWs_path = "C:\Program Files\sudoWs"
$server_path = "C:\Program Files\sudoWs\server"
$client_path = "C:\Program Files\sudoWs\client"
$sudoers_path = "C:\Program Files\sudoWs\sudoers"
$cert_path = "C:\Program Files\sudoWs\certificate"

### for debug
#$sudoWs_path = "C:\test\sudoWs"
#$server_path = "C:\test\sudoWs\server"
#$client_path = "C:\test\sudoWs\client"
#$sudoers_path = "C:\test\sudoWs\sudoers"
#$cert_path = "C:\test\sudoWs\certificate"

$configFile = $server_path+"\sudoWs_server.dll.config"


### check if current powershell is elevated ###

$current = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())

if(!$current.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)){
	Write-Host "You must run this setup with Administrators rights"
	exit 127
}


### Stop server scheduled task ###

Get-ScheduledTask -TaskName "sudoWs_server" -ErrorAction "SilentlyContinue"
$ret = $?

If($ret){
	Stop-ScheduledTask -TaskName "sudoWs_server"
}

Start-Sleep 3


### Remove old folders ###

If(Test-Path $server_path){
	$acl = Get-Acl $server_path
	$rule = New-Object System.Security.AccessControl.FileSystemAccessRule($env:UserName,"FullControl","ContainerInherit,ObjectInherit","None","Allow")
	$acl.SetAccessRule($rule)
	Set-Acl -Path $server_path -AclObject $acl
	Remove-Item -Path $server_path -Recurse -Force -ErrorAction "Stop"
}
If(Test-Path $client_path){
	$acl = Get-Acl $client_path
	$rule = New-Object System.Security.AccessControl.FileSystemAccessRule($env:UserName,"FullControl","ContainerInherit,ObjectInherit","None","Allow")
	$acl.SetAccessRule($rule)
	Set-Acl -Path $client_path -AclObject $acl
	Remove-Item -Path $client_path -Recurse -Force -ErrorAction "Stop"
}
If(Test-Path $sudoers_path){
	$acl = Get-Acl $sudoers_path
	$rule = New-Object System.Security.AccessControl.FileSystemAccessRule($env:UserName,"FullControl","ContainerInherit,ObjectInherit","None","Allow")
	$acl.SetAccessRule($rule)
	Set-Acl -Path $sudoers_path -AclObject $acl
	Remove-Item -Path $sudoers_path -Recurse -Force -ErrorAction "Stop"
}
If(Test-Path $cert_path){
	$acl = Get-Acl $cert_path
	$rule = New-Object System.Security.AccessControl.FileSystemAccessRule($env:UserName,"FullControl","ContainerInherit,ObjectInherit","None","Allow")
	$acl.SetAccessRule($rule)
	Set-Acl -Path $cert_path -AclObject $acl
	Remove-Item -Path $cert_path -Recurse -Force -ErrorAction "Stop"
}
If(Test-Path $sudoWs_path){
	Remove-Item -Path $sudoWs_path -Force -ErrorAction "Stop"
}


### Create new folders ###

New-Item -Path "C:\Program Files\sudoWs" -Type "Directory"
#New-Item -Path "C:\test\sudoWs" -Type "Directory"
Copy-Item -Recurse -Path "./bin/server/" -Destination "$server_path"
Copy-Item -Recurse -Path "./bin/client/" -Destination "$client_path"
Copy-Item -Recurse -Path "./bin/sudoers/" -Destination "$sudoers_path"

New-Item -Path "$cert_path" -Type "Directory"




### Install ###


### generate server certificate

### TODO ###

$cert = New-SelfSignedCertificate -CertStoreLocation "Cert:\CurrentUser\My" -HashAlgorithm "sha512" -KeyAlgorithm "RSA" -KeyLength "2048" -KeyDescription "sudoWs certificate" -KeyExportPolicy "Exportable" -KeyUsage "CertSign" -Subject "sudoWs" -NotAfter (Get-Date).AddYears(100)
$fingerPrint = $cert.Thumbprint

$randomPassword =  ("!@#$%^&*0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz".tochararray() | sort {Get-Random})[0..20] -join ''

$encryptedPassword = ConvertTo-SecureString -String $randomPassword -Force -AsPlainText

Export-PfxCertificate -Cert Cert:\currentuser\my\$fingerPrint -FilePath $cert_path\sudoWs_certificate.pfx -Password $encryptedPassword -Force

$xmlConfigFile = Get-Content $configFile
$xmlConfigFile | ForEach-Object { $_.Replace('<add key="certificate_password" value="none" />', '<add key="certificate_password" value="'+$randomPassword+'" />') } | Set-Content $configFile

Get-Item Cert:\CurrentUser\My\$fingerPrint | Remove-Item

### configure scheduled task

Get-ScheduledTask -TaskName "sudoWs_server" -ErrorAction "SilentlyContinue"
$ret = $?

If($ret){
	Unregister-ScheduledTask -TaskName "sudoWs_server" -Confirm:$False
}

Register-ScheduledTask -Xml (get-content "./task/server_task.xml" | out-string) -TaskName "sudoWs_server" | Enable-ScheduledTask



### Set permissions on folders ###


### remove inheritance on server, sudoers and certificate folders
### set ACL for Administators only

$newAcl = New-Object System.Security.AccessControl.DirectorySecurity
$newAcl.SetAccessRuleProtection($True, $True)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Administrators","ReadAndExecute","ContainerInherit,ObjectInherit","None","Allow")
$newAcl.SetAccessRule($rule)
Set-Acl -Path $server_path -AclObject $newAcl

$newAcl = New-Object System.Security.AccessControl.DirectorySecurity
$newAcl.SetAccessRuleProtection($True, $True)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Administrators","Read,Write","ContainerInherit,ObjectInherit","None","Allow")
$newAcl.SetAccessRule($rule)
Set-Acl -Path $sudoers_path -AclObject $newAcl

$newAcl = New-Object System.Security.AccessControl.DirectorySecurity
$newAcl.SetAccessRuleProtection($True, $True)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Administrators","Read","ContainerInherit,ObjectInherit","None","Allow")
$newAcl.SetAccessRule($rule)
Set-Acl -Path $cert_path -AclObject $newAcl


### set ReadAndExecute for Users

$newAcl = New-Object System.Security.AccessControl.DirectorySecurity
$newAcl.SetAccessRuleProtection($True, $True)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Users","ReadAndExecute","ContainerInherit,ObjectInherit","None","Allow")
$newAcl.SetAccessRule($rule)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Administrators","FullControl","ContainerInherit,ObjectInherit","None","Allow")
$newAcl.SetAccessRule($rule)
Set-Acl -Path $client_path -AclObject $newAcl


