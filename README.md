HtmlWarningsReportGenerator
===========================

Comand line tool generating html report from xml. It render warnings, errors and adnotations in source code. It can be easy integrated with continous integration servers and reports can be published as artifacts.

## Nuget 

Nuget package http://nuget.org/packages/HtmlWarningsReportGenerator/

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package HtmlWarningsReportGenerator

## Sample output

Index:

![Index](https://raw.github.com/rjasica/HtmlWarningsReportGenerator/master/web/index.png)

Sample warnings:

![Warnings](https://raw.github.com/rjasica/HtmlWarningsReportGenerator/master/web/warnings.png)

Reference to other file:

![Duplication](https://raw.github.com/rjasica/HtmlWarningsReportGenerator/master/web/duplication.png)

## Usage

Sample

    HWRG.exe --verbose --files r1.xml r2.xml r3.xml --outdir .\testreport
    
Help

  -f, --files      Required. Files to parse.
  -t, --type       (Default: html) Output report type.
  -o, --outdir     Required. Output directory..
  -v, --verbose    Display information messages.
  --help           Display this help screen.

## Converting MSbuild report with Style and Code Analysis

Use [Kobush.Build XmlLoger](https://github.com/rjasica/Kobush.Build)

    msbuild solution.sln /logger:"PATH\Kobush.Build.dll";msbuildlog.xml
    
    Import-Module PACKAGEPATH\tools\converters.psm1
    Convert-XmlMsbuildToHwrd msbuildlog.xml msbuildlog.hwrg
    
## Converting ReSharper Command Line Tools  

Download new version of [ReSharper Command Line Tools ](http://www.jetbrains.com/resharper/features/command-line.html) and create xml report

    Import-Module PACKAGEPATH\tools\converters.psm1
    Convert-InspectCodeToHwrd inspectcode.xml inspectcode.hwrg   
    Convert-DupfinderToHwrd dupfinder.xml dupfinder.hwrg 

## Sample input xml file

    <?xml version='1.0' encoding='utf-8' ?>
    <report>
      <types>
        <!-- Optional. Display settings  -->
        <type name='dupfinder' display='Duplications' color='#FF8000' background='#FFD6AA'/>
      </types>
      <files>
        <file name='File1.cs'>
        <!-- Contains warnings for file  -->
          <annotation type='msbuild' line='12' category='CS0001' message='Sample error message'/>
        </file>
        <file name='Path\File2.cs'>
          <annotation type='dupfinder' line='37' category='Duplication' message='Duplication with cost 88' size='9'>
            <!-- saple reference to other file -->
    		<reference name='File1.cs' line='12' size='9'/>
          </annotation>
        </file>
      </files>
    </report>


## Icon

Research by Brennan Novak from The [The Noun Project](http://thenounproject.com)

