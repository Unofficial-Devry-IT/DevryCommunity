#include <iostream>
#include <fstream>
#include <string>
using namespace std;

ifstream config("config.cfg");
string text;

if(config.is_open())
{
    while(getline(config, text))
        cout << text << endl;    
}

config.close(); // CANNOT forget to close this. Otherwise it'll be considered open by another program