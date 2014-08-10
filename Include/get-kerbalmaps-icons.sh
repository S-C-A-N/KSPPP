#!/bin/bash

c=0;
path="kerbalmaps-icons";
url="http://static.kerbalmaps.com/images/body";

for i in "moho" "eve" "kerbin" "duna" "dres" "jool" "eeloo" "gilly" "mun" "minmus" "ike" "laythe" "vall" "tylo" "bop" "pol"; do
#	wget -O "$path/$i" "$url-$i.png"
	if [[ "$c" -lt 10 ]];
	then
		mv "$path/$i.png" "$path/0$c-$i.png"	# 01, 02, 03 ... to make sure we get ordering right
	else
		mv "$path/$i.png" "$path/$c-$i.png"
	fi
	c=$(($c+1)) # increment counter
done

