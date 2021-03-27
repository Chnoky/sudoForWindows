NAME

sudoWs - sudo and sudoers for Windows



DESCRIPTION 

sudoWs is a client-server tool for Windows to run commands and powershell scripts with an elevated account without UAC prompt.



FILES 

C:\Program Files\sudoWs\client\
Contains sudoWs client side. Non elevated users can read and execute in this folder.

--

C:\Program Files\sudoWs\server\
Contains sudoWs server side. Only administrators can access to this folder.

--
C:\Program Files\sudoWs\sudoers\
Contains command allowed to run with sudoWs. Only administrators can access to this folder.

--
C:\Program Files\sudoWs\certificate\
Contains the server certificate to authenticate the server. Only administrators can access to this folder.



INSTALLATION

Copy or download the whole installer folder and run "install_sudoWs.ps1" powershell script.
You must run the installer script with a Windows administrator account.

Next, configure the sudoers file C:\Program Files\sudoWs\sudoers\sudoers.txt to allow users or groups to run elevated commands or scripts.



HOW TO USE sudoWs ?

In command console or powershell console :
Run "sudoWs.exe <Path to script | Powershell command>"

If the args is not in sudoers, execution is denied.
If the args is in sudoers but not with NOPASSWD option, UAC for Adminisrator password is prompting.
If the args is in sudoers with NOPASSWD option, the command or script is run with elevated account (SYSTEM) and the standard output is return.