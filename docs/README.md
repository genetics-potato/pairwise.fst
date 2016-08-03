# Pairwise.Fst [version 1.0.0.0]
**Module AssemblyName**: file:///G:/pairwise.fst/Pairwise.Fst/bin/Release/fst.exe
**Root namespace**: Pairwise.Fst.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/fst||
|/pairwise.fst||
|/pairwise.snp.fst||
|/pairwise.snp.fst.batch||
|/SNP.fst||

## Commands
--------------------------
##### Help for command '/fst':

**Prototype**: Pairwise.Fst.CLI::Int32 fst(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\pairwise.fst\Pairwise.Fst\bin\Release\fst.exe /fst /in <genotype.Csv> [/out <out.json>]
  Example:      fst /fst 
```



  Parameters information:
```
    /in
    Description:  
    Example:      /in ""

   [/out]
    Description:  
    Example:      /out ""


```

#### Accepted Types
##### /in
**Decalre**:  _Pairwise.Fst.FstPop_
Example: 
```json
{
    "Population": "System.String",
    "a": 0,
    "b": 0,
    "c": 0
}
```

##### /out
**Decalre**:  _Pairwise.Fst.F_STATISTICS_
Example: 
```json
{
    "FIS": 0,
    "FIT": 0,
    "FST": 0
}
```

##### Help for command '/pairwise.fst':

**Prototype**: Pairwise.Fst.CLI::Int32 pairwisefst(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\pairwise.fst\Pairwise.Fst\bin\Release\fst.exe /pairwise.fst /in <in.csv> [/out <out.csv>]
  Example:      fst /pairwise.fst 
```

##### Help for command '/pairwise.snp.fst':

**Prototype**: Pairwise.Fst.CLI::Int32 pairwisefst_SNP(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\pairwise.fst\Pairwise.Fst\bin\Release\fst.exe /pairwise.snp.fst /in <snp.genotypes.csv> [/out <out.csv>]
  Example:      fst /pairwise.snp.fst 
```

##### Help for command '/pairwise.snp.fst.batch':

**Prototype**: Pairwise.Fst.CLI::Int32 pairwisefstSNPBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\pairwise.fst\Pairwise.Fst\bin\Release\fst.exe /pairwise.snp.fst.batch /in <snp.genotypes.csv.DIR> [/out <out.csv.DIR>]
  Example:      fst /pairwise.snp.fst.batch 
```

##### Help for command '/SNP.fst':

**Prototype**: Pairwise.Fst.CLI::Int32 SNPFst(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\pairwise.fst\Pairwise.Fst\bin\Release\fst.exe /SNP.fst /in <snp.genotype.Csv> [/out <out.json>]
  Example:      fst /SNP.fst 
```



  Parameters information:
```
    /in
    Description:  
    Example:      /in ""

   [/out]
    Description:  
    Example:      /out ""


```

#### Accepted Types
##### /in
**Decalre**:  _Pairwise.Fst.SNPGenotype_
Example: 
```json
{
    "AlleleFrequency": "",
    "Frequency": [
        
    ],
    "GenotypeFreqnency": "",
    "Genotypes": [
        
    ],
    "Population": "System.String",
    "ssID": "System.String"
}
```

##### /out
**Decalre**:  _Pairwise.Fst.F_STATISTICS_
Example: 
```json
{
    "FIS": 0,
    "FIT": 0,
    "FST": 0
}
```

