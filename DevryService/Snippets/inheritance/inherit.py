class Person:
    def __init__(self, name, dob):
        self.name = name
        self.dob = dob
    
    def speak(self, text):
        print("{}: {}".format(self.name, text))
    
    def __str__(self):
        return "Name: {}\nDOB: {}".format(self.name, self.dob)
    
class Employee(Person):
    def __init__(self, name, dob, id):
        super().__init__(name, dob)
        self.id = id
    
    def quit(self):
        self.speak("Screw you guys I'm going home")
    
    def __str__(self):
        return super().__str__() + "\nId: {}".format(self.id)

person = Person("Jim Bob", "09/01/2000")
emp = Employee("Jimmayyy", "01/01/1990", 341234234)

person.speak("I'm a person")
emp.speak("I'm also a person, but also an employee")

emp.quit()

print("")
print(person)
print("")
print(emp)
# person does not have the ability to quit
#person.quit()