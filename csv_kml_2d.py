# Include Directory as command line argument
import csv
import os
import pathlib
import sys

#sys.argv[0] is name of script
root = str(sys.argv[1]) 
csvfolder = root + '/CSV/'
kml2dfolder = root + '/KML2D/'
list_csv = os.listdir(csvfolder)

# Create the folder where we have to Store KML2D Files if Not Exists
pathlib.Path(kml2dfolder).mkdir(parents=True, exist_ok=True) 

for filename in list_csv:
    # Verify if it is CSV File. If not, IGNORE
    if(pathlib.Path(filename).suffix.lower()!='.csv'):
        continue
    filename = pathlib.Path(filename).stem # Extract Just the File's Name For Ex:- VAAH.CSV becomes VAAH
    csvfile = csvfolder + filename + '.CSV' # Get Path to CSV File
    if(pathlib.Path(csvfile).suffix!='.CSV'):
        continue
    else:
        data = csv.reader(open(csvfile), delimiter = ',') # Open File for Reading
        next(data)  #Skip the 1st header row.
        kml_file = kml2dfolder + filename + ".KML"; # Get Path to KML File
        print(kml_file + " : " + filename )
        f = open(kml_file, 'w')   #Open the file to be written.

        f.write("<?xml version='1.0' encoding='UTF-8'?>\n") #Writing the kml file.
        f.write("<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">\n")
        f.write("<Document>\n")
        f.write("   <name>" + filename +"</name>\n")
        for row in data:
            f.write("   <Placemark>\n")
            f.write("       <name>" + str(row[1]) + "</name>\n")
            f.write("       <description>" + str(row[0]) + "</description>\n")
            f.write("       <Point>\n")
            f.write("           <coordinates>" + str(row[3]) + "," + str(row[2]) + "," + str(row[4]) + "</coordinates>\n")
            f.write("       </Point>\n")
            f.write("   </Placemark>\n")
        f.write("</Document>\n")
        f.write("</kml>\n")
        f.close()
        print ("File Created at: "+kml_file)