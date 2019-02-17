import tabula

# Read pdf into DataFrame
df = tabula.read_pdf("Airports/EC-AD-2.1VABO-en-GB.pdf", output_format='csv', lattice=True, pages=4) #, area=(406, 24, 695, 589))
clean_df = df.replace('\r',' ', regex=True)

# Read remote pdf into DataFrame
#df2 = tabula.read_pdf("https://github.com/tabulapdf/tabula-java/raw/master/src/test/resources/technology/tabula/arabic.pdf")

# convert PDF into CSV
#tabula.convert_into("data.pdf", "output.csv", output_format="csv")
export_csv = df.to_csv (r'export_dataframe.csv', index = None, header=True)
print(df)

export_csv = clean_df.to_csv (r'export_dataframe_clean.csv', index = None, header=True)
print(clean_df)

