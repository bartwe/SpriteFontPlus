#!/bin/bash
cd "$(dirname "$0")"
mkdir -p bin/DebugLinuxX64
mkdir -p obj/DebugLinuxX64
rm -f ./bin/DebugLinuxX64/*.so
rm -f ./obj/DebugLinuxX64/*.o
gcc -c -lm -fPIC -O2 LinuxUnifiedBuild.cpp -o obj/DebugLinuxX64/LinuxUnifiedBuild.o
gcc -O2 -lm obj/DebugLinuxX64/LinuxUnifiedBuild.o -o ./bin/DebugLinuxX64/SpriteFontPlus.Native.so -shared
