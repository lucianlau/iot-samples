import json

# var config = { splitby: 'keyname1[.]keyname2[.]keyname3[.]' }
#var config = {splitby:'root'}
#var config = {splitby:'root[.]a1'}
#var config = {splitby:'root[.]a1[.]a2'}
#var config =  {splitby:'root[.]a1[.]'}

config = {splitby:''}
msgToSend = []

def processMsg(msg):
    arraykeys = str(config["splitby"]).split(sep='[.]')
    if (len(arraykeys) == 1 or len(arraykeys[0]) <=0):
        if item in msg.values:
            for val in msg.values:
                print(va)
                #msgToSend.insert(val)
    if(len(arraykeys)==1 and len(arraykeys[0])>0):
        if item in msg.values:
            for val in msg.values:
                print(val)

                #if val == arraykeys[0]:



