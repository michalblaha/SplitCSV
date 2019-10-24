# Split CSV

Split large CSV into smaller, valid CSV. CSV files must use UTF-8 encoding.

`SplitCSV filename maxChunkSizeInBytes`

Example: `SplitCSV data.csv 100000`
- Splits data.csv into files with name `data_0001.csv`, `data_0002.csv`, `data_0003.csv` etc. Any file won't be larger than 100000 bytes. 



Header of CSV file (if is present) is considered as any other record. That's why present header is only in the first chunk.

