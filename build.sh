#!/bin/bash

dotnet publish -c Release -o temp

# Move the Preset.Binding.dll to the build folder
# check if the build folder does not exist, else create it
if [ ! -d "build" ]; then
  mkdir build
fi

cp temp/Preset.Binding.dll build

# Remove the temp folder
rm -rf temp