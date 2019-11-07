#! /bin/bash

echo "Robot-One Linux Installer"

echo "Installing requirements"
sudo apt-get install gcc-4.9 g++-4.9
if which g++-4.9 >/dev/null; then
    echo "GCC-4.9 - OK"
else
   sudo add-apt-repository ppa:ubuntu-toolchain-r/test
   sudo cp /etc/apt/sources.list ./sources.list.bkp
   sudo sh -c "echo 'deb http://dk.archive.ubuntu.com/ubuntu/ xenial main' >> /etc/apt/sources.list"
   sudo sh -c "echo 'deb http://dk.archive.ubuntu.com/ubuntu/ xenial universe' >> /etc/apt/sources.list"
   sudo apt update && sudo apt install gcc-4.9 g++-4.9
   sudo cp ./sources.list.bkp /etc/apt/sources.list
   sudo apt-get update
   sudo apt-get upgrade libstdc++6
fi

echo "Compiling C/C++ Library..."
cd lib/c_cpp
mkdir -p build-linux
cd build-linux
cmake -D CMAKE_C_COMPILER=gcc-4.9 -D CMAKE_CXX_COMPILER=g++-4.9 ..
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

echo "Installing application..."
mkdir -p ~/robot-one/app
cp -r app ~/robot-one/

echo "Setting Python path..."
add_python_path () {
        if ! echo "$PYTHONPATH" | /bin/grep -Eq "(^|:)$1($|:)" ; then
           echo "export PYTHONPATH=\"${PYTHONPATH}:$1\"" >> ~/.bashrc
           if [ "$2" = "after" ] ; then
              PYTHONPATH="$PYTHONPATH:$1"
           else
              PYTHONPATH="$1:$PYTHONPATH"
           fi
        fi
}

add_python_path ${HOME}/robot-one/lib/python

echo "Configuring application..."
chmod +x ~/robot-one/app/robotOne.x86_64

echo "cd ${HOME}/robot-one/app; ./robotOne.x86_64; cd ${PWD}" >> ~/robot-one/app/robotOne
chmod +x ~/robot-one/app/robotOne
sudo ln -sf ~/robot-one/app/robotOne /usr/local/bin/robotOne

echo "Done!"