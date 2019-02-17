# Read remote pdf into DataFrame, we can use this if doc is hosted online
#df2 = tabula.read_pdf("https://github.com/tabulapdf/tabula-java/raw/master/src/test/resources/technology/tabula/arabic.pdf")

# Thanks to user chezou on GitHub
import tabula
import PyPDF2
file="Airports/EC-AD-2.1VAAH-en-GB.pdf"

pdf_reader = PyPDF2.PdfFileReader(file,strict=True)
start = end = -1
for pg_no in range(0,pdf_reader.numPages):
    data = pdf_reader.getPage(pg_no).extractText()
    if data.find("AD 2.10 AERODROME OBSTACLES") != -1:
        start = pg_no
    if data.find("AD 2.11 METEOROLOGICAL INFORMATION PROVIDED") != -1:
        end = pg_no
        break

print('start='+str(start)+'\nend='+str(end))
list_int=[]
for num in range(start,end):
    list_int.append(num)      

# Read pdf into DataFrame
list = tabula.read_pdf(file, output_format='csv', lattice=True, pages=list_int, multiple_tables=True) #, area=(406, 24, 695, 589))

i=0
for df in list:
    clean_df = df.replace('\r',' ', regex=True) # This replaces \r in lattice mode to spaces
    export_csv = clean_df.to_csv (r'export_dataframe_clean_'+str(i)+'.csv', index = None, header=True) # This replaces \r in lattice mode to spaces
    print(clean_df)
    i+=1