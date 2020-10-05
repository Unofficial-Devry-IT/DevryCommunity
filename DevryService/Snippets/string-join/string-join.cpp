#include <string>
#include <vector>
#include <iostream>

using namespace std;

void join(const vector<string>& items, char delimiter, string& ref)
{
    ref.clear();

    for(vector<string>::const_iterator p = items.begin();
        p != items.end();
        ++p)
        {
            ref += *p;
            if(p != items.end() - 1)
                    ref += delimiter;   
        }
}

int main()
{
    vector<string> items = 
    {
        "super",
        "awesome",
        "content",
        "here"
    };
    string text;

    join(items, "\n", text);

    cout << text << "\n";
}