# Summary

psort is a utility that is a line oriented sort, possibly uniquely.  However, the
the sorted output is displayed in sets, every s seconds.

It's rather simplistic.

```bash
$ psort --help
Description:
  Line oriented sort, possibly uniquely, with the sorted output displayed in
  sets, every s seconds.

Usage:
  psort [options]

Options:
  -s #      Number of seconds for each batch (default 5).
  -u        Sort uniquely.
  --help    Display this help.
```

# Examples

```bash
$ fswatch --latency 3 . | ggrep --line-buffered '\.md$' | psort -u
testOutput.md
```
