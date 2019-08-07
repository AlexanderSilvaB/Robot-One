#! /bin/sh

PROJECT_PATH=$(pwd)/$UNITY_PROJECT_PATH
UNITY_BUILD_DIR=$(pwd)/Build
LOG_FILE=$UNITY_BUILD_DIR/unity-win.log


ERROR_CODE=0
echo "Items in project path ($PROJECT_PATH):"
ls "$PROJECT_PATH"


echo "Building project for Windows..."
mkdir $UNITY_BUILD_DIR
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath "$PROJECT_PATH" \
  -buildWindows64Player  "$(pwd)/build/win/RobotOne.exe" \
  -quit

echo "Building project for OS X..."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath "$PROJECT_PATH" \
  -buildOSXUniversalPlayer "$(pwd)/build/osx/RobotOne.app" \
  -quit

echo "Building project for Linux..."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath "$PROJECT_PATH" \
  -buildLinuxUniversalPlayer "$(pwd)/build/linux/RobotOne"  \
  -quit
  
if [ $? = 0 ] ; then
  echo "Building completed successfully."
  ERROR_CODE=0
else
  echo "Building failed. Exited with $?."
  ERROR_CODE=1
fi

echo 'Logs from build'
cat $(pwd)/unity.log

echo "Finishing with code $ERROR_CODE"
exit $ERROR_CODE