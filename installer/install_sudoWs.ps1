$sudoWs_path = "C:\Program Files\sudoWs"
$server_path = "C:\Program Files\sudoWs\server"
$client_path = "C:\Program Files\sudoWs\client"
$sudoers_path = "C:\Program Files\sudoWs\sudoers"

### for debug
#$sudoWs_path = "C:\test\sudoWs"
#$server_path = "C:\test\sudoWs\server"
#$client_path = "C:\test\sudoWs\client"
#$sudoers_path = "C:\test\sudoWs\sudoers"

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

Remove-Item -Path $sudoWs_path -Force -ErrorAction "Stop"

#New-Item -Path "C:\Program Files\sudoWs" -Type "Directory"
New-Item -Path "C:\test\sudoWs" -Type "Directory"
Copy-Item -Recurse -Path "./bin/server/" -Destination "$server_path"
Copy-Item -Recurse -Path "./bin/client/" -Destination "$client_path"
Copy-Item -Recurse -Path "./bin/sudoers/" -Destination "$sudoers_path"

### remove inheritance on server and sudoers folders
### set ACL for Administators only

$newAcl = New-Object System.Security.AccessControl.DirectorySecurity
$newAcl.SetAccessRuleProtection($True, $True)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Administrators","ReadAndExecute","ContainerInherit,ObjectInherit","InheritOnly","Allow")
$newAcl.SetAccessRule($rule)
Set-Acl -Path $server_path -AclObject $newAcl

$newAcl = New-Object System.Security.AccessControl.DirectorySecurity
$newAcl.SetAccessRuleProtection($True, $True)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Administrators","Read,Write","ContainerInherit,ObjectInherit","InheritOnly","Allow")
$newAcl.SetAccessRule($rule)
Set-Acl -Path $sudoers_path -AclObject $newAcl


### set ReadAndExecute for Users

$newAcl = New-Object System.Security.AccessControl.DirectorySecurity
$newAcl.SetAccessRuleProtection($True, $True)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Users","ReadAndExecute","ContainerInherit,ObjectInherit","InheritOnly","Allow")
$newAcl.SetAccessRule($rule)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Administrators","FullControl","ContainerInherit,ObjectInherit","InheritOnly","Allow")
$newAcl.SetAccessRule($rule)
Set-Acl -Path $client_path -AclObject $newAcl


### generate server certificate

### TODO ###


### configure scheduled task

Get-ScheduledTask -TaskName "sudoWs_server" -ErrorAction "SilentlyContinue"
$ret = $?

If($ret){
	Unregister-ScheduledTask -TaskName "sudoWs_server" -Confirm:$False
}

Register-ScheduledTask -Xml (get-content "./task/server_task.xml" | out-string) -TaskName "sudoWs_server" | Enable-ScheduledTask