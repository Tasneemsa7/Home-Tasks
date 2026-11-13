
using System;

class Program
{
    static void Main()
    {
        Student student = new Student("Ahmed", 20, "Level 2");

        Teacher teacher = new Teacher("Mona", 35, "Mathematics");

        Admin admin = new Admin("Ali", 40, "Student Affairs");

        // Polymorphism
        Person[] people = { student, teacher, admin };

        foreach (Person person in people)
        {
            Console.WriteLine("--------------------");
            person.ShowInfo();
            person.PerformRole();
        }

        Console.WriteLine("--------------------");

        // Unique methods
        student.Study();
        teacher.Teach();
        admin.Manage();
    }
}

// Abstract Base Class
abstract class Person
{
    // Shared properties
    public string Name { get; set; }
    public int Age { get; set; }

    // Constructor
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    // Abstract property
    public abstract string Role { get; }

    // Abstract method
    public abstract void PerformRole();

    // Concrete virtual method
    public virtual void ShowInfo()
    {
        Console.WriteLine("Name: " + Name);
        Console.WriteLine("Age: " + Age);
    }
}

// Derived Class 1
class Student : Person
{
    public string GradeLevel { get; set; }

    public Student(string name, int age, string gradeLevel)
        : base(name, age)
    {
        GradeLevel = gradeLevel;
    }

    public override string Role
    {
        get { return "Student"; }
    }

    public override void PerformRole()
    {
        Console.WriteLine("I am studying.");
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
        Console.WriteLine("Role: " + Role);
        Console.WriteLine("Grade: " + GradeLevel);
    }

    public void Study()
    {
        Console.WriteLine(Name + " is studying.");
    }
}

// Derived Class 2
class Teacher : Person
{
    public string Subject { get; set; }

    public Teacher(string name, int age, string subject)
        : base(name, age)
    {
        Subject = subject;
    }

    public override string Role
    {
        get { return "Teacher"; }
    }

    public override void PerformRole()
    {
        Console.WriteLine("I am teaching students.");
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
        Console.WriteLine("Role: " + Role);
        Console.WriteLine("Subject: " + Subject);
    }

    public void Teach()
    {
        Console.WriteLine(Name + " teaches " + Subject + ".");
    }
}

// Derived Class 3
class Admin : Person
{
    public string Department { get; set; }

    public Admin(string name, int age, string department)
        : base(name, age)
    {
        Department = department;
    }

    public override string Role
    {
        get { return "Admin"; }
    }

    public override void PerformRole()
    {
        Console.WriteLine("I am managing the school.");
    }

    public sealed override void ShowInfo()
    {
        base.ShowInfo();
        Console.WriteLine("Role: " + Role);
        Console.WriteLine("Department: " + Department);
    }

    public void Manage()
    {
        Console.WriteLine(Name + " manages " + Department + ".");
    }
}
