$pfx_cert = Get-Content  -AsByteStream .\setup_TemporaryKey.pfx
$encoded = [System.Convert]::ToBase64String($pfx_cert)
Set-Clipboard $encoded
Write-Output "copyed to clipboard"
Write-Output $encoded

