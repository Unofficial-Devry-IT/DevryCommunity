#include <iostream>
#include <fstream>
using namespace std;

ofstream config;
config << "verison=1.0.0" << endl;
config << "name=Mr. Bigglesworth" << endl;
config.close(); // MUST remember to close this otherwise it'll be considered "open by another program later on"