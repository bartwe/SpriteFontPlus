#!/bin/bash
set -euo pipefail

cd "$(dirname "$0")"
exec ./Build.DebugMacOS.sh DebugMacOSX64 x86_64
