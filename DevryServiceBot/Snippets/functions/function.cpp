#include <iostream>
using namespace std;

// This is a function
int Average(int a, int b)
{
    return (a + b) / 2;
}

int main()
{
    int average = Average(3, 22); // No we can reuse this function to calculate average!
    cout << "Average:\t" << average << endl;
}