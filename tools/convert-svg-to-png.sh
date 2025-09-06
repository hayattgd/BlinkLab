#!/bin/bash
# this script uses Inkscape to convert, so make sure you have Inkscape installed on your system.
cd ../assets
inkscape ./* --export-type=png -d 192
mv ./*.png ../res/Icons/
cd ../res/Icons
optipng -o7 -strip all ./*
