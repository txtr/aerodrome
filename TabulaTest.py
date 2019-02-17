import tabula;
import pandas;
import PyPDF2;
from functools import reduce;

def SelectPageWithOurData(source: str) -> tuple:
    # Open the File
    file = open(source, 'rb');
    pdf_reader = PyPDF2.PdfFileReader(file,strict=True);

    print(pdf_reader.numPages);

    # The Page Number Values to Check
    start:int = -1;
    end:int = -1;
    # Loop Across Pages
    for pg_no in range(0,pdf_reader.numPages):
        # Extract Data
        data = pdf_reader.getPage(pg_no).extractText();
        # AD 2.10 is What We Need to Extract
        if data.find("AD 2.10") != -1:
            start = pg_no;
        # AD 2.11 comes later.
        # If that found, means
        # Last Page Number Tracked
        if data.find("AD 2.11") != -1:
            end = pg_no;
            break;
    # Return Tuple
    return (start,end);


def ConvertPDFToCSV(source_pdf: str, dest_csv:str):
    (start_page, end_page) = SelectPageWithOurData(source_pdf);

    dfs = tabula.read_pdf(source_pdf,multiple_tables=True,output_format="DataFrame",pages="{}-{}".format(start_page,end_page+1));
    print(len(dfs))
    # Delete all bounding rows and columns greater than ours
    dfs = list(filter(lambda x: len(x.columns) >= 5 and len(x.columns) < 6, dfs));
    df:pandas.DataFrame = pandas.concat(dfs);
    
    return df;

print ((ConvertPDFToCSV(".pdf","unused.csv")));