$publisher = 'CN=lcassin'   # Must match Package.appxmanifest Publisher
$cert = New-SelfSignedCertificate `
  -Type Custom `
  -Subject $publisher `
  -CertStoreLocation Cert:\CurrentUser\My `
  -KeyAlgorithm RSA -KeyLength 2048 `
  -HashAlgorithm SHA256 `
  -KeyExportPolicy Exportable `
  -KeyUsage DigitalSignature `
  -TextExtension @("2.5.29.19={critical}{text}ca=FALSE","2.5.29.37={text}1.3.6.1.5.5.7.3.3") `
  -NotAfter (Get-Date).AddYears(2)

New-Item -ItemType Directory -Force -Path "C:\Temp" | Out-Null
$pfxPath = "C:\Temp\ConstructionCalculator_Dev.pfx"
$cerPath = "C:\Temp\ConstructionCalculator_Dev.cer"
$pfxPwd = Read-Host -AsSecureString "Enter a password to protect the PFX"
Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $pfxPwd
Export-Certificate -Cert $cert -FilePath $cerPath | Out-Null
Import-Certificate -FilePath $cerPath -CertStoreLocation Cert:\CurrentUser\TrustedPeople | Out-Null

cd "c:\Users\LCassin\source\repos\Construction_Calculator\ConstructionCalculatorMAUI"
dotnet publish .\ConstructionCalculatorMAUI.csproj -c Release -f net8.0-windows10.0.19041.0

# Sign the new package
$signtool = (Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\bin\*\x64\signtool.exe" -Recurse -ErrorAction SilentlyContinue | Select-Object -Last 1).FullName
$pkg = Get-ChildItem -Recurse -Filter *.msix .\AppPackages | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$pfxPath = "C:\Temp\ConstructionCalculator_Dev.pfx"
$pfxPassword = Read-Host "Enter the PFX password"
& $signtool sign /fd SHA256 /f $pfxPath /p $pfxPassword $pkg.FullName

# Double-click to install, or use PowerShell:
Start-Process $pkg.FullName
