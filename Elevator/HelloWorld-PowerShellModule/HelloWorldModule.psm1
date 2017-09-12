function Show-HelloWorld {
    param(
        [DateTime] $date = [DateTime]::Today
    )
    Write-Host "Hello, World! Today is $date."
}

Export-ModuleMember -function Show-HelloWorld