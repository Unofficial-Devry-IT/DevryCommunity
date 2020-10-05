public class Animal
{
    public string Name;
    public int Age;

    public Animal(string name, int age)
    {
        this.Name = name;
        this.Age = age;
    }

    public void Move()
    {
        Console.WriteLine($"{Name}: Moving...");
    }
}

// How to create an instance of animal
Animal animal = new Animal("Dog", 4);
Animal animal2 = new Animal("Cat", 3);

// Call this instance's move method
animal.Move();
animal2.Move();