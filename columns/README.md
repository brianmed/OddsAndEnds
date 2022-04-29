# Summary

columns is a utility that extracts out columns from a file or stdin.

There several options.  The --ifs and --ofs options control the input and ouput column defintions.
The --columns allow for several types of ranges.

| Example | Explanation |
| ------- | ----------- |
| 0 |   Single Column |
| 3- | Start at index 3 goto the end |
| -5 | Beginning to index 5 |
| 1-3 | Index 1 to 3 |
| 5-3 | Index 5 to 3 counting down |
| ^1-3 | End index to index 3, counting down |
| 4-^1 | Index 4 to the last index, counting down |

^0 and ^1 are treated the same

The --step option is the column enumeration step.  The defualt is 1.

```bash
$ columns --help
Description:
  Extract columns from lines in a file.

Usage:
  columns [options]

Options:
  --columns <columns> (REQUIRED)  List of Columns to Extract
  --debug                         Debug Output
  --ifs <ifs>                     Delimiter for Input Columns
  --input <input>                 Input File
  --ofs <ofs>                     Delimiter for Output Columns
  --step <step>                   Column Traversal Step
  --version                       Show version information
  -?, -h, --help                  Show help and usage information
```

# Examples

All columns using 0 and ^0.

```bash
$ echo '0,1,2,3' | columns --ifs , --columns 0-^0          
0,1,2,3
```

All columns using ^0 and 0 (in reverse).

```bash echo '0,1,2,3' | columns --ifs , --columns ^0-0
3,2,1,0
```

Every other column.

```bash
$ echo '0,1,2,3,4\n5,6,7,8,9' | columns --ifs , --columns 0-^0 --step 2
0,2,4
5,7,9
```
