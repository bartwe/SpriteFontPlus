#!/bin/bash
cd "$(dirname "$0")"
mkdir -p bin/DebugLinuxArm32
mkdir -p obj/DebugLinuxArm32
rm -f ./bin/DebugLinuxArm32/*.so
rm -f ./obj/DebugLinuxArm32/*.o
gcc -c -lm -fPIC -O2 LinuxUnifiedBuild.cpp -o obj/DebugLinuxArm32/LinuxUnifiedBuild.o
gcc -O2 -lm obj/DebugLinuxArm32/LinuxUnifiedBuild.o -o ./bin/DebugLinuxArm32/SpriteFontPlus.Native.so -shared
