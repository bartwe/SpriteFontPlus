#!/bin/bash
set -euo pipefail

cd "$(dirname "$0")"
exec ./Build.DebugMacOS.sh DebugMacOSArm64 arm64
