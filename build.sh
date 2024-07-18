#!/usr/bin/env bash

files=( Preset.Binding OpenTabletDriver.External.Common OTD.UX.Remote.Lib)

dotnet publish -c Release -o temp

# Move the Preset.Binding.dll to the build folder
# check if the build folder does not exist, else create it
if [ ! -d "build" ]; then
  mkdir build
else
  rm -rf build/*
fi

for file in "${files[@]}"; 
do
  cp temp/$file.dll build
  cp temp/$file.pdb build
done

(
  cd build
  # zip the Preset.Binding.dll
  jar -cfM Preset.Binding.zip ./Preset.Binding.dll
)

# Remove the temp folder
rm -rf temp