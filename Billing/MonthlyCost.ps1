$data = (az consumption usage list --start-date "2021-03-01" --end-date "2021-03-31" | ConvertFrom-Json)

$accumCost = 0
foreach ($i in $data)
{
    if (($i.tags) -and ($i.tags.Project) -and ($i.tags.Project = "AlMualim"))
    {
        $accumCost += $i.pretaxCost
    }
}

Write-Host $accumCost
Write-Host -NoNewLine 'Press any key to continue...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');