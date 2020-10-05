class Animal:
    def __init__(self, name, age):
        self.name = name
        self.age = age
    
    def move(self):
        print("{}: Moving...".format(self.name))

animal = Animal("Dog", 4)
animal2 = Animal("Cat", 2)

animal.move()
animal2.move()