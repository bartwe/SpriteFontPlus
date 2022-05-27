#pragma once
#ifndef Shared_hpp
#define Shared_hpp

#ifndef BW_NS_IMPORT
#define BW_EXTERN_C extern "C" {
#define BW_EXTERN_C_END }
#ifndef BW_LINUX
#define BW_DECLSPEC __declspec(dllexport)
#define BW_CDECL __cdecl
#else
#define BW_DECLSPEC 
#define BW_CDECL 
// only suitable for 32bit __attribute__((cdecl))
#endif
#else
#define BW_EXTERN_C
#define BW_EXTERN_C_END 
#define BW_DECLSPEC
#define BW_CDECL
#endif

#include <cstdio>
#include <cstring>
#include <cstdint>

template <class T>
const T& max(const T& a, const T& b) {
    return (a < b) ? b : a;
}

template <class T>
const T& min(const T& a, const T& b) {
    return (a > b) ? b : a;
}

template <class T>
const T abs(const T& a) {
    return (a < 0) ? -a : a;
}

#endif //Shared_hpp
