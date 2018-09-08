Param
(
    [string]$repo
)

Write-Host "Setting up gh-pages";

Set-Location $repo;
git symbolic-ref HEAD refs/heads/gh-pages
Remove-Item .git/index
Remove-Item "CakeScripts" -recurse -force
git clean -fdx
Write-Output "GitHub page goes here" > index.html
git add .
git commit -a -m "Initial pages commit"
git push origin gh-pages
git checkout master

Write-Host "gh-pages setup completed";