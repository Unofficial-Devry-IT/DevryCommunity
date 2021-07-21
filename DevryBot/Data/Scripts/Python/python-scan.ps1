Param(
    [Parameter(Mandatory=$true,
     ValueFromPipeline=$true,
     ParameterSetName="profile")]
    [String]
    $profile,

    [Parameter(Mandatory=$true, ValueFromPipeline=$true, ParameterSetName="scan")]
    [String]
    $scan,

    [Parameter(Mandatory=$true, ValueFromPipeline=$true, ParameterSetName="name")]
    [String]
    $name
)
prospector --profile-path $profile --output-format json  $scan > $name.json