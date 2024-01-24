# Strict CSV

A recration of the CSV specification under much stricter guidelines to simplify serialization and deserialization.

## Goal

The aim of this format is to be (de)serializable without user intervention. Unlike CSV, which allows for different delimiters, string delimiters, encodings and formats, Strict CSV defines strict format guidelines, which completely remove the guesswork when importing/exporting data.

The secondary aim is to have a format that is relatively easy to (de)serialize, especially when performance is not critical.

Human readibility is not accounted. This format is only meant to be used by machines for information exchange.

This was partly inspired by the [WSV](https://dev.stenway.com/WSV/Specification.html) (Whitespace Separated Values) format, however, this format aims to be more familiar to those used to the CSV format, and more performant (at serialization/deserialization) at the expense of human readibility.

## Specifications

- [Encoding](#encoding)
- [Values](#values)
- [Rows](#rows)
- [Header](#header)
- [Compact](#compact)
- [File Extension](#file-extension)
- [Example](#example)
- [Changelog](#changelog)

### Encoding

One of the following encodings must be used:

- UTF-8
- UTF-16 (Big Endian)
- UTF-16 Reversed (Little Endian)
- UTF-32 (Big Endian)
- UTF-32 Reversed (Little Endian)

All encodings, except for UTF-8, must include a preamble (BOM - Byte Order Mark) indicating the used encoding. UTF-8 may include BOM, but it is not required. If BOM is missing, UTF-8 is the assumed (default) encoding.

### Special Characters

- Line Feed
  - Abbreviation: `LF`
  - Character: `\n`
  - Unicode: `U+000A`
  - ASCII
    - Decimal: `10`
    - Hexadecimal: `0x0a`
- Carriage Return
  - Abbreviation: `CR`
  - Character: `\r`
  - Unicode: `U+000D`
  - ASCII
    - Decimal: `13`
    - Hexadecimal: `0x0d`
- Comma
  - Character: `,`
  - Unicode: `U+002C`
  - ASCII
    - Decimal: `44`
    - Hexadecimal: `0x2c`
- Double Quote
  - Character: `"`
  - Unicode: `U+0022`
  - ASCII
    - Decimal: `34`
    - Hexadecimal: `0x22`

### Values

Each non-null value must be surrounded by double quote characters. The contained double quote characters must be written as an escaped sequence of two double quote characters.

Multiline values must use a single carriage return character to delimit each line. The line feed character and its combinations with the carriage return character are not supported.

Empty values not surrounded by double quote characters are treated as `null` values, which differ from empty strings (`""`).

Values must be delimited by a single comma character.

### Rows

Each row is delimited by a single line feed character. The carriage return character and its combinations with the line feed character are not supported here.

The last row must not end with a delimiter.

### Header

The first line must contain the header with column titles in the same format as normal rows. All following rows must include the same number of values per row and the header.

### Compact

The formatted data must be 'compact' and minimal, in other words, there must be no extra characters between commas and (double quoted) values (such as white spaces), between rows, before and after the header and last row, and etc.

Comments are not supported.

### File Extension

The default file extension is `.scsv`. If the OS doesn't support extensions longer than 3 characters, `.ssv` can be used.

## Example

For visualizing, the line feed and carriage return characters have been replaced with the LF and CR strings respectively.

```csv
"Value Type","Example"LF
"Normal","something or another"LF
"Contains Double Quotes","""The way to get started is to quit talking and begin doing"" -Walt Disney"LF
"Multiline","first lineCRsecond lineCRthird line"LF
"Empty String",""LF
"Null",
```

## Changelog

### 0.1.0

First draft of the `Strict CSV` format Specifications.
