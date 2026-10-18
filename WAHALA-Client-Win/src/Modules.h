#pragma once
#include <string>
#include "Network.h"

class Modules {
public:
    static void HandleCommand(const std::string& cmd, Network& net);
};
