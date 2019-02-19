f = open('Bom.csv')
a = f.read()
f.close()
b = a.split('\n')
r = '<?xml version="1.0" encoding="UTF-8"?>\n<kml xmlns="http://www.opengis.net/kml/2.2">'
for x in b:
  x = x.replace(', ',',') #.decode('latin-1','ignore')
  y = x.split(',')
  if len(y) < 3:
    break
  elif len(y) > 3:
    desc = ' '.join(y[3:])
  else:
    desc = 'No description'
 
  # Replacing non-XML-allowed characters here (add more if needed)
  y[2] = y[2].replace('&','&amp;')
 
  desc = desc.replace('&','&amp;')
 
  r += '\n<Placemark><name>'+y[2].encode('utf-8','xmlcharrefreplace')+'</name>' \
    '\n<description>'+desc.encode('utf-8','xmlcharrefreplace')+'</description>\n' \
    '<Point><coordinates>'+str(y[0]).encode('utf-8','xmlcharrefreplace')+','+str(y[1]).encode('utf-8','xmlcharrefreplace')+',0</coordinates></Point>\n</Placemark>'
 
r += '\n</kml>'
f = open('Bom.kml','w')
f.write(r)
f.close()