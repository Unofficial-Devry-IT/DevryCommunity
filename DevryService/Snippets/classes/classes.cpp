#include <iostream>
#include <string>
using namespace std;

class Animal
{
    public:
        string Name;
        int Age;

        Animal(string name, int age)
        {
            this->Age = age;
            this->Name = name;
        }

        void Move()
        {
            cout << this->Name << ": Moving..." << endl;
        }
};

int main()
{
    Animal animal = Animal("Dog", 4);
    animal.Move();
}