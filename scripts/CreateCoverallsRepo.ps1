Param(
    [string]$repoOwner,
    [string]$repo,
    [string]$coverallstoken
)

$githubRepo = "$repoOwner/$repo";

# Set the authorization token in the header
$headers = @{};
$headers['Authorization'] = "token $coverallstoken";

# Create HashTable for message body, set the values and then convert it to JSon
$bodyHash = 
@{
    service = "gitHub"
    name = $githubRepo
};
$repoHash =
@{
    repo = $bodyHash
};
$body = $repoHash | ConvertTo-Json

# Create HashTable for the Invoke-RestMethod
$paramHash = 
@{
    Uri = "https://coveralls.io/api/repos" 
    Method = "Post"
    body = $body 
    ContentType = "application/json"
    Headers = $headers
    UseBasicParsing = $True
    DisableKeepAlive = $True
}

# Execute the web request
Write-Host "Requesting Coveralls project for $githubRepo...";
$r = Invoke-RestMethod @paramHash
Write-Host "Coveralls project added";
Write-Host "Url: https://coveralls.io/github/$githubRepo";