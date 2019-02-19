# Written by Jason D'Costa
# Pratik Chowdhury helped criticize my music choices including heavy metal

import csv
data = csv.reader(open('Bom.csv'), delimiter = ',')
next(data)  #Skip the 1st header row.
f = open('Bom_test.kml', 'w')   #Open the file to be written.

f.write("<?xml version='1.0' encoding='UTF-8'?>\n") #Writing the kml file.
f.write("<kml xmlns='http://earth.google.com/kml/2.1'>\n")
f.write("<Document>\n")
f.write("   <name>" + 'Bom_test.kml' +"</name>\n")
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
print ("File Created.")