Param
(
    [string]$repoOwner,
    [string]$repo,
    [string]$collaboratorName,
    [string]$githubToken
)

Write-Host "https://api.github.com/repos/$repoOwner/$repo/collaborators/$collaboratorName";

$head = 
@{
    Authorization = 'token ' + $githubToken
}

$paramHash = 
@{
    Uri = "https://api.github.com/repos/$repoOwner/$repo/collaborators/$collaboratorName" 
    Method = "Put"
    ContentType = "application/json"
    Headers = $head
    UseBasicParsing = $True
    DisableKeepAlive = $True
}

Write-Host "Adding collaborator for document publishing";
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$r = Invoke-RestMethod @paramHash

Write-Host "Invitation Url: " $r.html_url;