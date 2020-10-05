public class Person
{
    public string Name;
    public DateTime DOB;

    public Person(string name, DateTime dob)
    {
        this.Name = name;
        this.DOB = dob;
    }

    public void Speak(string text)
    {
        Console.WriteLine($"{Name}: {text}");
    }

    public override string ToString()
    {
        return $"Name: {Name}\nDOB: {DOB.ToString("MM/dd/yyyy")}";
    }
}

public class Employee : Person
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Employee(string name, DateTime dob) : base(name, dob) { }
    
    public void Quit()
    {
        Console.WriteLine($"{Name}: Screw you guys... I'm going home!");
    }

    public override string ToString()
    {
        return base.ToString() + $"\nID: {Id}";
    }
}

Person person = new Person("Sam", new DateTime(2000, 01, 02));
Employee emp = new Employee("Jimmayyyyyyy", new DateTime(1900, 01, 02));

Console.WriteLine(person);
Console.WriteLine(emp);

emp.Quit();