If you intend to install Octicons locally, install `octicons-local.ttf`. It should appear as “github-octicons” in your font list. It is specially designed not to conflict with GitHub's web fonts.

### Table 3 - > Table 4

In this step, I used a regex to filter out all of the junk, and switch the order of the unicode-value and the name:

This regex:
```\.octicon-(?<name>[^:]+):before \{ content: (?<value>[^\}]+)\}(?<rest>[^\n]+)(?<n>\n)?```

And this replacement:
```${value} ${name}${n}```

Gives us what we want. 

### Table 4 -> Table 5

In this stage, we:

* sort the file


### Table 5 - > Table 6

In this stage, we:

* remove the first column (because we want to index by the position (row) and not the unicode value)

```bash
cat octicons.table05 | cut -d " " -f 2 > octicons.table06
```

### Table 6 -> Table 7

In this stage, we use some sed magic:

```bash
sed -e 's/\-\([a-z]\)/\U\1/g' -e 's/^\([a-z]\)/\U\1/g' -e 's/$/,/g' < octicons.table06 > octicons.table07
```

This:
* Converts the dashed-base seperators to CamelCase-style
* Converts the first character to upper case
* Adds a comma at the end


### Table 7 -> (paste into source)

In this step, we remove entry **GitHubLogo**, and then copy/paste this into an enum!
