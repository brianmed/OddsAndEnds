# Summary

stream is a utility that is a line oriented find and replace using C# Regular Expressions.

Multiple regular expressions can be specified and they are processed in order.

```bash
$ stream --help
Description:
  Process regular expression(s) over lines in a file.

Usage:
  stream [options]

Options:
  -e        A regular expression.
  -f        A file with regular expressions.
  --help    Display this help.
```

# Examples

```bash
$ echo -n 'hello\nworld\n' | stream -e 's/hello/joy/'
joy
world
```
