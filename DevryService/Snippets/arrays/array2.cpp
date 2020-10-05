#include <array>

// array<type, sizeOfArray> variableName {comma delimeted contents };

array<int, 5> numbers{1, 20, 30, 40, 50};

for (int i = 0; i < numbers.size(); i++)
    cout << numbers[i] << endl;