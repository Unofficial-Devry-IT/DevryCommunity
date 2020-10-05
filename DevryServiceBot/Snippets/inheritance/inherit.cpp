
#include <iostream>
#include <string>
using namespace std;

class Person
{
public:
    string Name;
    string DOB;

    Person(string name, string dob)
    {
        this->Name = name;
        this->DOB = dob;
    }

    void Speak(string text)
    {
        cout << this->Name << ": " << text << endl;
    }
};

class Employee : public Person
{
public:
    int Id;

    Employee(string name, string dob, int id) : Person(name, dob)
    {
        this->Id = id;
    }

    void Quit()
    {
        this->Speak("Screw you guys I'm going home");
    }
};

int main()
{
    Person person = Person("Jim Bob", "09/01/1990");
    Employee emp = Employee("Jimmaayyyy", "01/01/2000", 12312312);

    person.Speak("Herro there");
    emp.Speak("I hate all the things");
    emp.Quit();
}
