#include <iostream>
using namespace std;


double division(int a, int b)
{
    if (b == 0)
        throw "Division by zero condition";

    return (a / b);
}

int main()
{
    int a, b;

    cout << "Enter a number:" << endl;
    cin >> a;
    cout << "Enter another number:" << endl;
    cin >> b;

    try
    {
        int result = division(a, b);
        cout << "Result: " << result << endl;
    }
    catch (const char *msg)
    {
        cerr << msg << endl;
    }
}