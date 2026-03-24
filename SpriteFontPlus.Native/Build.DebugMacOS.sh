#!/bin/bash
set -euo pipefail

cd "$(dirname "$0")"

if [[ $# -ne 2 ]]; then
    printf 'Usage: %s <ConfigurationName> <MacOSArch>\n' "$0" >&2
    printf 'Example: %s DebugMacOSArm64 arm64\n' "$0" >&2
    exit 1
fi

if [[ "$(uname -s)" != "Darwin" ]]; then
    printf 'Build.DebugMacOS.sh must be run on macOS.\n' >&2
    exit 1
fi

configurationName="$1"
macOsArch="$2"

case "$configurationName" in
    DebugMacOSX64|DebugMacOSArm64)
        ;;
    *)
        printf 'Unsupported configuration for SpriteFontPlus.Native macOS build: %s\n' "$configurationName" >&2
        exit 1
        ;;
esac

case "$macOsArch" in
    x86_64|arm64)
        ;;
    aarch64)
        macOsArch="arm64"
        ;;
    *)
        printf 'Unsupported macOS target architecture for SpriteFontPlus.Native: %s\n' "$macOsArch" >&2
        exit 1
        ;;
esac

outputDirectory="./bin/${configurationName}"
objectDirectory="./obj/${configurationName}"

mkdir -p "$outputDirectory"
mkdir -p "$objectDirectory"
rm -f "${outputDirectory}/SpriteFontPlus.Native.dylib"
rm -f "${objectDirectory}"/*.o

if [[ -n "${CXX:-}" ]]; then
    cxx="${CXX}"
elif command -v clang++ >/dev/null 2>&1; then
    cxx="clang++"
else
    cxx="g++"
fi

"$cxx" \
    -std=c++20 \
    -fPIC \
    -O2 \
    -I . \
    -arch "$macOsArch" \
    -c LinuxUnifiedBuild.cpp \
    -o "${objectDirectory}/LinuxUnifiedBuild.o"

"$cxx" \
    -dynamiclib \
    -arch "$macOsArch" \
    -o "${outputDirectory}/SpriteFontPlus.Native.dylib" \
    "${objectDirectory}/LinuxUnifiedBuild.o" \
    -lm
