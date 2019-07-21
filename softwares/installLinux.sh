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