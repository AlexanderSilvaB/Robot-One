#! /bin/bash

echo "Robot-One Linux Installer"

echo "Compiling C/C++ Library..."
cd lib/c_cpp
mkdir -p build-linux
cd build-linux
cmake ..
make
echo "Installing C/C++ Library..."
sudo make install
cd ../../../

echo "Installing Python Library..."
mkdir -p ~/robot-one/lib/python
cp lib/python/robotOne.py ~/robot-one/lib/python/robotOne.py

echo "Installing Matlab Library..."
mkdir -p ~/robot-one/lib/matlab
cp -r lib/matlab/robotOne ~/robot-one/lib/matlab/

echo "Installing examples..."
mkdir -p ~/robot-one/examples
cp -r examples ~/robot-one/

echo "Setting Python path"
echo "export PYTHONPATH=\"${PYTHONPATH}:${HOME}/robot-one/lib/python\"" >> ~/.bashrc

echo "Done!"