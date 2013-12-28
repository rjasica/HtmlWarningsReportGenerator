function WriteHwrdReport([Array] $types, [Object] $files, [string] $outputPath)
{
    $stream = [System.IO.StreamWriter] $outputPath
    $stream.WriteLine("<?xml version='1.0' encoding='utf-8' ?>")
    $stream.WriteLine("<report>")
    $stream.WriteLine(" <types>")

    foreach($typedef in $types)
    {
        $type=$typedef.Type
        $display=$typedef.Display
        $color=$typedef.Color
        $bgcolor=$typedef.Background
        $stream.WriteLine("  <type name='$type' display='$display' color='$color' background='$bgcolor'/>")
    }
    $stream.WriteLine(" </types>")
    $stream.WriteLine(" <files>" )
    $dir = "$(pwd)\"
    foreach($file in $files.Keys)
    {
        $fileinputname = $file
        if($fileinputname.StartsWith($dir, [System.StringComparison]::InvariantCultureIgnoreCase))
        {
            $fileinputname=$fileinputname.Substring($dir.Length)
        }

        $filename=[System.Security.SecurityElement]::Escape($fileinputname)
        $stream.WriteLine("  <file name='$filename'>")
        foreach($issue in $files[$file])
        {
            $message=[System.Security.SecurityElement]::Escape($issue.Message).Trim()
            $category=[System.Security.SecurityElement]::Escape($issue.Category).Trim()
            $type=$issue.Type.Trim()
            $line = $issue.Line
            if(($line -eq "") -or ($line -eq $null))
            {
                $line = 1
            }
            $size = 1
            if($issue.Size -gt 1)
            {
                $size = $issue.Size
            }
            $stream.Write("   <annotation type='$type' line='$line' category='$category' message='$message' size='$size'")
            if($issue.References -eq $null)
            {
                $stream.WriteLine("/>")
            }
            else
            {
                $stream.WriteLine(">")
                foreach($ref in $issue.References)
                {
                    $stream.WriteLine("   <reference name='$($ref.Name)' line='$($ref.Line)' size='$($ref.Size)'/>")
                }
                $stream.WriteLine("   </annotation>")
            }
     
        }
        $stream.WriteLine("  </file>")
    }
    $stream.WriteLine(" </files>" )
    $stream.WriteLine("</report>")
    $stream.Close()
}

function Convert-InspectCodeToHwrd([Parameter(Mandatory=$true)][string] $inputPath, [Parameter(Mandatory=$true)][string] $outputPath)
{
    <#
    .SYNOPSIS
    Convert JetBrains Reshaper InspectCode command line tool xml file to Hwrd format
    .PARAMETER inputPath 
    The path and file name of xml input file
    .PARAMETER outputPath
    The path and file name of xml output file
    #>

    $xmlfile = new-object System.Xml.XmlDocument
    $xmlfile.load([String]$inputPath)
    $messagenodes= $xmlfile.SelectNodes("//Issue")

    $files = @{}

    foreach($issue in $messagenodes)
    {
        $list = $files[$issue.File]
        if($list -eq $null)
        {
            $files[$issue.File] = $list = New-Object Collections.Generic.List[Object]
        }
        $annotation=@{Message=$issue.Message; Category=$issue.TypeId; Type="inpeccode"; Line=$issue.Line}
        $list.Add($annotation)
    }
    $types = @(@{Type="inpeccode"; Display="Code Inspection"; Color="#AB00AB"; Background="#FFB3FF"})
    WriteHwrdReport $types $files $outputPath
}

function Convert-DupfinderToHwrd([Parameter(Mandatory=$true)][string] $inputPath, [Parameter(Mandatory=$true)][string] $outputPath)
{
    <#
    .SYNOPSIS
    Convert JetBrains Reshaper DupFinder command line tool xml file to Hwrd format
    .PARAMETER inputPath 
    The path and file name of xml input file
    .PARAMETER outputPath
    The path and file name of xml output file
    #>
    $xmlfile = new-object System.Xml.XmlDocument
    $xmlfile.load([String]$inputPath)
    $nodes= $xmlfile.SelectNodes("//Duplicate")

    $files = @{}

    foreach($dup in $nodes)
    {
        $fragments = $dup.Fragment

        foreach($fragment in $fragments)
        {
            $list = $files[$fragment.FileName]
            if($list -eq $null)
            {
                $files[$fragment.FileName] = $list = New-Object Collections.Generic.List[Object]
            }
            $refs = New-Object Collections.Generic.List[Object]
            foreach($fragref in $fragments)
            {
                if($fragref -ne $fragment)
                {
                    $range = $fragref.LineRange
                    $size = $range.End - $range.Start + 1
                    $refs.Add(@{Name=$fragref.FileName; Line=$range.Start; Size=$size}) 
                }
            }
            $range = $fragment.LineRange
            $size = $range.End - $range.Start + 1
            $annotation=@{Message="Duplication with cost $($dup.Cost)"; Category="Duplication"; Type="dupfinder"; Line=$range.Start; Size=$size;References=$refs}
            $list.Add($annotation)
        }
    }

    $types = @(@{Type="dupfinder"; Display="Duplications"; Color="#FF8000"; Background="#FFD6AA"})
    WriteHwrdReport $types $files $outputPath
}

function Convert-MsbuildToHwrd([Parameter(Mandatory=$true)][string] $inputPath, [Parameter(Mandatory=$true)][string] $outputPath)
{
    <#
    .SYNOPSIS
    Deprescated. Use Convert-XmlMsbuildToHwrd and Kobush.Build XmlLogger
    .PARAMETER inputPath 
    The path and file name of xml input file
    .PARAMETER outputPath
    The path and file name of xml output file
    #>
    $content = Get-Content $inputPath | Out-String
    $regex = [regex]"(?m)^((?<file>[\w].*)\s*\((?<line>\d+)(,\d*)?\))?(MSBUILD)?\s*:\s*([Ww]arning|[Ee]rror|([Ff]atal [Ee]rror)|[Ii]nfo)\s*:\s*(?<category>.+?)\s*:\s*(?<message>.*)$"
    $caregex = [regex]"CA\d+"
    $saregex = [regex]"SA\d+"

    $files = @{}
    $matches = $regex.Matches($content)
    foreach($match in $matches)
    {
        $file=$match.Groups["file"].Value.Trim()
        $line=$match.Groups["line"].Value
        $category=$match.Groups["category"].Value
        $message=$match.Groups["message"].Value

        $list = $files[$file]
        if($list -eq $null)
        {
            $files[$file] = $list = New-Object Collections.Generic.List[Object]
        }
        
		$category = $issue.code
        if($caregex.IsMatch($category))
        {
            $type="ca"
        }
        else 
        {
            if($saregex.IsMatch($category))
            {
                $type="sa"
            }
            else
            {
                $type="msbuild"
            }
        }

        $annotation=@{Message=$issue.InnerText; Category=$category; Type=$type; Line=$issue.line}
        $list.Add($annotation)
    }

    $types = @(
        @{Type="msbuild"; Display="Msbuild"; Color="#FF0000"; Background="#F7DEDE"},
        @{Type="sa"; Display="Style Analysis"; Color="#0000FF"; Background="#C4D6FF"},
        @{Type="ca"; Display="Code Analysis"; Color="#1C797A"; Background="#88B2B3"}
    )

    WriteHwrdReport $types $files $outputPath
}

function Convert-XmlMsbuildToHwrd([Parameter(Mandatory=$true)][string] $inputPath, [Parameter(Mandatory=$true)][string] $outputPath)
{
    <#
    .SYNOPSIS
    Convert Xml Msbuild log (Kobush.Build) to Hwrg format
    .PARAMETER inputPath 
    The path and file name of xml input file
    .PARAMETER outputPath
    The path and file name of xml output file
    #>
    $xmlfile = new-object System.Xml.XmlDocument
    $xmlfile.load([String]$inputPath)
    $nodes= $xmlfile.SelectNodes("//warning | //error")

    $files = @{}
    $caregex = [regex]"CA\d+"
    $saregex = [regex]"SA\d+"
    foreach($issue in $nodes)
    {
        $filename = $issue.file;
        if(($filename -eq $null) -or ($filename -eq ""))
        {
            $filename="unknown"
        } 
        $list = $files[$filename]
        if($list -eq $null)
        {
            $files[$filename] = $list = New-Object Collections.Generic.List[Object]
        }
		
		$category = $issue.code
        if($caregex.IsMatch($category))
        {
            $type="ca"
        }
        else 
        {
            if($saregex.IsMatch($category))
            {
                $type="sa"
            }
            else
            {
                $type="msbuild"
            }
        }
        
		$annotation=@{Message=$issue.InnerText; Category=$category; Type=$type; Line=$issue.line}
		$list.Add($annotation)
    }
    $types = @(
        @{Type="msbuild"; Display="Msbuild"; Color="#FF0000"; Background="#F7DEDE"},
        @{Type="sa"; Display="Style Analysis"; Color="#0000FF"; Background="#C4D6FF"},
        @{Type="ca"; Display="Code Analysis"; Color="#1C797A"; Background="#88B2B3"}
    )
    WriteHwrdReport $types $files $outputPath
}

export-modulemember *-*