param(
    [hashtable] $properties = @{}
)

import-module ($PSScriptRoot + '\..\tools\psake-4.6.0\psake.psm1')

try {
    invoke-psake ($PSScriptRoot + '\build.ps1') -properties $properties
}
finally
{
    remove-module psake
}