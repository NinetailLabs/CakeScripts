Write-Host "Setting up gh-pages";

git symbolic-ref HEAD refs/heads/gh-pages
Remove-Item .git/index
Remove-Item "CakeScripts" -recurse -force
git clean -fdx
Write-Output "GitHub page goes here" > index.html
git add .
git commit -a -m "Initial pages commit"
git push origin gh-pages
git checkout master
git pull --recurse-submodules

Write-Host "gh-pages setup completed";