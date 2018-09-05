Param(
    [string]$repoOwner,
    [string]$repo,
    [string]$appveyortoken
);

$githubRepo = "$repoOwner/$repo";

# Set the authorization token in the header
$headers = @{};
$headers['Authorization'] = "Bearer $appveyortoken";

# Create HashTable for message body, set the values and then convert it to JSon
$bodyHash = 
@{
    repositoryProvider = "gitHub"
    repositoryName = $githubRepo
};
$body = $bodyHash | ConvertTo-Json

# Create HashTable for the Invoke-RestMethod
$paramHash = 
@{
    Uri = "https://ci.appveyor.com/api/projects" 
    Method = "Post"
    body = $body 
    ContentType = "application/json"
    Headers = $headers
    UseBasicParsing = $True
    DisableKeepAlive = $True
}

# Execute the web request
Write-Host "Requesting AppVeyor project for $githubRepo...";
$r = Invoke-RestMethod @paramHash
Write-Host "AppVeyor project added";
Write-Host "Project Id: " $r.projectId;
Write-Host "Project Name: " $r.name;
Write-Host "Url: https://ci.appveyor.com/project/$githubRepo";