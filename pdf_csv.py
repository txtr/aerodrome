# Thanks to user chezou on GitHub
import tabula

# Read pdf into DataFrame
df = tabula.read_pdf("Airports/EC-AD-2.1VAKE-en-GB.pdf", output_format='csv', lattice=True, pages=3, multiple_tables=True) #, area=(406, 24, 695, 589))
#Option to select pages automatically yet to be added

clean_df = df.replace('\r',' ', regex=True) # This replaces \r in lattice mode to spaces

# Read remote pdf into DataFrame, we can use this if doc is hosted online
#df2 = tabula.read_pdf("https://github.com/tabulapdf/tabula-java/raw/master/src/test/resources/technology/tabula/arabic.pdf")

# convert PDF into CSV
#tabula.convert_into("data.pdf", "output.csv", output_format="csv") We don't use this though

export_csv = df.to_csv (r'export_dataframe.csv', index = None, header=True)
print(df)

export_csv = clean_df.to_csv (r'export_dataframe_clean.csv', index = None, header=True)
print(clean_df)

