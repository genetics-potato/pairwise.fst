# OMIM_CLI [version 1.0.0.0]

**(OMIM) Online Mendelian Inheritance in Man CLI query tools**
_SMRUCC Clinic tools for Online Mendelian Inheritance in Man CLI query_
Copyright © xie.guigang@gcmodeller.org 2016

**Module AssemblyName**: file:///C:/Users/xieguigang/OneDrive/SMRUCC.Medical/CLI/bin/Release/OMIM.exe
**Root namespace**: OMIM_CLI.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|[/debug](#/debug)|Displays the debugging information from the OMIM API server.|
|[/Query.clinicalSynopsis](#/Query.clinicalSynopsis)||
|[/Query.entry](#/Query.entry)|Gets OMIM data by mim entry number.|
|[/Query.geneMap.entry](#/Query.geneMap.entry)||
|[/Query.geneMap.loci](#/Query.geneMap.loci)||
|[/Query.geneMap.seqID](#/Query.geneMap.seqID)||
|[/Search.clinicalSynopsis](#/Search.clinicalSynopsis)||
|[/Search.entry](#/Search.entry)||
|[/Search.geneMap](#/Search.geneMap)||
|[/Set](#/Set)|Setup the OMIM API tools variables.|
|[/Set.Key](#/Set.Key)|Setup the OMIM API key for query the database.|
|[/vars](#/vars)|Listing all of the variables for using the `/Set` command.|

> 
> ### OMIM® - Online Mendelian Inheritance in Man®
> 
> Welcome to OMIM®, Online Mendelian Inheritance in Man®. OMIM is a comprehensive, authoritative compendium of human genes and genetic phenotypes that is freely available and updated daily. 
> The full-text, referenced overviews in OMIM contain information on all known mendelian disorders and over 15,000 genes. OMIM focuses on the relationship between phenotype and genotype. 
> It is updated daily, and the entries contain copious links to other genetics resources.		
> 
> This database was initiated in the early 1960s by Dr. Victor A. McKusick as a catalog of mendelian traits and disorders, entitled Mendelian Inheritance in Man (MIM). 
> Twelve book editions of MIM were published between 1966 and 1998. The online version, OMIM, was created in 1985 by a collaboration between the National Library of Medicine and the William H. Welch Medical Library at Johns Hopkins. 
> It was made generally available on the internet starting in 1987. In 1995, OMIM was developed for the World Wide Web by NCBI, the National Center for Biotechnology Information.
> 
> OMIM is authored and edited at the McKusick-Nathans Institute of Genetic Medicine, Johns Hopkins University School of Medicine, under the direction of Dr. Ada Hamosh.
>                                     
> Copyright® 1966-2016 Johns Hopkins University.
> 
> The OMIM® database including the collective data contained therein is the property of the Johns Hopkins University, which holds the copyright thereto.             
> The OMIM database is made available to the general public subject to certain restrictions. You may use the OMIM database and data obtained from this site for your personal use, for educational or scholarly use, or for research purposes only. 
> The OMIM database may not be copied, distributed, transmitted, duplicated, reduced or altered in any way for commercial purposes, or for the purpose of redistribution, without a license from the Johns Hopkins University.
> Requests for information regarding a license for commercial use or redistribution of the OMIM database may be sent via e-mail to JHTT-Communications@jhmi.edu.

## CLI API list
--------------------------
<h3 id="/debug"> 1. /debug</h3>

Displays the debugging information from the OMIM API server.
**Prototype**: ``OMIM_CLI.CLI::Int32 Debug()``

###### Usage
```bash
OMIM
```
###### Example
```bash
OMIM
```
<h3 id="/Query.clinicalSynopsis"> 2. /Query.clinicalSynopsis</h3>


**Prototype**: ``OMIM_CLI.CLI::Int32 QueryclinicalSynopsis(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Query.clinicalSynopsis /id <id|id_list|id_list.txt> [/out <out_file|std_out>]
```
###### Example
```bash
OMIM
```
<h3 id="/Query.entry"> 3. /Query.entry</h3>

Gets OMIM data by mim entry number.
**Prototype**: ``OMIM_CLI.CLI::Int32 QueryEntry(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Query.entry /id <id|id_list|id_list.txt> [/includes <all,...> /text <all> /out <out_file|std_out>]
```
###### Example
```bash
OMIM /Query.entry /id 100100,100200 /includes referenceList,clinicalSynopsis /text all /out ~/example.json
```



#### Parameters information:
##### /id
The OMIM database entry id or list of these id value.
If the id list source is a text file, then this text file format required of each line is a mim id number.
This makes more easy on large amount data query.

###### Example
```bash
/id "~/entries.txt"
```
##### [/includes]
Additional content can be retrieved using the 'include' parameter, the includes are as follows:

|Parameter         |Description|
|------------------|---------------------------------------------------------------------------------------------------------|
|text              |Includes the text field sections with the entry.|
|existflags        |Include the 'exists' flags with the entry (clinical synopsis, allelic variant, gene map & phenotype map).|
|allelicVariantList|Includes the allelic variant list with the entry.|
|clinicalSynopsis  |Include the clinical synopsis with the entry.|
|seeAlso           |Includes the 'see also' field with the entry.|
|referenceList     |Include the reference list with the entry.|
|geneMap           |Include the gene map/phenotype map data with the entry.|
|externalLinks     |Include the external links with the entry.|
|contributors      |Includes the 'contributors' field with the entry.|
|creationDate      |Includes the 'creation date' field with the entry.|
|editHistory       |Includes the 'edit history' field with the entry.|
|dates             |Include the dates data with the entry.|
|all               |Include the above data with the entry.|

###### Example
```bash
/includes referenceList,clinicalSynopsis
```
##### Accepted Types
###### /id
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

**Decalre**:  _System.String[]_
Example: 
```json
[
    "System.String"
]
```

###### /includes
**Decalre**:  _System.String[]_
Example: 
```json
[
    "System.String"
]
```

<h3 id="/Query.geneMap.entry"> 4. /Query.geneMap.entry</h3>


**Prototype**: ``OMIM_CLI.CLI::Int32 QueryGeneMaps_entry(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Query.geneMap.entry /id <id|id_list|id_list.txt> [/out <out_file|std_out>]
```
###### Example
```bash
OMIM
```
<h3 id="/Query.geneMap.loci"> 5. /Query.geneMap.loci</h3>


**Prototype**: ``OMIM_CLI.CLI::Int32 QueryGeneMaps_loci(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Query.geneMap.loci /chr <chr_id> /chr.Sort <int> [/phenotypeExists /start <default:0> /limit <default:0> /out <out_file|std_out>]
```
###### Example
```bash
OMIM
```
<h3 id="/Query.geneMap.seqID"> 6. /Query.geneMap.seqID</h3>


**Prototype**: ``OMIM_CLI.CLI::Int32 QueryGeneMaps_seqID(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Query.geneMap.seqID /seqid <id> [/phenotypeExists /start <default:0> /limit <default:0> /out <out_file|std_out>]
```
###### Example
```bash
OMIM
```
<h3 id="/Search.clinicalSynopsis"> 7. /Search.clinicalSynopsis</h3>


**Prototype**: ``OMIM_CLI.CLI::Int32 clinicalSynopsisSearch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Search.clinicalSynopsis
```
###### Example
```bash
OMIM
```
<h3 id="/Search.entry"> 8. /Search.entry</h3>


**Prototype**: ``OMIM_CLI.CLI::Int32 EntrySearch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Search.entry /term <search_term> [/filter <term> /fields <null> /sort <sort_options> /out <out_file|std_out> /start 0 /limit 10 /retrieve <geneMap|clinicalSynopsis,default:null>]
```
###### Example
```bash
OMIM
```
<h3 id="/Search.geneMap"> 9. /Search.geneMap</h3>


**Prototype**: ``OMIM_CLI.CLI::Int32 geneMapSearch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Search.geneMap
```
###### Example
```bash
OMIM
```
<h3 id="/Set"> 10. /Set</h3>

Setup the OMIM API tools variables.
**Prototype**: ``OMIM_CLI.CLI::Int32 SetVar(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Set <var> <value>
```
###### Example
```bash
OMIM /Set "format" "json"
```
<h3 id="/Set.Key"> 11. /Set.Key</h3>

Setup the OMIM API key for query the database.
**Prototype**: ``OMIM_CLI.CLI::Int32 SetKey(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /Set.Key <key>
```
###### Example
```bash
OMIM /Set.Key "039583F9182AE1F7C343746B9B54C1F1BF1678A9"
```



#### Parameters information:
##### key
The api key of this OMIM query client.

###### Example
```bash
039583F9182AE1F7C343746B9B54C1F1BF1678A9
```
##### Accepted Types
###### key
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/vars"> 12. /vars</h3>

Listing all of the variables for using the `/Set` command.
**Prototype**: ``OMIM_CLI.CLI::Int32 ListSettings(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
OMIM /vars
```
###### Example
```bash
OMIM /vars
```
